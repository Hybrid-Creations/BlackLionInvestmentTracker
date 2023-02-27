using BLIT.Extensions;
using Godot;

namespace BLIT.UI;

public partial class PendingInvestmentItem : VBoxContainer
{
    internal void Init(string itemName, ImageTexture icon, PendingInvestmentData pendingInvestment)
    {
        GetNode<TextureRect>("ItemProperties/Icon").Texture = icon;
        GetNode<Label>("ItemProperties/Icon/Quantity").Text = pendingInvestment.Quantity.ToString();
        GetNode<Label>("ItemProperties/Name").Text = $" {itemName}";
        GetNode<RichTextLabel>("ItemProperties/InvestmentPrice").Text = GetInvestmentPrice(pendingInvestment);
        GetNode<RichTextLabel>("ItemProperties/CurrentSellPrice").Text = GetCurrentSellPrice(pendingInvestment);
        GetNode<RichTextLabel>("ItemProperties/BreakEvenSellPrice").Text = GetNeededPriceForAnyProfit(pendingInvestment);
        GetNode<RichTextLabel>("ItemProperties/CurrentProfit").Text = $"[right]{pendingInvestment.PotentialProfit.ToCurrencyString(true)}[/right]";
    }

    private static string GetInvestmentPrice(PendingInvestmentData pendingInvestment)
    {
        return $"[right]{pendingInvestment.TotalBuyPrice.ToCurrencyString(true)}\n [color=gray]each[/color][ {pendingInvestment.IndividualPrice.ToCurrencyString(true)}/right]";
    }

    private static string GetCurrentSellPrice(PendingInvestmentData pendingInvestment)
    {
        return $"[right]{pendingInvestment.TotalPotentialSellPrice.ToCurrencyString(true)}\n [color=gray]each[/color] {pendingInvestment.CurrentIndividualSellPrice.Value.ToCurrencyString(true)}[/right]";
    }

    private static string GetNeededPriceForAnyProfit(PendingInvestmentData pendingInvestment)
    {
        // If you are making profit, it looks good
        if (pendingInvestment.PotentialProfit > 0)
            return "[center]-----[/center]";
        // We know we arent making profit so calculate what you would have to sell it at to break even
        else
        {
            var idealPrice = Mathf.CeilToInt(pendingInvestment.IndividualPrice * Constants.MultiplyInverseTax);
            return $"[right]{(pendingInvestment.Quantity * idealPrice).ToCurrencyString(true)}\n [color=gray]each[/color] {idealPrice.ToCurrencyString(true)}[/right]";
        }
    }
}
