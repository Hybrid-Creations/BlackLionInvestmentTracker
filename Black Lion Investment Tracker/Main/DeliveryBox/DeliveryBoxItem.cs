using Godot;
using Gw2Sharp.WebApi.V2.Models;

namespace BLIT.UI;

public partial class DeliveryBoxItem : Panel
{
    [Export]
    ItemIcon icon;
    public void Init(CommerceDeliveryItem item)
    {
        icon.Init(Cache.Items.GetItemData(item.Id).Icon, item.Count);
    }
}
