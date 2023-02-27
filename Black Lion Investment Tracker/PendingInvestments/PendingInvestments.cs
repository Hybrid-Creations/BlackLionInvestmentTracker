using System.Linq;
using BLIT.Extensions;
using Godot;

namespace BLIT.UI;

public partial class PendingInvestments : VBoxContainer
{
    [Export]
    PackedScene collapsedPendingTransactionScene;
    [Export]
    VBoxContainer investmentHolder;
    [Export]
    Label loadingLabel;

    private void ClearList()
    {
        // Remove Old Investment Items From UI
        investmentHolder.ClearChildrenSafe();
        loadingLabel.Show();
    }

    public void ListInvestments()
    {
        ClearList();

        loadingLabel.Hide();

        string status = "Filling list of Pending Investments...";
        int index = 0;
        // Add New Investment Items To UI
        foreach (var investment in Main.Database.CollapsedPendingInvestments.OrderBy(ci => ci.OldestPurchaseDate))
        {
            try
            {
                var instance = collapsedPendingTransactionScene.Instantiate<CollapsedPendingTransactionItem>();
                instance.Init(Cache.Items.GetItemData(investment.ItemId), investment);
                investmentHolder.AddChildSafe(instance, 0);
            }
            catch (System.Exception e)
            {
                // Most likely a new item that Gw2Sharp doesn't understand so we'll just skip it
                GD.PushWarning($"Failed to retreive info on item {investment.ItemId}, most likely Gw2Sharp has not been updated yet to handle the item");
                GD.PrintErr(e);
            }
            AppStatusIndicator.ShowStatus($"{status} ({index}/{Main.Database.PendingInvestments.Count})");
            index++;
        }
        AppStatusIndicator.ShowStatus($"{status} ({Main.Database.PendingInvestments.Count}/{Main.Database.PendingInvestments.Count})");

        AppStatusIndicator.ClearStatus();
        Main.Database.Save();
    }
}
