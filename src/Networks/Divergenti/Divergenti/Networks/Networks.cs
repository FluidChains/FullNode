using NBitcoin;
using Blockcore.Networks;


namespace Divergenti.Networks
{
    public static class Networks
    {
        public static NetworksSelector Divergenti
        {
            get
            {
                return new NetworksSelector(() => new DivergentiMain(), () => new DivergentiTest(), () => new DivergentiRegTest());
            }
        }
    }
}
