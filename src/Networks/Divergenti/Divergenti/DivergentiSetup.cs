using System;
using System.Collections.Generic;
using Divergenti.Networks;
using Divergenti.Networks.Setup;
using NBitcoin;


namespace Divergenti
{
    internal class DivergentiSetup
    {
        internal static DivergentiSetup Instance = new DivergentiSetup();

        internal CoinSetup Setup = new CoinSetup
        {
            FileNamePrefix = "Divergenti",
            ConfigFileName = "divergenti.conf",
            Magic = "39-47-19-31",
            CoinType = 596, // SLIP-0044: https://github.com/satoshilabs/slips/blob/master/slip-0044.md,
            PremineReward = 800000000,
            PoWBlockReward = 250,
            PoSBlockReward = 25,
            LastPowBlock = 45000,
            GenesisText = "https://www.bbc.com/news/science-environment-55364664",
            TargetSpacing = TimeSpan.FromSeconds(64),
            ProofOfStakeTimestampMask = 0x0000003F, // 0x0000003F // 64 sec
            PoSVersion = 4
        };

        internal NetworkSetup Main = new NetworkSetup
        {
            Name = "DivergentiMain",
            RootFolderName = "divergenti",
            CoinTicker = "DIVER",
            DefaultPort = 3452,
            DefaultRPCPort = 3451,
            DefaultAPIPort = 39320,
            DefaultSignalRPort = 39820,
            PubKeyAddress = 30,  // D https://en.bitcoin.it/wiki/List_of_address_prefixes
            ScriptAddress = 90, // d
            SecretAddress = 158,
            GenesisTime = 1608582080,
            GenesisNonce = 88996,
            GenesisBits = 0x1E0FFFFF,
            GenesisVersion = 1,
            GenesisReward = Money.Zero,
            HashGenesisBlock = "000006fc5033beab628af59aebc6d9c983e33014b9fc3dda51e4b6c9a0b58f5a",
            HashMerkleRoot = "7e78875e1156338aef1f0c5f833e1495b0cbb76143110bf7709c0b4d21c2df5b",
            DNS = new[] { "", "", "", "" },
            Nodes = new[] { "", "", "", "" },
            Checkpoints = new Dictionary<int, CheckpointInfo>
            {
                 {       0, new CheckpointInfo(new uint256("0x000006fc5033beab628af59aebc6d9c983e33014b9fc3dda51e4b6c9a0b58f5a"), new uint256("0x0000000000000000000000000000000000000000000000000000000000000000")) },
                 {       2, new CheckpointInfo(new uint256("0xd16981abb754dcffc0812756bb4bedfeba43a80b7bd675b4563bd5d72344eb91"), new uint256("0xca0214716e5a2e8f54f08f42d1e79d48c35f3f141e2790d8a69fe138bda8b3eb")) }, //Premine
                 {      10, new CheckpointInfo(new uint256("0x140726d04676fbc95a38c6c9d77b90581395b974e9ee39dbcf2a4496628ee12a"), new uint256("0xc3e88926170bd0d6c65a5d1fe9fa2b6653133c8cb70c5b4ce4606719791895ce")) },
                 {     100, new CheckpointInfo(new uint256("0x0780097ff2fbc7c22af23610806a1455ecd9f8646ebfa4f9bcfda278ba2c6432"), new uint256("0x8e9541c7399f94e3d0082f2e88aafde6c9d8c914a372d0c39f4a02e75385368b")) },
                 {    1000, new CheckpointInfo(new uint256("0xc896ceb33aa41aad93d70160ddf0c3838034ac3c8f5c82577572fd991516aa87"), new uint256("0x4c644cf3335a00ae776063be85a9444ab7f539735efb58feb8866bd8815e33ee")) },
                 {    5000, new CheckpointInfo(new uint256("0xf6aa7c8201f0373dc28bbf4208cb0297a3046bb0cf3dbd0bd30b79b7e6c48ecc"), new uint256("0xc5d2d5bfb9d8f15b0c79cfc2c7a3bf9e859416ae986134cb1f31d18230e5daa3")) },
                 {   10000, new CheckpointInfo(new uint256("0x547ad33d55aa90705baab1a9292a36bfb4353fdf00c35e51e2f718bbd8d2df9a"), new uint256("0x9fccfea09c623c42f70224dbce3ff7e02a09ced3f373ba089ec7795e4f85d662")) },
                 {   20000, new CheckpointInfo(new uint256("0xb39e47f9cb09cf75146cfaea624d3a5605365e97b4975c89dbdc100589a99471"), new uint256("0x49f8b242334ba4e5acc3a2713d3531115791215287028c1c9fdc7ad3a0b5b951")) }
            }
        };

        internal NetworkSetup RegTest = new NetworkSetup
        {
            Name = "DivergentiMain",
            RootFolderName = "divergenti",
            CoinTicker = "TDIVER",
            DefaultPort = 3452,
            DefaultRPCPort = 3451,
            DefaultAPIPort = 39320,
            DefaultSignalRPort = 39820,
            PubKeyAddress = 30,  // D https://en.bitcoin.it/wiki/List_of_address_prefixes
            ScriptAddress = 90, // d
            SecretAddress = 158,
            GenesisTime = 1608582080,
            GenesisNonce = 88996,
            GenesisBits = 0x1F00FFFF,
            GenesisVersion = 1,
            GenesisReward = Money.Zero,
            HashGenesisBlock = "000006fc5033beab628af59aebc6d9c983e33014b9fc3dda51e4b6c9a0b58f5a",
            HashMerkleRoot = "7e78875e1156338aef1f0c5f833e1495b0cbb76143110bf7709c0b4d21c2df5b",
            DNS = new[] { "seednoderegtest1.divergenti.cloud" },
            Nodes = new[] { "vps301.divergenti.cloud" },
            Checkpoints = new Dictionary<int, CheckpointInfo>
            {
                // TODO: Add checkpoints as the network progresses.
            }
        };

        //TODO: Update parameters
        internal NetworkSetup Test = new NetworkSetup
        {
            Name = "DivergentiTest",
            RootFolderName = "divergentitest",
            CoinTicker = "TDIVE",
            DefaultPort = 16782,
            DefaultRPCPort = 16781,
            DefaultAPIPort = 39222,
            DefaultSignalRPort = 39721,
            PubKeyAddress = 68,
            ScriptAddress = 199,
            SecretAddress = 196,
            GenesisTime = 1542885720,
            GenesisNonce = 5218499,
            GenesisBits = 0x1F0FFFFF,
            GenesisVersion = 1,
            GenesisReward = Money.Zero,
            HashGenesisBlock = "000001eaf5fc73c5a2ded387a256d79b34b0acaaead5767a52c8c6081b79d031",
            HashMerkleRoot = "88cd7db112380c4d6d4609372b04cdd56c4f82979b7c3bf8c8a764f19859961f",
            DNS = new[] { "seedtest1.divergenti.cloud", "seedtest2.divergenti.network", "seedtest3.divergenti.cloud" },
            Nodes = new[] { "23.97.234.230", "13.73.143.193", "89.10.227.34" },
            Checkpoints = new Dictionary<int, CheckpointInfo>
            {
                // TODO: Add checkpoints as the network progresses.
            }
        };



        public bool IsPoSv3()
        {
            return Setup.PoSVersion == 3;
        }

        public bool IsPoSv4()
        {
            return Setup.PoSVersion == 4;
        }





    }
}
