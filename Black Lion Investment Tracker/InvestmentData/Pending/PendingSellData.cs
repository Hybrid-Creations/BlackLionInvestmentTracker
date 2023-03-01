using System;
using Gw2Sharp.WebApi.V2.Models;

namespace BLIT.Investments;

public partial class PendingSellData : SellData
{
    public DateTimeOffset CreatedDate { get => Date; }

    public PendingSellData(CommerceTransactionCurrent currentSellTransaction) : base()
    {
        TransactionId = currentSellTransaction.Id;
        ItemId = currentSellTransaction.ItemId;
        Date = currentSellTransaction.Created;
        IndividualSellPrice = currentSellTransaction.Price;
        Quantity = currentSellTransaction.Quantity;
    }
}
