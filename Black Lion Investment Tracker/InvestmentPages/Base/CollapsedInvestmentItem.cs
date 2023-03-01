using System.Linq;
using BLIT.ConstantVariables;
using BLIT.Extensions;
using BLIT.Investments;
using Godot;

namespace BLIT.UI;

public abstract partial class CollapsedInvestmentItem<TCollapsedInvestment, TInvestment, TInvestmentData, TBuyData, TSellData, TInvestmentItemScene> : VBoxContainer
where TCollapsedInvestment : CollapsedInvestment<TInvestment, TInvestmentData, TBuyData, TSellData>
where TInvestment : Investment<TInvestmentData, TBuyData, TSellData>
where TInvestmentData : InvestmentData<TBuyData, TSellData>
where TBuyData : BuyData
where TSellData : SellData
where TInvestmentItemScene : InvestmentItem<TInvestment, TInvestmentData, TBuyData, TSellData>
{
    [ExportCategory("UI References")]
    [Export]
    protected HBoxContainer itemProperties;

    [ExportGroup("Toggle Button")]
    [Export]
    Button toggleTreeButton;
    [Export]
    Texture2D arrowRight;
    [Export]
    Texture2D arrowDown;

    [ExportCategory("Sub Investments")]
    [Export]
    PackedScene subInvestmentItemScene;
    [Export]
    VBoxContainer subInvestmentsHolder;
    [Export]
    HBoxContainer subInvestmentTitles;

    protected TCollapsedInvestment collapsedInvestment;

    public override void _Ready()
    {
        subInvestmentTitles.Hide();
    }

    public virtual void Init(ItemData _item, TCollapsedInvestment _collapsedInvestment)
    {
        collapsedInvestment = _collapsedInvestment;

        TextureRect texRectIcon = itemProperties.GetNode<TextureRect>("Icon");
        Label lblQuantity = itemProperties.GetNode<Label>("Icon/Quantity");
        Label lblName = itemProperties.GetNode<Label>("Name");
        RichTextLabel richLblBuyPrice = itemProperties.GetNode<RichTextLabel>("BuyPrice");
        RichTextLabel richLblSellPrice = itemProperties.GetNode<RichTextLabel>("SellPrice");
        RichTextLabel richLblProfit = itemProperties.GetNode<RichTextLabel>("Profit");
        Label lblBuyDate = itemProperties.GetNode<Label>("BuyDate");

        if (texRectIcon is not null)
            texRectIcon.Texture = _item.Icon;
        if (lblQuantity is not null)
            lblQuantity.Text = _collapsedInvestment.Quantity.ToString();
        if (lblName is not null)
            lblName.Text = _item.Name;
        if (richLblBuyPrice is not null)
            richLblBuyPrice.Text = _collapsedInvestment.GetBuyPriceStringFromInvestment<TCollapsedInvestment, TInvestment, TInvestmentData, TBuyData, TSellData>(true, RichStringAlignment.RIGHT);
        if (richLblSellPrice is not null)
            richLblSellPrice.Text = _collapsedInvestment.GetSellPriceStringFromInvestment<TCollapsedInvestment, TInvestment, TInvestmentData, TBuyData, TSellData>(true, RichStringAlignment.RIGHT);
        if (richLblProfit is not null)
            richLblProfit.Text = $"[right]{_collapsedInvestment.TotalProfit.ToCurrencyString(true)}[/right]";
        if (lblBuyDate is not null)
            lblBuyDate.Text = _collapsedInvestment.OldestPurchaseDate.ToTimeSinceString();
    }

    public void TreeButtonToggled(bool enabled)
    {
        if (enabled)
        {
            subInvestmentTitles.Show();
            toggleTreeButton.Icon = arrowDown;

            foreach (var investment in collapsedInvestment.SubInvestments.OrderBy(si => si.Data.BuyData.DatePurchased))
            {
                var instance = subInvestmentItemScene.Instantiate<TInvestmentItemScene>();
                instance.Init(Cache.Items.GetItemData(investment.Data.BuyData.ItemId), investment);
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
                        OnMarkedNotAnInvestment();

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

    protected abstract void OnMarkedNotAnInvestment();
}
