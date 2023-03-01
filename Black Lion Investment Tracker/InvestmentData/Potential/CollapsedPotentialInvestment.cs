using System.Linq;

namespace BLIT.Investments;

public sealed class CollapsedPotentialInvestment : CollapsedInvestment<PotentialInvestment, PotentialInvestmentData, BuyData, PotentialSellData>
{
    public int CurrentIndividualSellPrice => SubInvestments.First().Data.CurrentIndividualSellPrice;
    public int TotalPotentialProfit => SubInvestments.Sum(i => i.PotentialProfit);
    public CollapsedPotentialInvestment(params PotentialInvestment[] subInvestments) : base(subInvestments) { }
}
