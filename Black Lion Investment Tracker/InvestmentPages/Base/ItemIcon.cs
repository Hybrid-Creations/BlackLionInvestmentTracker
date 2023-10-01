using System;
using Godot;

namespace BLIT.UI;

public partial class ItemIcon : TextureRect
{
    [Export]
    TextureRect icon;

    [Export]
    Label quantity;

    [Export]
    TextureRect inDeliveryBox;

    public void Init(Texture2D icon, long quantity, bool isInDeliveryBox)
    {
        this.icon.Texture = icon;
        this.quantity.Text = $"{quantity}";

        if (isInDeliveryBox == false)
            inDeliveryBox.QueueFree();
    }
}
