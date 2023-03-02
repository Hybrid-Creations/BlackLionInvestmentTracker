using System;
using BLIT.ConstantVariables;
using BLIT.Extensions;
using BLIT.Investments;
using Godot;

namespace BLIT.UI;

public partial class PotentialInvestmentItem : InvestmentItem
{
    public virtual void Init(ItemData item, PotentialInvestment investment)
    {
        itemProperties.GetNode<TextureRect>("Icon").Texture = item.Icon;
        itemProperties.GetNode<Label>("Icon/Quantity").Text = investment.BuyData.Quantity.ToString();
        itemProperties.GetNode<Label>("Name").Text = item.Name;
        itemProperties.GetNode<RichTextLabel>("BuyPrice").Text = investment.GetBuyPriceStringFromInvestment(true, RichStringAlignment.RIGHT);
        itemProperties.GetNode<RichTextLabel>("SellPrice").Text = investment.GetSellPriceStringFromInvestment(true, RichStringAlignment.RIGHT);
        itemProperties.GetNode<RichTextLabel>("BreakEvenPrice").Text = GetNeededPriceForAnyProfit(investment);
        itemProperties.GetNode<RichTextLabel>("Profit").Text = investment.GetProfitStringFromInvestment(true, RichStringAlignment.RIGHT);
        itemProperties.GetNode<Label>("BuyDate").Text = investment.BuyData.DatePurchased.ToTimeSinceString();
        // itemProperties.GetNode<Label>("SellDate").Text = investment.LatestSellDate.ToTimeSinceString();
    }

    private string GetNeededPriceForAnyProfit(PotentialInvestment PotentialInvestment)
    {
        // If you are making profit, it looks good
        if (PotentialInvestment.TotalPotentialProfit > 0)
            return "[center]-----[/center]";
        // We know we arent making profit so calculate what you would have to sell it at to break even
        else
        {
            var idealPrice = Mathf.CeilToInt(PotentialInvestment.IndividualPotentialSellPrice * Constants.MultiplyInverseTax);
            return $"[right]{(PotentialInvestment.BuyData.Quantity * idealPrice).ToCurrencyString(true)}\n [color=gray]each[/color] {idealPrice.ToCurrencyString(true)}[/right]";
        }
    }
}
