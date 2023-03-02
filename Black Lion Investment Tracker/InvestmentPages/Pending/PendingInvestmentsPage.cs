using System;
using System.Collections.Generic;
using System.Linq;
using BLIT.Extensions;
using BLIT.Investments;
using Godot;
using Gw2Sharp.WebApi.Exceptions;

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

    public void ListInvestmentDatas(List<CollapsedPendingInvestment> investmentDatas, string baseStatusMessage)
    {
        ClearList();

        loadingLabel.Hide();

        int index = 0;
        AppStatusIndicator.ShowStatus($"{baseStatusMessage} ({index}/{investmentDatas.Count})");
        // Add New Investment Items To UI
        foreach (var investment in investmentDatas.OrderBy(ci => ci.OldestPurchaseDate))
        {
            try
            {
                var instance = collapsedInvestmentScene.Instantiate<CollapsedPendingInvestmentItem>();
                instance.Init(Cache.Items.GetItemData(investment.ItemId), investment);
                investmentHolder.AddChildSafe(instance, 0);
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
            AppStatusIndicator.ShowStatus($"{baseStatusMessage} ({index}/{investmentDatas.Count})");
            index++;
        }
        AppStatusIndicator.ShowStatus($"{baseStatusMessage} ({investmentDatas.Count}/{investmentDatas.Count})");

        AppStatusIndicator.ClearStatus();
    }


    private static void ProbablyRealException(Exception e)
    {
        GD.PushError(e);
        GD.PushWarning("Unexpected error from GW2Sharp, might be an API issue?");
        APIStatusIndicator.ShowStatus("Possible Issues With API, Some Requests Are Failing.");
    }
}
