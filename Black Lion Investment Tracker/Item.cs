using Godot;

namespace BLIT;
public partial class Item : HBoxContainer
{
    public void Init(string name, int amount)
    {
        GetNode<Label>("Name").Text = name;
        GetNode<Label>("BuyPrice").Text = amount.ToString();
    }
}

public class ItemData
{
    public string Name;
    public int Profit;

    public ItemData(string name, int profit)
    {
        Name = name;
        Profit = profit;
    }
}