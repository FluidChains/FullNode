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
            var blockMined = 0;
            int poWMoneySupply = 0;
            while (blockMined <= poWLast)
            {
                if (chain.GetBlock(blockMined).Header.CheckProofOfWork() == false)
                {
                    poWMoneySupply++;
                }
                else
                {
                    poWMoneySupply += 12;
                }

                blockMined++;
            }

            return poWMoneySupply ;
        }

    }
}
