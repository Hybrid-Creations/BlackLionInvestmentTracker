using BLIT.Extensions;
using BLIT.Investments;
using Godot;

namespace BLIT.UI;

public partial class CompletedInvestments : InvestmentPage<CollapsedCompletedInvestment, CompletedInvestment, CompletedInvestmentData, BuyData, SellData>
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

    public void ListInvestments()
    {
        ClearTotals();
        ClearList();

        loadingLabel.Hide();

        string status = "Filling list of Investments...";
        ListInvestmentDatas(Main.Database.CollapsedCompletedInvestments, status);

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
