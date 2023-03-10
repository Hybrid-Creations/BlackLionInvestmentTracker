using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BLIT.ConstantVariables;
using BLIT.Extensions;
using BLIT.Investments;
using Godot;

namespace BLIT.UI;

public partial class CompletedInvestmentsPage : InvestmentsPage
{
    [Export]
    HBoxContainer totals;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        ClearTotals();
        ClearList();
    }

    private void ClearTotals()
    {
        totals.GetNode<RichTextLabel>("Invested").Text = $"Invested:   0";
        totals.GetNode<RichTextLabel>("Return").Text = $"Return:   0";
        totals.GetNode<RichTextLabel>("Profit").Text = $"Profit:   0";
        totals.GetNode<Label>("ROI").Text = $"ROI:   0%";
    }

    private void SetTotals()
    {
        totals.GetNode<RichTextLabel>("Invested").Text = $"Invested:   {Main.Database.TotalInvested.ToCurrencyString(RichImageType.PX32, 24)}";
        totals.GetNode<RichTextLabel>("Return").Text = $"Return:   {Main.Database.TotalReturn.ToCurrencyString(RichImageType.PX32, 24)}";
        totals.GetNode<RichTextLabel>("Profit").Text = $"Profit:   {Main.Database.TotalProfit.ToCurrencyString(RichImageType.PX32, 24)}";
        totals.GetNode<Label>("ROI").Text = $"ROI:  {Main.Database.ROI:00}%";
    }

    private void ClearList()
    {
        // Remove Old Investment Items From UI
        investmentHolder.ClearChildrenSafe();
        loadingLabel.Show();
    }

    public Task ListInvestmentDatasAsync(List<CollapsedCompletedInvestment> investmentDatas, string baseStatusMessage, CancellationToken cancelToken)
    {
        return Task.Run(() =>
        {
            ClearTotals();
            ClearList();

            loadingLabel.Hide();

            int index = 0;
            AppStatusIndicator.ShowStatus($"{baseStatusMessage} ({index}/{investmentDatas.Count})");
            // Add New Investment Items To UI
            foreach (var investment in investmentDatas.OrderByDescending(ci => ci.OldestPurchaseDate))
            {
                try
                {
                    var instance = collapsedInvestmentScene.Instantiate<CollapsedCompletedInvestmentItem>();
                    instance.Init(Cache.Items.GetItemData(investment.ItemId), investment);

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
                AppStatusIndicator.ShowStatus($"{baseStatusMessage} ({index}/{investmentDatas.Count})");
                index++;
            }
            AppStatusIndicator.ShowStatus($"{baseStatusMessage} ({investmentDatas.Count}/{investmentDatas.Count})");

            // Calculate Profit
            var totalInvested = Main.Database.TotalInvested;
            var totalReturn = Main.Database.TotalReturn;
            var totalProfit = Main.Database.TotalProfit;
            GD.Print($"Total Invested: {totalInvested.ToCurrencyString(RichImageType.NONE)}, Total Return: {totalReturn.ToCurrencyString(RichImageType.NONE)},  Total Profit With Tax Removed: {totalProfit.ToCurrencyString(RichImageType.NONE)}, ROI: {Main.Database.ROI}");

            if (cancelToken.IsCancellationRequested)
            {
                ClearTotals();
                ClearList();
                AppStatusIndicator.ClearStatus();
                return;
            }

            SetTotals();

            AppStatusIndicator.ClearStatus();
        }, cancelToken);
    }

    private static void ProbablyRealException(Exception e)
    {
        GD.PushError(e);
        GD.PushWarning("Unexpected error from GW2Sharp, might be an API issue?");
        APIStatusIndicator.ShowStatus("Possible Issues With API, Some Requests Are Failing.");
    }
}
