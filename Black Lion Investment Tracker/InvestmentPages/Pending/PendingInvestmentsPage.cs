using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BLIT.Extensions;
using BLIT.Investments;
using BLIT.Status;
using Godot;

namespace BLIT.UI;

public partial class PendingInvestmentsPage : InvestmentsPage
{
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        ClearList();
    }

    private void ClearList()
    {
        // Remove Old Investment Items From UI
        investmentHolder.ClearChildrenSafe();
        loadingLabel.Show();
    }

    public async Task ListInvestmentDatasAsync(List<CollapsedPendingInvestment> investmentDatas, string baseStatusMessage, CancellationToken cancelToken)
    {
        ClearList();

        loadingLabel.Hide();

        var prices = await Main.MyClient.WebApi.V2.Commerce.Prices.ManyAsync(investmentDatas.Select(i => i.ItemId).Distinct(), cancelToken);

        int index = 0;
        AppStatusManager.ShowStatus($"{nameof(PendingInvestmentsPage)}{nameof(ListInvestmentDatasAsync)}", $"{baseStatusMessage} ({index}/{investmentDatas.Count})");
        // Add New Investment Items To UI
        foreach (var investment in investmentDatas.OrderByDescending(ci => ci.OldestPurchaseDate))
        {
            try
            {
                var instance = collapsedInvestmentScene.Instantiate<CollapsedPendingInvestmentItem>();
                instance.Init(Cache.Items.GetItemData(investment.ItemId), investment, prices.First(l => investment.ItemId == l.Id).Sells.UnitPrice);

                if (cancelToken.IsCancellationRequested)
                    break;

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
                {
                    ProbablyRealException(ag);
                }
            }
            catch (Exception e)
            {
                ProbablyRealException(e);
            }
            AppStatusManager.ShowStatus($"{nameof(PendingInvestmentsPage)}{nameof(ListInvestmentDatasAsync)}", $"{baseStatusMessage} ({index}/{investmentDatas.Count})");
            index++;
        }
        AppStatusManager.ShowStatus($"{nameof(PendingInvestmentsPage)}{nameof(ListInvestmentDatasAsync)}", $"{baseStatusMessage} ({index}/{investmentDatas.Count})");

        if (cancelToken.IsCancellationRequested)
        {
            ClearList();
            AppStatusManager.ClearStatus($"{nameof(PendingInvestmentsPage)}{nameof(ListInvestmentDatasAsync)}");
            return;
        }

        AppStatusManager.ClearStatus($"{nameof(PendingInvestmentsPage)}{nameof(ListInvestmentDatasAsync)}");
    }


    private static void ProbablyRealException(Exception e)
    {
        GD.PushError(e);
        GD.PushWarning("Unexpected error from GW2Sharp, might be an API issue?");
        APIStatusIndicator.ShowStatus("Possible Issues With API, Some Requests Are Failing.");
    }
}
