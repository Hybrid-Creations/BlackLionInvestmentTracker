using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using BLIT.ConstantVariables;

namespace BLIT.Investments;

[DataContract]
public class CompletedInvestment : Investment
{
    [DataMember] internal List<SellData> SellDatas { get; private set; } = new();
    public DateTimeOffset LatestSellDate => SellDatas.Max(s => s.Date);

    public bool AllSellDatasAreTheSame
    {
        get
        {
            var firstPrice = SellDatas.First().IndividualSellPrice;
            return SellDatas.All(s => s.IndividualSellPrice == firstPrice);
        }
    }

    public int IndividualSellPrice => SellDatas.First().IndividualSellPrice;
    public double AverageIndividualSellPrice => SellDatas.Average(s => s.IndividualSellPrice);
    public int TotalSellPrice => SellDatas.Sum(s => s.TotalSellPrice);

    public double IndividualProfit => (SellDatas.First().IndividualSellPrice * Constants.MultiplyTax) - BuyData.IndividualBuyPrice;
    /// <summary>
    /// The Average Indivual Profit We Got From Selling Reduced By The 15% BLTP Tax, Minus The Individual Price We Bought The Items For.
    /// </summary>
    public double AverageIndividualProfit => TotalProfit / SellDatas.Count;
    /// <summary>
    /// The Total Profit We Got From Selling Reduced By The 15% BLTP Tax, Minus The Total We Bought The Items For.
    /// </summary>
    public double TotalProfit => (TotalSellPrice * Constants.MultiplyTax) - BuyData.TotalBuyPrice;

    public CompletedInvestment(BuyData buyData, List<SellData> sellDatas) : base(buyData)
    {
        SellDatas = sellDatas;
    }

    public CompletedInvestment() { }
}
