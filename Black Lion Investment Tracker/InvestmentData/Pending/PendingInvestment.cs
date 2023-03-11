using System;
using System.Collections.Generic;
using System.Linq;
using BLIT.ConstantVariables;

namespace BLIT.Investments;

public class PendingInvestment : Investment
{
    internal List<SellData> PostedSellDatas { get; private set; } = new();

    private readonly Lazy<int> lazyCurrentSellPrice;
    internal int CurrentSellPrice => lazyCurrentSellPrice.Value;
    public int LowestIndividualSellPrice => PostedSellDatas.Max(s => s.IndividualSellPrice);

    public bool AllSellDatasAreTheSame
    {
        get
        {
            var firstPrice = PostedSellDatas.First().IndividualSellPrice;
            return PostedSellDatas.All(s => s.IndividualSellPrice == firstPrice);
        }
    }

    public int IndividualListedSellPrice => PostedSellDatas.First().IndividualSellPrice;
    public double AverageIndividualListedSellPrice => PostedSellDatas.Average(s => s.IndividualSellPrice);
    public int LowestIndividualListedSellPrice => PostedSellDatas.Max(s => s.IndividualSellPrice);
    public int TotalListedSellPrice => PostedSellDatas.Sum(s => s.TotalSellPrice);

    public double IndividualProfit => (PostedSellDatas.First().IndividualSellPrice * Constants.MultiplyTax) - BuyData.IndividualBuyPrice;
    /// <summary>
    /// The Average Indivual Profit We Got From Selling Reduced By The 15% BLTP Tax, Minus The Individual Price We Bought The Items For.
    /// </summary>
    public double AverageIndividualProfit => TotalProfit / PostedSellDatas.Count;
    /// <summary>
    /// The Total Profit We Got From Selling Reduced By The 15% BLTP Tax, Minus The Total We Bought The Items For.
    /// </summary>
    public double TotalProfit => (TotalListedSellPrice * Constants.MultiplyTax) - BuyData.TotalBuyPrice;

    public PendingInvestment(BuyData buyData, List<SellData> sellDatas, Lazy<int> lazyCurrentSellPrice) : base(buyData)
    {
        PostedSellDatas = sellDatas;
        this.lazyCurrentSellPrice = lazyCurrentSellPrice;
    }
}
