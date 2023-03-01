namespace BLIT.Investments;

public sealed class CompletedInvestment : Investment<CompletedInvestmentData, BuyData, SellData>
{
    public CompletedInvestment(CompletedInvestmentData data) : base(data) { }
}
