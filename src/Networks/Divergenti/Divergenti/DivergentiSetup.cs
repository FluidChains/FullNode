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
            ProofOfStakeTimestampMask = 0x0000000F, // 0x0000003F // 64 sec
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
            PubKeyAddress = 39,  // D https://en.bitcoin.it/wiki/List_of_address_prefixes
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
            PubKeyAddress = 39,  // D https://en.bitcoin.it/wiki/List_of_address_prefixes
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
