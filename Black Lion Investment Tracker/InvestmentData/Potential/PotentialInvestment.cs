using BLIT.ConstantVariables;

namespace BLIT.Investments;

public class PotentialInvestment : Investment
{
    int _currentSellPrice;
    internal int CurrentSellPrice
    {
        get => _currentSellPrice;
        set => _currentSellPrice = value - 1;
    }

    public int IndividualPotentialSellPrice => CurrentSellPrice; // Adjusted to be one copper less than the most recent posting
    public int TotalPotentialSellPrice => CurrentSellPrice * BuyData.Quantity; // Adjusted to be one copper less than the most recent posting

    /// <summary>
    /// The Indivual Potential Profit We Could Sell For Reduced By The 15% BLTP Tax, Minus The Individual Price We Bought The Items For.
    /// </summary>
    public int IndividualPotentialProfit => Constants.ApplySellingFees(CurrentSellPrice) - BuyData.IndividualBuyPrice;
    public override int AverageIndividualProfit => IndividualPotentialProfit;

    /// <summary>
    /// The Total We Could Sell For Reduced By The 15% BLTP Tax, Minus The Total We Bought The Items For.
    /// </summary>
    public int TotalPotentialProfit => Constants.ApplySellingFees(TotalPotentialSellPrice) - BuyData.TotalBuyPrice;
    public override int TotalNetProfit => TotalPotentialProfit;

    public PotentialInvestment(BuyData buyData)
        : base(buyData) { }

    public override bool IndividualSellPriceDifferent => false;
    public override int IndividualSellPrice => CurrentSellPrice;
    public override int AverageIndividualSellPrice => CurrentSellPrice;
    public override int TotalSellPrice => CurrentSellPrice * BuyData.Quantity;
}
