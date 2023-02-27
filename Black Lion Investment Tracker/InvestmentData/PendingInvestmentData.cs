using System;
using System.Collections.Generic;
using Godot;
using Gw2Sharp.WebApi.V2.Models;

namespace BLIT;

public class PendingInvestmentData
{
    public long TransactionId;
    public int ItemId;
    public DateTimeOffset PurchaseDate;
    public int IndividualPrice;
    public int Quantity;
    public Lazy<int> CurrentIndividualSellPrice;
    public List<PendingSellData> PostedSellDatas = new();

    public int TotalBuyPrice => IndividualPrice * Quantity;
    public int TotalPotentialSellPrice => CurrentIndividualSellPrice.Value * Quantity;

    // The Total We Sold For Reduced By The 15% BLTP Tax Minus The Total We Bought The Items For
    public int PotentialProfit => Mathf.FloorToInt((TotalPotentialSellPrice * Constants.MultiplyTax) - TotalBuyPrice);
    public double PotentialROI => PotentialProfit / (double)TotalBuyPrice * 100;

    public PendingInvestmentData(CommerceTransactionHistory buyTransaction, Lazy<int> currentSellPrice)
    {
        TransactionId = buyTransaction.Id;
        ItemId = buyTransaction.ItemId;
        PurchaseDate = buyTransaction.Purchased;
        IndividualPrice = buyTransaction.Price;
        Quantity = buyTransaction.Quantity;
        CurrentIndividualSellPrice = currentSellPrice;
    }
}
