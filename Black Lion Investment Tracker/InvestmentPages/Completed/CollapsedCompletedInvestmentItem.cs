using System;
using System.Linq;
using BLIT.ConstantVariables;
using BLIT.Extensions;
using BLIT.Investments;
using Godot;

namespace BLIT.UI;

public sealed partial class CollapsedCompletedInvestmentItem : CollapsedInvestmentItem
{
    private CollapsedCompletedInvestment collapsedInvestment;

    public override double TotalBuyPrice => collapsedInvestment.TotalBuyPrice;
    public override double TotalSellPrice => collapsedInvestment.TotalSellPrice;
    public override double TotalProfit => collapsedInvestment.TotalProfit;
    public override DateTimeOffset LastActive => collapsedInvestment.LastActiveDate;

    public override void _Ready()
    {
        subInvestmentTitles.Hide();
    }

    public void Init(ItemData _item, CollapsedCompletedInvestment _collapsedInvestment)
    {
        if (IsQueuedForDeletion())
            return;

        collapsedInvestment = _collapsedInvestment;

        if (_collapsedInvestment.IndividualSellPriceDifferent == false)
            HideTreeToggle();

        itemProperties.GetNode<ItemIcon>("Icon").Init(_item.Icon, _collapsedInvestment.Quantity, false);
        itemProperties.GetNode<Label>("Name").Text = ItemName = _item.Name;
        itemProperties.GetNode<RichTextLabel>("BuyPrice").Text = _collapsedInvestment.GetBuyPriceStringFromInvestment(true, RichStringAlignment.RIGHT);
        itemProperties.GetNode<RichTextLabel>("SellPrice").Text = _collapsedInvestment.GetSellPriceStringFromInvestment(true, RichStringAlignment.RIGHT);
        itemProperties.GetNode<RichTextLabel>("Profit").Text = _collapsedInvestment.GetProfitStringFromInvestment(true, RichStringAlignment.RIGHT);
        itemProperties.GetNode<Label>("BuyDate").Text = _collapsedInvestment.LastActiveDate.ToTimeSinceString();
    }

    public void TreeButtonToggled(bool enabled)
    {
        if (enabled)
        {
            subInvestmentTitles.Show();
            toggleTreeButton.Icon = arrowDown;
            TitleBorder3.Show();

            foreach (var investment in collapsedInvestment.SubInvestments.OrderByDescending(si => si.LatestSellDate))
            {
                var instance = subInvestmentItemScene.Instantiate<CompletedInvestmentItem>();
                instance.Init(Cache.Items.GetItemData(investment.BuyData.ItemId), false, investment);
                subInvestmentsHolder.AddChildSafe(instance);
            }
        }
        else
        {
            subInvestmentTitles.Hide();
            toggleTreeButton.Icon = arrowRight;
            TitleBorder3.Hide();
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
                RightClickMenu.OpenMenu(
                    GetTree().Root,
                    mouse.GlobalPosition,
                    new[] { MarkNotAnInvestment },
                    (str) =>
                    {
                        if (str == MarkNotAnInvestment)
                        {
                            Main.Database.CollapsedCompletedInvestments.Remove(collapsedInvestment);

                            foreach (var investment in collapsedInvestment.SubInvestments)
                            {
                                Main.Database.CompletedInvestments.Remove(investment);
                                Main.Database.NotInvestments.Add(investment.BuyData.TransactionId);
                            }
                            Main.Database.Save();
                            QueueFree();
                        }
                    }
                );
            }
            else if (mouse.ButtonMask == MouseButtonMask.Left)
            {
                RightClickMenu.CloseInstance();
            }
        }
    }
}
