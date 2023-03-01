using System;
using Gw2Sharp.WebApi.V2.Models;

namespace BLIT.Investments;

public class BuyData
{
    public long TransactionId { get; protected set; }
    public int ItemId { get; protected set; }
    public int Quantity { get; internal set; }
    public int IndividualBuyPrice { get; protected set; }
    public DateTimeOffset DatePurchased { get; protected set; }

    public BuyData() { }
    public BuyData(CommerceTransactionHistory buyTransaction)
    {
        TransactionId = buyTransaction.Id;
        ItemId = buyTransaction.ItemId;
        Quantity = buyTransaction.Quantity;
        IndividualBuyPrice = buyTransaction.Price;
        DatePurchased = buyTransaction.Purchased;
    }
}
