using System;
using System.Collections.Generic;
using System.Linq;
using BLIT.ConstantVariables;
using Godot;

namespace BLIT.Investments;

public class CompletedInvestment : Investment
{
    internal List<SellData> SellDatas { get; private set; } = new();

    public int AverageIndividualSellPrice { get; private set; }
    public int TotalSellPrice { get; private set; }
    public DateTimeOffset LatestSellDate { get; private set; }

    /// <summary>
    /// The Indivual Profit We Got From Selling Reduced By The 15% BLTP Tax, Minus The Individual Price We Bought The Items For.
    /// </summary>
    public int AverageIndividualProfit { get; private set; }
    /// <summary>
    /// The Total Profit We Got From Selling Reduced By The 15% BLTP Tax, Minus The Total We Bought The Items For.
    /// </summary>
    public int TotalProfit { get; private set; }

    public CompletedInvestment(BuyData buyData, List<SellData> sellDatas) : base(buyData)
    {
        SellDatas = sellDatas;

        LatestSellDate = SellDatas.OrderBy(s => s.Date).First().Date;

        AverageIndividualSellPrice = Mathf.RoundToInt(SellDatas.Average(s => s.IndividualSellPrice));
        TotalSellPrice = SellDatas.Sum(s => s.IndividualSellPrice * s.Quantity);

        AverageIndividualProfit = Mathf.FloorToInt((AverageIndividualSellPrice * Constants.MultiplyTax) - BuyData.IndividualBuyPrice);
        TotalProfit = Mathf.FloorToInt((TotalSellPrice * Constants.MultiplyTax) - BuyData.TotalBuyPrice);
    }
}
