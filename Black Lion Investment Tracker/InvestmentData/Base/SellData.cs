using System;
using System.Runtime.Serialization;
using Gw2Sharp.WebApi.V2.Models;

namespace BLIT.Investments;

[DataContract]
public class SellData
{
    [DataMember] public long TransactionId { get; protected set; }
    [DataMember] public int ItemId { get; protected set; }
    [DataMember] public int Quantity { get; internal set; }
    [DataMember] public int IndividualSellPrice { get; protected set; }
    [DataMember] internal DateTimeOffset Date { get; set; }

    public int TotalSellPrice => IndividualSellPrice * Quantity;

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
