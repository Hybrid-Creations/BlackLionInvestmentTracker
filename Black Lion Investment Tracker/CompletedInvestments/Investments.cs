using System;
using System.Linq;
using BLIT.Extensions;
using Godot;

namespace BLIT.UI;

public partial class Investments : VBoxContainer
{
    [Export]
    PackedScene collapsedTransactionScene;
    [Export]
    VBoxContainer investmentHolder;
    [Export]
    HBoxContainer totals;
    [Export]
    Label loadingLabel;

    static Investments Instance;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        Instance = this;

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

    public void ListInvestments()
    {
        ClearTotals();
        ClearList();

        loadingLabel.Hide();

        string status = "Filling list of Investments...";
        int index = 0;
        // Add New Investment Items To UI
        foreach (var investment in Main.Database.CollapsedInvestments.OrderBy(ci => ci.OldestPurchaseDate))
        {
            try
            {
                var instance = collapsedTransactionScene.Instantiate<CollapsedTransactionItem>();
                instance.Init(Cache.Items.GetItem(investment.ItemId).Name, Cache.Icons.GetIcon(investment.ItemId), investment);
                investmentHolder.AddChildSafe(instance, 0);
            }
            catch (Exception e)
            {
                // Most likely a new item that Gw2Sharp doesn't understand so we'll just skip it
                GD.PushWarning($"Failed to retreive info on item {investment.ItemId}, most likely Gw2Sharp has not been updated yet to handle the item");
                GD.PrintErr(e);
            }
            AppStatusIndicator.ShowStatus($"{status} ({index}/{Main.Database.CollapsedInvestments.Count})");
            index++;
        }
        AppStatusIndicator.ShowStatus($"{status} ({Main.Database.CollapsedInvestments.Count}/{Main.Database.CollapsedInvestments.Count})");

        // Calculate Profit
        var totalInvested = Main.Database.TotalInvested;
        var totalReturn = Main.Database.TotalReturn;
        var totalProfit = Main.Database.TotalProfit;
        GD.Print($"Total Invested: {totalInvested.ToCurrencyString(false)}, Total Return: {totalReturn.ToCurrencyString(false)},  Total Profit With Tax Removed: {totalProfit.ToCurrencyString(false)}, ROI: {Main.Database.ROI}");
        SetTotals();

        AppStatusIndicator.ClearStatus();
        Main.Database.Save();
    }
}
