using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Gw2Sharp.WebApi.V2.Models;
using Newtonsoft.Json;

namespace BLIT;

public partial class InvestmentData
{
    public long TransactionId;
    public int ItemId;
    public DateTimeOffset PurchaseDate;
    public int IndividualPrice;
    public int Quantity;
    public List<SellData> SellDatas;

    [JsonIgnore] public int TotalBuyPrice => IndividualPrice * Quantity;
    [JsonIgnore] public int TotalSellPrice => SellDatas.Sum(s => s.TotalSellPrice);
    [JsonIgnore] public int SellQuantity => SellDatas.Sum(s => s.Quantity);
    [JsonIgnore] public DateTimeOffset LatestSellDate => SellDatas.OrderBy(d => d.SellDate).First().SellDate;

    // The Total We Sold For Reduced By The 15% BLTP Tax Minus The Total We Bought The Items For
    [JsonIgnore] public int Profit => Mathf.FloorToInt((TotalSellPrice * .85f) - TotalBuyPrice);
    [JsonIgnore] public double ROI => Profit / (double)TotalBuyPrice * 100;

    public InvestmentData(CommerceTransactionHistory buyTransaction)
    {
        TransactionId = buyTransaction.Id;
        ItemId = buyTransaction.ItemId;
        PurchaseDate = buyTransaction.Purchased;
        IndividualPrice = buyTransaction.Price;
        Quantity = buyTransaction.Quantity;
        SellDatas = new List<SellData>();
    }

    [JsonConstructor]
    public InvestmentData() { }
}
