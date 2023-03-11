using System;
using Godot;

namespace BLIT.UI;

public partial class ItemIcon : TextureRect
{
    [Export] TextureRect icon;
    [Export] Label quantity;

    public void Init(Texture2D icon, long quantity)
    {
        this.icon.Texture = icon;
        this.quantity.Text = $"{quantity}";
    }
}
