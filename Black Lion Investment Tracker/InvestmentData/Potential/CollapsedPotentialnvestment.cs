namespace BLIT.Investments;

public sealed class CollapsedPotentialnvestment : CollapsedInvestment<PotentialInvestment, PotentialInvestmentData, BuyData, SellData>
{
    public CollapsedPotentialnvestment(params PotentialInvestment[] subInvestments) : base(subInvestments) { }
}
