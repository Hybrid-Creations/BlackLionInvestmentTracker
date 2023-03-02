using System;
using Gw2Sharp.WebApi.V2.Models;

namespace BLIT.Investments;

public class SellData
{
    public virtual long TransactionId { get; protected set; }
    public virtual int ItemId { get; protected set; }
    public virtual int Quantity { get; internal set; }
    public virtual int IndividualSellPrice { get; protected set; }
    internal virtual DateTimeOffset Date { get; set; }

    protected SellData() { }

    public SellData(CommerceTransactionHistory sellTransaction)
    {
        if (sellTransaction is null) return;

        TransactionId = sellTransaction.Id;
        ItemId = sellTransaction.ItemId;
        Date = sellTransaction.Created;
        IndividualSellPrice = sellTransaction.Price;
        Quantity = sellTransaction.Quantity;
    }

    public SellData(CommerceTransactionCurrent currentSellTransaction)
    {
        TransactionId = currentSellTransaction.Id;
        ItemId = currentSellTransaction.ItemId;
        Date = currentSellTransaction.Created;
        IndividualSellPrice = currentSellTransaction.Price;
        Quantity = currentSellTransaction.Quantity;
    }
}
