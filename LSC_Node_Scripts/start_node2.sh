#!/bin/sh
Message="Changing dir..."
NodesDirectory="/Users/jtopham/LocalSCNodes"
echo $Message
cd ..
git checkout Test
cd src/Stratis.LocalSmartContracts.StandardSidechainD
echo "Running standard node 2..."
echo "**Data held in $NodesDirectory/node2**"
dotnet run --no-build -sidechain -datadir="$NodesDirectory/node2" -port=36203 -apiport=38203 -connect=127.0.0.1:36201 -bind=127.0.0.1