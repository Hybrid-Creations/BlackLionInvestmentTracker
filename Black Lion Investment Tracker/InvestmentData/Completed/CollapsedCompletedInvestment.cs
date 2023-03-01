namespace BLIT.Investments;

public sealed class CollapsedCompletedInvestment : CollapsedInvestment<CompletedInvestment, CompletedInvestmentData, BuyData, SellData>
{
    public CollapsedCompletedInvestment(params CompletedInvestment[] subInvestments) : base(subInvestments) { }
}
