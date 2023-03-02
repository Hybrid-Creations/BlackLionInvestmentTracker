using System;
using System.Collections.Generic;
using System.Linq;
using BLIT.Extensions;
using BLIT.Investments;
using Godot;
using Gw2Sharp.WebApi.Exceptions;

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
        totals.GetNode<RichTextLabel>("Invested").Text = $"Invested:  {Main.Database.TotalInvested.ToCurrencyString(true)}";
        totals.GetNode<RichTextLabel>("Return").Text = $"Return:  {Main.Database.TotalReturn.ToCurrencyString(true)}";
        totals.GetNode<RichTextLabel>("Profit").Text = $"Profit:  {Main.Database.TotalProfit.ToCurrencyString(true)}";
        totals.GetNode<Label>("ROI").Text = $"ROI:  {Main.Database.ROI:00}%";
    }

    private void ClearList()
    {
        // Remove Old Investment Items From UI
        investmentHolder.ClearChildrenSafe();
        loadingLabel.Show();
    }

    public void ListInvestmentDatas(List<CollapsedCompletedInvestment> investmentDatas, string baseStatusMessage)
    {
        ClearTotals();
        ClearList();

        loadingLabel.Hide();

        int index = 0;
        AppStatusIndicator.ShowStatus($"{baseStatusMessage} ({index}/{investmentDatas.Count})");
        // Add New Investment Items To UI
        foreach (var investment in investmentDatas.OrderBy(ci => ci.OldestPurchaseDate))
        {
            try
            {
                var instance = collapsedInvestmentScene.Instantiate<CollapsedCompletedInvestmentItem>();
                instance.Init(Cache.Items.GetItemData(investment.ItemId), investment);
                investmentHolder.AddChildSafe(instance, 0);
            }
            catch (NotFoundException)
            {
                // Most likely a new item that Gw2Sharp doesn't understand so we'll just skip it
                GD.PushWarning($"Failed to retreive info on item {investment.ItemId}, most likely Gw2Sharp has not been updated yet to handle the item");
            }
            catch (Exception e)
            {
                GD.PushError(e);
                GD.PushWarning("Unexpected error from GW2Sharp, might be an API issue?");
                APIStatusIndicator.ShowStatus("Possible Issues With API, Some Requests Are Failing.");
            }
            AppStatusIndicator.ShowStatus($"{baseStatusMessage} ({index}/{investmentDatas.Count})");
            index++;
        }
        AppStatusIndicator.ShowStatus($"{baseStatusMessage} ({investmentDatas.Count}/{investmentDatas.Count})");

        // Calculate Profit
        var totalInvested = Main.Database.TotalInvested;
        var totalReturn = Main.Database.TotalReturn;
        var totalProfit = Main.Database.TotalProfit;
        GD.Print($"Total Invested: {totalInvested.ToCurrencyString(false)}, Total Return: {totalReturn.ToCurrencyString(false)},  Total Profit With Tax Removed: {totalProfit.ToCurrencyString(false)}, ROI: {Main.Database.ROI}");
        SetTotals();

        AppStatusIndicator.ClearStatus();
    }
}