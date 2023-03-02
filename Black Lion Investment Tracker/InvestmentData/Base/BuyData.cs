using System;
using System.Runtime.Serialization;
using Gw2Sharp.WebApi.V2.Models;

namespace BLIT.Investments;

[DataContract]
public class BuyData
{
    [DataMember] public long TransactionId { get; protected set; }
    [DataMember] public int ItemId { get; protected set; }
    [DataMember] public int Quantity { get; internal set; }
    [DataMember] public int IndividualBuyPrice { get; protected set; }
    [DataMember] public DateTimeOffset DatePurchased { get; protected set; }

    public int TotalBuyPrice => IndividualBuyPrice * Quantity;

    public BuyData(CommerceTransactionHistory buyTransaction)
    {
        TransactionId = buyTransaction.Id;
        ItemId = buyTransaction.ItemId;
        Quantity = buyTransaction.Quantity;
        IndividualBuyPrice = buyTransaction.Price;
        DatePurchased = buyTransaction.Purchased;
    }

    public BuyData() { }
}
