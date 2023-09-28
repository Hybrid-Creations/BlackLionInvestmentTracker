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
    private const string StatusKey = $"{nameof(PotentialInvestmentsPage)}{nameof(ListInvestmentDatas)}";

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        ClearList();
    }

    private void ClearList()
    {
        // Remove Old Investment Items From UI
        investmentHolder.ClearChildren();
        loadingLabel.Show();
    }

    public void ListInvestmentDatas(List<CollapsedPotentialInvestment> investmentDatas, string baseStatusMessage)
    {
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
                    var instance = collapsedInvestmentScene.Instantiate<CollapsedPotentialInvestmentItem>();
                    instance.Init(Cache.Items.GetItemData(investment.ItemId), investment, prices.First(p => p.Id == investment.ItemId).Sells.UnitPrice);

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
                index++;
            }
          
            AppStatusManager.ClearStatus(StatusKey);
        });
    }
}
