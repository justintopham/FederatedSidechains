#!/bin/sh
Message="Changing dir..."
NodesDirectory="/Users/jtopham/LocalSCNodes"
echo $Message
cd ..
git checkout Test
cd src/Stratis.LocalSmartContracts.FederationGatewayD
echo "Running miner 1..."
echo "Data held in $NodesDirectory/miner1"
dotnet run --no-build -sidechain -datadir="$NodesDirectory/miner1" -port=36201 -apiport=38201 -publickey=02f5b2a2fc2aa9f2ab85e9727720f9b280ed937f897e444810abaada26738b13c4 -mincoinmaturity=1 -mindepositconfirmations=1 -txindex=1 -redeemscript="1 02f5b2a2fc2aa9f2ab85e9727720f9b280ed937f897e444810abaada26738b13c4 1 OP_CHECKMULTISIG" -connect=0 -bind=127.0.0.1