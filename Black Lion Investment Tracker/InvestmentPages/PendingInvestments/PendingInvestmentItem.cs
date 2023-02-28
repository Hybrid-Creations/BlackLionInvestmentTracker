using BLIT.ConstantVariables;
using BLIT.Extensions;
using BLIT.Investments;
using Godot;

namespace BLIT.UI;

public partial class PendingInvestmentItem : VBoxContainer
{
    internal void Init(ItemData item, PendingInvestment pendingInvestment)
    {
        GetNode<TextureRect>("ItemProperties/Icon").Texture = item.Icon;
        GetNode<Label>("ItemProperties/Icon/Quantity").Text = pendingInvestment.Quantity.ToString();
        GetNode<Label>("ItemProperties/Name").Text = $" {item.Name}";
        GetNode<RichTextLabel>("ItemProperties/InvestmentPrice").Text = pendingInvestment.GetBuyPriceStringFromInvestment(true, RichStringAlignment.RIGHT);
        GetNode<RichTextLabel>("ItemProperties/CurrentSellPrice").Text = pendingInvestment.GetSellPriceStringFromInvestment(true, RichStringAlignment.RIGHT);
        GetNode<RichTextLabel>("ItemProperties/BreakEvenSellPrice").Text = GetNeededPriceForAnyProfit(pendingInvestment);
        GetNode<RichTextLabel>("ItemProperties/CurrentProfit").Text = $"[right]{pendingInvestment.PotentialProfit.ToCurrencyString(true)}[/right]";
        GetNode<Label>("ItemProperties/InvestDate").Text = pendingInvestment.Data.BuyData.DatePurchased.ToTimeSinceString();
    }

    private static string GetNeededPriceForAnyProfit(PendingInvestment pendingInvestment)
    {
        // If you are making profit, it looks good
        if (pendingInvestment.PotentialProfit > 0)
            return "[right]-----[/right]";
        // We know we arent making profit so calculate what you would have to sell it at to break even
        else
        {
            var idealPrice = Mathf.CeilToInt(pendingInvestment.IndividualSellPrice * Constants.MultiplyInverseTax);
            return $"[right]{(pendingInvestment.Quantity * idealPrice).ToCurrencyString(true)}\n [color=gray]each[/color] {idealPrice.ToCurrencyString(true)}[/right]";
        }
    }
}
