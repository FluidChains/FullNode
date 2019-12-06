using System;
using System.Collections.Generic;
using System.Text;
using NBitcoin;
using Stratis.Bitcoin.Interfaces;
using Stratis.Bitcoin.Networks;

namespace Stratis.Bitcoin.Utilities
{
    public class MoneySupply
    {
        public StratisMain NetworkParameters = new StratisMain();

        public int GetMoneySupplyPoW(ConcurrentChain chain)
        {
            var poWLast = this.NetworkParameters.Consensus.LastPOWBlock;
            var poWReward = this.NetworkParameters.Consensus.ProofOfWorkReward / 100000000;
            var poSReward = this.NetworkParameters.Consensus.ProofOfStakeReward / 100000000;
            var blockMined = 1;
            int poWMoneySupply = 0;
            while (blockMined <= poWLast)
            {

                if (chain.GetBlock(blockMined).Header.CheckProofOfWork() == false)
                {
                    poWMoneySupply += (int)poSReward;
                }
                else
                {
                    if (chain.GetBlock(blockMined).Height != 2)
                    {
                        poWMoneySupply += (int)poWReward;
                    }

                }

                blockMined++;
            }

            return poWMoneySupply;
        }

    }
}
