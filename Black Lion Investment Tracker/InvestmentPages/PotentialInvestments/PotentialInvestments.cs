
using BLIT.Extensions;
using BLIT.Investments;

namespace BLIT.UI;

public partial class PotentialInvestments : InvestmentPageeee<CollapsedPotentialInvestment, PotentialInvestment, PotentialInvestmentData, BuyData, PotentialSellData, CollapsedPotentialInvestmentItem, PotentialInvestmentItem>
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

        string status = "Filling list of Potential Investments...";
        ListInvestmentDatas(Main.Database.CollapsedPotentialInvestments, status);

        AppStatusIndicator.ClearStatus();
        Main.Database.Save();
    }
}
