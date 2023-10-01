using Godot;
using Gw2Sharp.WebApi.V2.Models;

namespace BLIT.UI;

public partial class DeliveryBoxItem : Panel
{
    [Export]
    ItemIcon icon;

    public void Init(CommerceDeliveryItem item)
    {
        // We actually want to tell it not to render the delivery box icon here, as we already know that this item is in the delivery box
        icon.Init(Cache.Items.GetItemData(item.Id).Icon, item.Count, false);
    }
}
