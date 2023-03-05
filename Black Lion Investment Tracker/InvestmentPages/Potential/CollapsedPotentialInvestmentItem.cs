
using System.Linq;
using BLIT.ConstantVariables;
using BLIT.Extensions;
using BLIT.Investments;
using Godot;

namespace BLIT.UI;

public sealed partial class CollapsedPotentialInvestmentItem : CollapsedInvestmentItem
{
    private CollapsedPotentialInvestment collapsedInvestment;

    public override void _Ready()
    {
        subInvestmentTitles.Hide();
    }

    public void Init(ItemData _item, CollapsedPotentialInvestment _collapsedInvestment)
    {
        collapsedInvestment = _collapsedInvestment;

        itemProperties.GetNode<TextureRect>("Icon").Texture = _item.Icon;
        itemProperties.GetNode<Label>("Icon/Quantity").Text = _collapsedInvestment.Quantity.ToString();
        itemProperties.GetNode<Label>("Name").Text = _item.Name;
        itemProperties.GetNode<RichTextLabel>("BuyPrice").Text = _collapsedInvestment.GetBuyPriceStringFromInvestment(true, RichStringAlignment.RIGHT);
        itemProperties.GetNode<RichTextLabel>("SellPrice").Text = _collapsedInvestment.GetSellPriceStringFromInvestment(true, RichStringAlignment.RIGHT);
        itemProperties.GetNode<RichTextLabel>("BreakEvenPrice").Text = GetNeededPriceForAnyProfit(_collapsedInvestment);
        itemProperties.GetNode<RichTextLabel>("Profit").Text = _collapsedInvestment.GetProfitStringFromInvestment(true, RichStringAlignment.RIGHT);
        itemProperties.GetNode<Label>("BuyDate").Text = _collapsedInvestment.OldestPurchaseDate.ToTimeSinceString();
    }

    private string GetNeededPriceForAnyProfit(CollapsedPotentialInvestment collapsedInvestment)
    {
        // If you are making profit, it looks good
        if (collapsedInvestment.TotalPotentialProfit > 0)
            return Constants.EmptyItemPropertyEntry;
        // We know we arent making profit so calculate what you would have to sell it at to break even
        else
        {
            var idealPrice = Mathf.CeilToInt(collapsedInvestment.IndividualBuyPrice * Constants.MultiplyInverseTax);
            return DataExtentions.CombineTotalAndIndividual(collapsedInvestment.Quantity * idealPrice, idealPrice).ColorRichString("#ffd500").AlignRichString(RichStringAlignment.RIGHT);
        }
    }

    public void TreeButtonToggled(bool enabled)
    {
        if (enabled)
        {
            subInvestmentTitles.Show();
            toggleTreeButton.Icon = arrowDown;

            foreach (var investment in collapsedInvestment.SubInvestments.OrderBy(si => si.BuyData.DatePurchased))
            {
                var instance = subInvestmentItemScene.Instantiate<PotentialInvestmentItem>();
                instance.Init(Cache.Items.GetItemData(investment.BuyData.ItemId), investment);
                subInvestmentsHolder.AddChild(instance, 0);
            }
        }
        else
        {
            toggleTreeButton.Icon = arrowRight;
            subInvestmentTitles.Hide();
            subInvestmentsHolder.ClearChildren();
        }
    }

    public void GUIInput(InputEvent @event)
    {
        const string MarkNotAnInvestment = "Mark Not An Investment";

        if (@event is InputEventMouseButton mouse)
        {
            if (mouse.ButtonMask == MouseButtonMask.Right)
            {
                RightClickMenu.OpenMenu(GetTree().Root, mouse.GlobalPosition, new[] { MarkNotAnInvestment }, (str) =>
                {
                    if (str == MarkNotAnInvestment)
                    {
                        Main.Database.CollapsedPotentialInvestments.Remove(collapsedInvestment);

                        foreach (var investment in collapsedInvestment.SubInvestments)
                        {
                            Main.Database.PotentialInvestments.Remove(investment);
                            Main.Database.NotInvestments.Add(investment.BuyData.TransactionId);
                        }
                        Main.Database.Save();
                        QueueFree();
                    }
                });
            }
            else if (mouse.ButtonMask == MouseButtonMask.Left)
            {
                RightClickMenu.CloseInstance();
            }
        }
    }
}
