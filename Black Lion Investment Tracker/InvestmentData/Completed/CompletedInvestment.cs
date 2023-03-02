using System;
using System.Collections.Generic;
using System.Linq;
using BLIT.ConstantVariables;

namespace BLIT.Investments;

public class CompletedInvestment
{
    public BuyData BuyData { get; protected set; }
    internal List<SellData> SellDatas { get; set; } = new();

    public int IndividualSellPrice => SellDatas.First().IndividualSellPrice;
    public int TotalSellPrice => SellDatas.Sum(s => s.IndividualSellPrice * s.Quantity);
    public DateTimeOffset LatestSellDate => SellDatas.OrderBy(s => s.Date).First().Date;

    /// <summary>
    /// The Indivual Profit We Got From Selling Reduced By The 15% BLTP Tax, Minus The Individual Price We Bought The Items For.
    /// </summary>
    public int IndividualProfit => (int)Math.Floor((SellDatas.First().IndividualSellPrice * Constants.MultiplyTax) - BuyData.IndividualBuyPrice);
    /// <summary>
    /// The Total Profit We Got From Selling Reduced By The 15% BLTP Tax, Minus The Total We Bought The Items For.
    /// </summary>
    public int Profit => (int)Math.Floor((TotalSellPrice * Constants.MultiplyTax) - BuyData.TotalBuyPrice);

    public CompletedInvestment(BuyData buyData, List<SellData> sellDatas)
    {
        BuyData = buyData;
        SellDatas = sellDatas;
    }
}
