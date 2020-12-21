using NBitcoin;

namespace Divergenti.Networks.Consensus
{
    public class DivergentiPosConsensusOptions : PosConsensusOptions
    {
        public override int GetStakeMinConfirmations(int height, Network network)
        {
            // StakeMinConfirmations must equal MaxReorgLength so that nobody can stake in isolation and then force a reorg
            return (int)network.Consensus.MaxReorgLength;
        }
    }
}
