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
    }

    private string GetNeededPriceForAnyProfit(PotentialInvestment investment)
    {
        // If you are making profit, it looks good
        if (investment.TotalPotentialProfit > 0)
            return Constants.EmptyItemPropertyEntry;
        // We know we arent making profit so calculate what you would have to sell it at to break even
        else
        {
            var idealPrice = Mathf.CeilToInt(investment.BuyData.IndividualBuyPrice * Constants.MultiplyInverseTax);
            return DataExtentions.CombineTotalAndIndividual(investment.BuyData.Quantity * idealPrice, idealPrice).AlignRichString(RichStringAlignment.RIGHT);
        }
    }
}
