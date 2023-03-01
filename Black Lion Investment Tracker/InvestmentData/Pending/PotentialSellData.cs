using Gw2Sharp.WebApi.V2.Models;

namespace BLIT.Investments;

public partial class PotentialSellData : PendingSellData
{
    public PotentialSellData(CommerceTransactionCurrent currentSellTransaction) : base(currentSellTransaction) { }
}
