using System.Collections.Generic;
using System.Linq;
using BLIT.ConstantVariables;
using BLIT.Extensions;
using Godot;
using Gw2Sharp.WebApi.V2.Models;

namespace BLIT.UI;

public partial class DeliveryBoxPreview : PanelContainer
{
    [Export]
    RichTextLabel coinsLabel;
    [Export]
    GridContainer itemsContainer;
    [Export]
    PackedScene itemPreview;

    public override void _Ready()
    {
        itemsContainer.ClearChildren();
    }

    public void Refresh(long coins, IEnumerable<CommerceDeliveryItem> items)
    {
        var curSize = coinsLabel.CustomMinimumSize;
        coinsLabel.CustomMinimumSize = new Vector2(coins > 10000 ? 125 : coins > 100 ? 75 : 35, curSize.Y);
        coinsLabel.Text = coins.ToCurrencyString(RichImageType.PX32).AlignRichString(RichStringAlignment.CENTER);

        itemsContainer.ClearChildren();
        itemsContainer.Columns = Mathf.Max(1, Mathf.CeilToInt(Mathf.Sqrt(items.Count())));
        foreach (var item in items)
        {
            var instance = itemPreview.Instantiate<DeliveryBoxItem>();
            instance.Init(item);
            itemsContainer.AddChildSafe(instance);
        }
    }
}
