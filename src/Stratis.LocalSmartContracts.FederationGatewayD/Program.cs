using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NBitcoin;
using NBitcoin.Protocol;
using Stratis.Bitcoin;
using Stratis.Bitcoin.Builder;
using Stratis.Bitcoin.Configuration;
using Stratis.Bitcoin.Features.Api;
using Stratis.Bitcoin.Features.BlockStore;
using Stratis.Bitcoin.Features.Consensus;
using Stratis.Bitcoin.Features.MemoryPool;
using Stratis.Bitcoin.Features.Miner;
using Stratis.Bitcoin.Features.Notifications;
using Stratis.Bitcoin.Features.RPC;
using Stratis.Bitcoin.Features.SmartContracts;
using Stratis.Bitcoin.Features.SmartContracts.Wallet;
using Stratis.Bitcoin.Features.PoA;
using Stratis.Bitcoin.Features.Wallet;
using Stratis.Bitcoin.Networks;
using Stratis.Bitcoin.Utilities;
using Stratis.FederatedPeg.Features.FederationGateway;
using Stratis.Sidechains.LocalSmartContracts.Networks;


namespace Stratis.LocalSmartContracts.FederationGatewayD
{
    class Program
    {
        private const string MainchainArgument = "-mainchain";
        private const string SidechainArgument = "-sidechain";

        private static void Main(string[] args)
        {
            RunFederationGatewayAsync(args).Wait();
        }

        private static async Task RunFederationGatewayAsync(string[] args)
        {
            try
            {
                bool isMainchainNode = args.FirstOrDefault(a => a.ToLower() == MainchainArgument) != null;
                bool isSidechainNode = args.FirstOrDefault(a => a.ToLower() == SidechainArgument) != null;

                if (isSidechainNode == isMainchainNode)
                {
                    throw new ArgumentException($"Gateway node needs to be started specifying either a {SidechainArgument} or a {MainchainArgument} argument");
                }

                IFullNode node = isMainchainNode ? GetMainchainFullNode(args) : GetSidechainFullNode(args);

                if (node != null)
                    await node.RunAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine("There was a problem initializing the node. Details: '{0}'", ex.Message);
            }
        }

        private static IFullNode GetMainchainFullNode(string[] args)
        {
            var nodeSettings = new NodeSettings(networksSelector: Networks.Stratis, protocolVersion: ProtocolVersion.PROVEN_HEADER_VERSION, args: args)
            {
                MinProtocolVersion = ProtocolVersion.ALT_PROTOCOL_VERSION
            };
            
            bool keyGenerationRequired = nodeSettings.ConfigReader.GetOrDefault("generateKeyPair", false);
            if (keyGenerationRequired)
            {
                GenerateFederationKey(nodeSettings.DataFolder);
                return null;
            }

            IFullNode node = new FullNodeBuilder()
                .AddCommonFeatures(nodeSettings)
                .UsePosConsensus()
                .UseWallet()
                .AddPowPosMining()
                .Build();

            return node;
        }

        private static IFullNode GetSidechainFullNode(string[] args)
        {
            var nodeSettings = new NodeSettings(network: new LocalSmartContractsMain(), protocolVersion: ProtocolVersion.ALT_PROTOCOL_VERSION, args: args)
            {
                MinProtocolVersion = ProtocolVersion.ALT_PROTOCOL_VERSION
            };
            
            bool keyGenerationRequired = nodeSettings.ConfigReader.GetOrDefault("generateKeyPair", false);
            if (keyGenerationRequired)
            {
                GenerateFederationKey(nodeSettings.DataFolder);
                return null;
            }

            IFullNode node = new FullNodeBuilder()
                .AddCommonFeatures(nodeSettings)
                .AddSmartContracts()
                .UseSmartContractWallet()
                .UseReflectionExecutor()
                .UseFederatedPegPoAMining()
                .Build();

            return node;
        }
        
        private static void GenerateFederationKey(DataFolder dataFolder)
        {
            var tool = new KeyTool(dataFolder);
            Key key = tool.GeneratePrivateKey();

            string savePath = tool.GetPrivateKeySavePath();
            tool.SavePrivateKey(key);

            var stringBuilder = new StringBuilder();

            stringBuilder.AppendLine($"Federation key pair was generated and saved to {savePath}.");
            stringBuilder.AppendLine("Make sure to back it up!");
            stringBuilder.AppendLine($"Your public key is {key.PubKey}.");
            stringBuilder.AppendLine();
            stringBuilder.AppendLine("Press eny key to exit...");

            Console.WriteLine(stringBuilder.ToString());

            Console.ReadKey();
        }
    }

    internal static class CommonFeaturesExtension
    {
        internal static IFullNodeBuilder AddCommonFeatures(this IFullNodeBuilder fullNodeBuilder, NodeSettings nodeSettings)
        {
            return fullNodeBuilder
                .UseNodeSettings(nodeSettings)
                .UseBlockStore()
                .AddFederationGateway()
                .UseTransactionNotification()
                .UseBlockNotification()
                .UseApi()
                .UseMempool()
                .AddRPC();
        }
    }
}
