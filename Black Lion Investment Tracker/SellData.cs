using System;
using Gw2Sharp.WebApi.V2.Models;
using Newtonsoft.Json;

namespace BLIT;

public partial class SellData
{
    public long TransactionId;
    public DateTimeOffset SellDate;
    public int IndividualPrice;
    public int Quantity;

    [JsonIgnore] public int TotalSellPrice => IndividualPrice * Quantity;

    public SellData(CommerceTransactionHistory sellTransaction)
    {
        TransactionId = sellTransaction.Id;
        SellDate = sellTransaction.Purchased;
        IndividualPrice = sellTransaction.Price;
        Quantity = sellTransaction.Quantity;
    }

    [JsonConstructor]
    public SellData() { }
}
