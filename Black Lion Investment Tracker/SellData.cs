using System;
using Gw2Sharp.WebApi.V2.Models;

namespace BLIT;

public class SellData
{
    public long TransactionId;
    public DateTimeOffset SellDate;
    public int Price;
    public int Quantity;

    public SellData(CommerceTransactionHistory sellTransaction)
    {
        TransactionId = sellTransaction.Id;
        SellDate = sellTransaction.Purchased;
        Price = sellTransaction.Price;
        Quantity = sellTransaction.Quantity;
    }
}