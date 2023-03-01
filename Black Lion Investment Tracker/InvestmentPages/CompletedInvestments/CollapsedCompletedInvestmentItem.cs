using BLIT.Investments;

namespace BLIT.UI;

public sealed partial class CollapsedCompletedInvestmentItem : CollapsedInvestmentItem<CollapsedCompletedInvestment, CompletedInvestment, CompletedInvestmentData, BuyData, SellData, CompletedInvestmentItem>
{
    protected override void OnMarkedNotAnInvestment()
    {
        Main.Database.CollapsedCompletedInvestments.Remove(collapsedInvestment);

        foreach (var investment in collapsedInvestment.SubInvestments)
        {
            Main.Database.CompletedInvestments.Remove(investment);
            Main.Database.NotInvestments.Add(investment.Data.BuyData.TransactionId);
        }
    }
}
