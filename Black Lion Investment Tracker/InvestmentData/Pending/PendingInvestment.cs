using System;
using System.Collections.Generic;
using System.Linq;
using BLIT.ConstantVariables;

namespace BLIT.Investments;

public class PendingInvestment
{
    internal BuyData BuyData { get; private set; }
    internal List<SellData> PostedSellDatas { get; private set; } = new();

    public int IndividualPostedSellPrice => PostedSellDatas.First().IndividualSellPrice;
    public int TotalPostedSellPrice => PostedSellDatas.Sum(s => s.IndividualSellPrice * s.Quantity);

    /// <summary>
    /// The Indivual Profit We Got From Selling Reduced By The 15% BLTP Tax, Minus The Individual Price We Bought The Items For.
    /// </summary>
    public int IndividualPotentialProfit => (int)Math.Floor((PostedSellDatas.First().IndividualSellPrice * Constants.MultiplyTax) - BuyData.IndividualBuyPrice);
    /// <summary>
    /// The Total Profit We Got From Selling Reduced By The 15% BLTP Tax, Minus The Total We Bought The Items For.
    /// </summary>
    public int TotalPotentialProfit => (int)Math.Floor((TotalPostedSellPrice * Constants.MultiplyTax) - BuyData.TotalBuyPrice);

    public PendingInvestment(BuyData buyData, List<SellData> sellDatas)
    {
        BuyData = buyData;
        PostedSellDatas = sellDatas;
    }
}
