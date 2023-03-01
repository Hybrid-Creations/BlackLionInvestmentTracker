namespace BLIT.Investments;

public sealed class PotentialInvestment : Investment<PotentialInvestmentData, BuyData, SellData>
{
    public PotentialInvestment(PotentialInvestmentData data) : base(data) { }
}
