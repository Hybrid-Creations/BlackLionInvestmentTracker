using System;
using Gw2Sharp.WebApi.V2.Models;

namespace BLIT;

public partial class PendingSellData
{
    public long TransactionId;
    public DateTimeOffset CreatedDate;
    public int IndividualPrice;
    public int Quantity;

    public int TotalSellPrice => IndividualPrice * Quantity;

    public PendingSellData(CommerceTransactionCurrent sellTransaction)
    {
        TransactionId = sellTransaction.Id;
        CreatedDate = sellTransaction.Created;
        IndividualPrice = sellTransaction.Price;
        Quantity = sellTransaction.Quantity;
    }
}
