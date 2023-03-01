using BLIT.Extensions;
using BLIT.Investments;

namespace BLIT.UI;

public partial class PendingInvestments : InvestmentPageeee<CollapsedPendingInvestment, PendingInvestment, PendingInvestmentData, BuyData, PendingSellData, CollapsedPendingInvestmentItem, PendingInvestmentItem>
{
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
        ListInvestmentDatas(Main.Database.CollapsedPendingInvestments, status);

        AppStatusIndicator.ClearStatus();
        Main.Database.Save();
    }
}
