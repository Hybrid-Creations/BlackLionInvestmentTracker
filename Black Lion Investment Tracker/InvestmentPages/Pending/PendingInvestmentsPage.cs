using System;
using System.Collections.Generic;
using System.Linq;
using BLIT.Tools;
using BLIT.Extensions;
using BLIT.Investments;
using BLIT.Status;
using Godot;

namespace BLIT.UI;

public partial class PendingInvestmentsPage : InvestmentsPage
{
    private const string StatusKey = $"{nameof(PendingInvestmentsPage)}{nameof(ListInvestmentDatas)}";

    public void ListInvestmentDatas(List<CollapsedPendingInvestment> investmentDatas, string baseStatusMessage)
    {
        if (investmentDatas.Count <= 0)
            return;

        var prices = Main.MyClient.WebApi.V2.Commerce.Prices.ManyAsync(investmentDatas.Select(i => i.ItemId).Distinct()).Result;

        ThreadsHelper.CallOnMainThread(() =>
        {
            ClearList();
            loadingLabel.Hide();

            int index = 0;
            AppStatusManager.ShowStatus(StatusKey, $"{baseStatusMessage}");
            // Add New Investment Items To UI
            foreach (var investment in investmentDatas.OrderByDescending(ci => ci.OldestPurchaseDate))
            {
                try
                {
                    var instance = collapsedInvestmentScene.Instantiate<CollapsedPendingInvestmentItem>();
                    instance.Init(Cache.Items.GetItemData(investment.ItemId), investment, prices.First(l => investment.ItemId == l.Id).Sells.UnitPrice);

                    investmentHolder.AddChildSafe(instance);
                }
                catch (AggregateException ag)
                {
                    if (ag.ToString().Contains("Unsupported type") && ag.ToString().Contains("GW2Sharp"))
                    {
                        // Most likely a new item that Gw2Sharp doesn't understand so we'll just skip it
                        GD.PushWarning($"Failed to retreive info on item {investment.ItemId}, most likely Gw2Sharp has not been updated yet to handle the item");
                    }
                    else
                        ProbablyRealException(ag);
                }
                catch (Exception e)
                {
                    ProbablyRealException(e);
                }
                index++;
            }
            Direction = SortingDirection.Descending;
            ResetAllSortingArrows();
            lastActiveSortingArrow.Show();
            lastActiveSortingArrow.FlipV = Direction == SortingDirection.Ascending;

            AppStatusManager.ClearStatus(StatusKey);
        });
    }
}
