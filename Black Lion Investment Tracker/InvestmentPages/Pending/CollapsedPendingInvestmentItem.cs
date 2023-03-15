using System.Linq;
using BLIT.ConstantVariables;
using BLIT.Extensions;
using BLIT.Investments;
using Godot;

namespace BLIT.UI;

public sealed partial class CollapsedPendingInvestmentItem : CollapsedInvestmentItem
{
    private CollapsedPendingInvestment collapsedInvestment;
    int currentSellPrice;

    public override void _Ready()
    {
        subInvestmentTitles.Hide();
    }

    public void Init(ItemData _item, CollapsedPendingInvestment _collapsedInvestment, int _currentSellPrice)
    {
        if (IsQueuedForDeletion()) return;

        collapsedInvestment = _collapsedInvestment;
        currentSellPrice = _currentSellPrice;

        itemProperties.GetNode<TextureRect>("Icon").Texture = _item.Icon;
        itemProperties.GetNode<Label>("Icon/Quantity").Text = _collapsedInvestment.Quantity.ToString();
        itemProperties.GetNode<Label>("Name").Text = _item.Name;
        itemProperties.GetNode<RichTextLabel>("BuyPrice").Text = _collapsedInvestment.GetBuyPriceStringFromInvestment(true, RichStringAlignment.RIGHT);
        itemProperties.GetNode<RichTextLabel>("SellPrice").Text = _collapsedInvestment.GetSellPriceStringFromInvestment(true, RichStringAlignment.RIGHT);
        itemProperties.GetNode<RichTextLabel>("BreakEvenPrice").Text = GetCurrentListedPrice(_collapsedInvestment);
        itemProperties.GetNode<RichTextLabel>("Profit").Text = _collapsedInvestment.GetProfitStringFromInvestment(true, RichStringAlignment.RIGHT);
        itemProperties.GetNode<Label>("BuyDate").Text = _collapsedInvestment.OldestPurchaseDate.ToTimeSinceString();
    }

    private string GetCurrentListedPrice(CollapsedPendingInvestment investment)
    {
        var listedIsHigher = currentSellPrice < investment.LowestIndividualSellPrice;
        return $"{(listedIsHigher ? $"\n[right][color=gray]ea[/color] [color=#ff9200]{currentSellPrice.ToCurrencyString(RichImageType.PX32)}" : Constants.EmptyItemPropertyEntry)}";
    }

    public void TreeButtonToggled(bool enabled)
    {
        if (enabled)
        {
            subInvestmentTitles.Show();
            toggleTreeButton.Icon = arrowDown;

            foreach (var investment in collapsedInvestment.SubInvestments.OrderByDescending(si => si.BuyData.DatePurchased))
            {
                var instance = subInvestmentItemScene.Instantiate<PendingInvestmentItem>();
                instance.Init(Cache.Items.GetItemData(investment.BuyData.ItemId), investment, currentSellPrice);
                subInvestmentsHolder.AddChildSafe(instance);
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
                        Main.Database.CollapsedPendingInvestments.Remove(collapsedInvestment);

                        foreach (var investment in collapsedInvestment.SubInvestments)
                        {
                            Main.Database.PendingInvestments.Remove(investment);
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
