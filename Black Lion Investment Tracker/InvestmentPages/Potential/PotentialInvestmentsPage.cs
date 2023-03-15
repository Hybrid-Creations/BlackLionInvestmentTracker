using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BLIT.Extensions;
using BLIT.Investments;
using BLIT.Status;
using Godot;
using Gw2Sharp.WebApi.Exceptions;

namespace BLIT.UI;

public partial class PotentialInvestmentsPage : InvestmentsPage
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

    public async Task ListInvestmentDatasAsync(List<CollapsedPotentialInvestment> investmentDatas, string baseStatusMessage, CancellationToken cancelToken)
    {
        ClearList();

        loadingLabel.Hide();

        var prices = await Main.MyClient.WebApi.V2.Commerce.Prices.ManyAsync(investmentDatas.Select(i => i.ItemId).Distinct(), cancelToken);

        int index = 0;
        AppStatusManager.ShowStatus($"{nameof(PotentialInvestmentsPage)}{nameof(ListInvestmentDatasAsync)}", $"{baseStatusMessage} ({index}/{investmentDatas.Count})");
        // Add New Investment Items To UI
        foreach (var investment in investmentDatas.OrderByDescending(ci => ci.OldestPurchaseDate))
        {
            try
            {
                var instance = collapsedInvestmentScene.Instantiate<CollapsedPotentialInvestmentItem>();
                instance.Init(Cache.Items.GetItemData(investment.ItemId), investment, prices.First(p => p.Id == investment.ItemId).Sells.UnitPrice);

                if (cancelToken.IsCancellationRequested)
                    break;

                investmentHolder.AddChildSafe(instance);
            }
            catch (AggregateException ag)
            {
                if (ag.ToString().Contains("Unsupported type"))
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
            AppStatusManager.ShowStatus($"{nameof(PotentialInvestmentsPage)}{nameof(ListInvestmentDatasAsync)}", $"{baseStatusMessage} ({index}/{investmentDatas.Count})");
            index++;
        }
        AppStatusManager.ShowStatus($"{nameof(PotentialInvestmentsPage)}{nameof(ListInvestmentDatasAsync)}", $"{baseStatusMessage} ({index}/{investmentDatas.Count})");

        if (cancelToken.IsCancellationRequested)
        {
            ClearList();
            AppStatusManager.ClearStatus($"{nameof(PotentialInvestmentsPage)}{nameof(ListInvestmentDatasAsync)}");
            return;
        }

        AppStatusManager.ClearStatus($"{nameof(PotentialInvestmentsPage)}{nameof(ListInvestmentDatasAsync)}");
    }

    private static void ProbablyRealException(Exception e)
    {
        GD.PushError(e.ToString());
        GD.PushWarning("Unexpected error from GW2Sharp, might be an API issue?");
        APIStatusIndicator.ShowStatus("Possible Issues With API, Some Requests Are Failing.");
    }
}
