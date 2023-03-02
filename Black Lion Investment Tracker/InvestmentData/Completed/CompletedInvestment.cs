using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using BLIT.ConstantVariables;
using Godot;

namespace BLIT.Investments;

[DataContract]
public class CompletedInvestment : Investment
{
    [DataMember] internal List<SellData> SellDatas { get; private set; } = new();
    public DateTimeOffset LatestSellDate => SellDatas.OrderBy(s => s.Date).First().Date;

    public int AverageIndividualSellPrice => SellDatas.First().IndividualSellPrice;
    public int TotalSellPrice => SellDatas.Sum(s => s.TotalSellPrice);

    /// <summary>
    /// The Total Profit We Got From Selling Reduced By The 15% BLTP Tax, Minus The Total We Bought The Items For.
    /// </summary>
    public int TotalProfit => Mathf.RoundToInt((TotalSellPrice * Constants.MultiplyTax) - BuyData.TotalBuyPrice);
    /// <summary>
    /// The Indivual Profit We Got From Selling Reduced By The 15% BLTP Tax, Minus The Individual Price We Bought The Items For.
    /// </summary>
    public int AverageIndividualProfit => Mathf.RoundToInt(((TotalSellPrice * Constants.MultiplyTax) - BuyData.TotalBuyPrice) / SellDatas.Count);

    public CompletedInvestment(BuyData buyData, List<SellData> sellDatas) : base(buyData)
    {
        SellDatas = sellDatas;
    }

    public CompletedInvestment() { }
}
