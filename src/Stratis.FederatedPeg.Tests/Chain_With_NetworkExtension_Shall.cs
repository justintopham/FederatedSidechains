﻿using FluentAssertions;
using NBitcoin;
using Stratis.Bitcoin.Networks;
using Stratis.FederatedPeg.Features.FederationGateway.NetworkHelpers;
using Stratis.Sidechains.Networks;
using Xunit;

namespace Stratis.FederatedPeg.Tests
{
    [Collection("FederatedPegTests")]
    public class Chain_With_NetworkExtension_Shall
    {
        [Fact]
        public void correctly_identify_mainchain()
        {
            var stratisRegTest = new StratisRegTest();
            Chain chain = stratisRegTest.ToChain();
            chain.Should().Be(Chain.Mainchain);
            chain.Should().NotBe(Chain.Sidechain);

            var stratisTest = new StratisTest();
            chain = stratisTest.ToChain();
            chain.Should().Be(Chain.Mainchain);
            chain.Should().NotBe(Chain.Sidechain);

            var stratisMain = new StratisMain();
            chain = stratisMain.ToChain();
            chain.Should().Be(Chain.Mainchain);
            chain.Should().NotBe(Chain.Sidechain);
        }

        [Fact]
        public void correctly_identify_sidechain()
        {	
            Network apexRegTest = FederatedPegNetwork.NetworksSelector.Regtest();
            Chain chain = apexRegTest.ToChain();
            chain.Should().Be(Chain.Sidechain);
            chain.Should().NotBe(Chain.Mainchain);

            Network apexTest = FederatedPegNetwork.NetworksSelector.Testnet();
            chain = apexTest.ToChain();
            chain.Should().Be(Chain.Sidechain);
            chain.Should().NotBe(Chain.Mainchain);

            Network apexMain = FederatedPegNetwork.NetworksSelector.Mainnet();
            chain = apexMain.ToChain();
            chain.Should().Be(Chain.Sidechain);
            chain.Should().NotBe(Chain.Mainchain);
        }
    }
}
