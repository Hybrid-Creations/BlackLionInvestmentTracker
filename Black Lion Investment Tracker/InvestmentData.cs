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
    public int IndividualPrice;
    public int Quantity;

    public int TotalBuyPrice => IndividualPrice * Quantity;
    public int TotalSellPrice => SellDatas.Sum(s => s.Price * s.Quantity);
    public int SellQuantity => SellDatas.Sum(s => s.Quantity);
    public DateTimeOffset LatestSellDate => SellDatas.OrderBy(d => d.SellDate).First().SellDate;

    // The Total We Sold For Reduced By The 15% BLTP Tax Minus The Total We Bought The Items For
    public int Profit => Mathf.FloorToInt((TotalSellPrice * .85f) - (IndividualPrice * Quantity));

    public double ROI => Profit / (double)TotalBuyPrice * 100;

    public List<SellData> SellDatas;

    public InvestmentData(CommerceTransactionHistory buyTransaction)
    {
        TransactionId = buyTransaction.Id;
        ItemId = buyTransaction.ItemId;
        PurchaseDate = buyTransaction.Purchased;
        IndividualPrice = buyTransaction.Price;
        Quantity = buyTransaction.Quantity;
        SellDatas = new List<SellData>();
    }
}

public class CollapsedInvestmentData
{
    public int ItemId;
    public int IndividualPrice;
    public int Quantity => SubInvestments.Sum(s => s.Quantity);
    public int TotalBuyPrice => SubInvestments.Sum(i => i.TotalBuyPrice);
    public int TotalSellPrice => SubInvestments.Sum(i => i.TotalSellPrice);
    // The sub investments already account for the BLTP 15% tax
    public int TotalProfit => SubInvestments.Sum(i => i.Profit);
    public DateTimeOffset LatestPurchaseDate => SubInvestments.OrderBy(i => i.PurchaseDate).First().PurchaseDate;
    public DateTimeOffset LatestSellDate => SubInvestments.SelectMany(i => i.SellDatas).OrderBy(d => d.SellDate).First().SellDate;

    public List<InvestmentData> SubInvestments;

    public CollapsedInvestmentData(InvestmentData firstInvestment)
    {
        ItemId = firstInvestment.ItemId;
        IndividualPrice = firstInvestment.IndividualPrice;
        SubInvestments = new List<InvestmentData>
        {
            firstInvestment
        };
    }
}
