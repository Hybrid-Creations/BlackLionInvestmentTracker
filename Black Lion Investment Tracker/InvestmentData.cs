using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Gw2Sharp.WebApi.V2.Models;

namespace BLIT;

public class InvestmentData
{
    public long TransactionId;
    public int ItemId;
    public DateTimeOffset PurchaseDate;
    public int Price;
    public int Quantity;

    public int TotalBuyPrice => Price * Quantity;
    public int TotalSellPrice => SellDatas.Sum(s => s.Price * s.Quantity);
    public int SellQuantity => SellDatas.Sum(s => s.Quantity);
    public DateTimeOffset LatestSellDate => SellDatas.OrderBy(d => d.SellDate).First().SellDate;

    // The Total We Sold For Reduced By The 15% BLTP Tax Minus The Total We Bought The Items For
    public int Profit => Mathf.FloorToInt((TotalSellPrice * .85f) - (Price * Quantity));

    public List<SellData> SellDatas;

    public InvestmentData(CommerceTransactionHistory buyTransaction)
    {
        TransactionId = buyTransaction.Id;
        ItemId = buyTransaction.ItemId;
        PurchaseDate = buyTransaction.Purchased;
        Price = buyTransaction.Price;
        Quantity = buyTransaction.Quantity;
        SellDatas = new List<SellData>();
    }

}
