using System.Linq;

namespace BLIT.Investments;

public sealed class CollapsedPendingInvestment : CollapsedInvestment<PendingInvestment, PendingInvestmentData, BuyData, PendingSellData>
{
    public int CurrentIndividualSellPrice => SubInvestments.First().Data.CurrentIndividualSellPrice;
    public int TotalPotentialProfit => SubInvestments.Sum(i => i.PotentialProfit);
    public CollapsedPendingInvestment(params PendingInvestment[] subInvestments) : base(subInvestments) { }
}