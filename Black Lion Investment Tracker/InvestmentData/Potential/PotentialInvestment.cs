using System;
using BLIT.ConstantVariables;

namespace BLIT.Investments;

public class PotentialInvestment : Investment
{
    private readonly Lazy<int> lazyCurrentSellPrice;
    internal int CurrentSellPrice => lazyCurrentSellPrice.Value;

    public int IndividualPotentialSellPrice => CurrentSellPrice;
    public int TotalPotentialSellPrice => CurrentSellPrice * BuyData.Quantity;

    /// <summary>
    /// The Indivual Potential Profit We Could Sell For Reduced By The 15% BLTP Tax, Minus The Individual Price We Bought The Items For.
    /// </summary>
    public int IndividualPotentialProfit => (int)Math.Floor((CurrentSellPrice * Constants.MultiplyTax) - BuyData.IndividualBuyPrice);
    /// <summary>
    /// The Total We Could Sell For Reduced By The 15% BLTP Tax, Minus The Total We Bought The Items For.
    /// </summary>
    public int TotalPotentialProfit => (int)Math.Floor((TotalPotentialSellPrice * Constants.MultiplyTax) - BuyData.TotalBuyPrice);

    public PotentialInvestment(BuyData buyData, Lazy<int> lazyCurrentSellPrice) : base(buyData)
    {
        this.lazyCurrentSellPrice = lazyCurrentSellPrice;
    }
}
