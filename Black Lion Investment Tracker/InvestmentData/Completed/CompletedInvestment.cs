using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace BLIT.Investments;

public class CompletedInvestment : Investment
{
    public DateTimeOffset LatestSellDate => SellDatas.Max(s => s.Date);

    public override bool IndividualSellPriceDifferent
    {
        get
        {
            var firstPrice = SellDatas.First().IndividualSellPrice;
            return !SellDatas.All(s => s.IndividualSellPrice == firstPrice);
        }
    }

    public override int IndividualSellPrice => SellDatas.First().IndividualSellPrice;
    public override int AverageIndividualSellPrice => Mathf.RoundToInt(SellDatas.Average(s => s.IndividualSellPrice));
    public override int TotalSellPrice => SellDatas.Sum(s => s.TotalSellPrice);
    public int TotalNetSellPrice => SellDatas.Sum(s => s.NetSellPrice);

    /// <summary>
    /// The Average Indivual Profit We Got From Selling Reduced By The 15% BLTP Tax, Minus The Individual Price We Bought The Items For.
    /// </summary>
    public override int AverageIndividualProfit => Mathf.RoundToInt(TotalNetProfit / SellDatas.Sum(s => s.Quantity));

    /// <summary>
    /// The Total Profit We Got From; Selling Reduced By The 15% BLTP Tax, Minus The Total We Bought The Items For.
    /// </summary>
    public override int TotalNetProfit => TotalNetSellPrice - BuyData.TotalBuyPrice;

    public CompletedInvestment(BuyData buyData, List<SellData> sellDatas)
        : base(buyData)
    {
        SellDatas = sellDatas;
    }

    public CompletedInvestment() { }
}
