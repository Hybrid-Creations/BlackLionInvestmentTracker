using System.Threading;
using System.Threading.Tasks;
using Godot;

namespace BLIT.UI;

public partial class DeliveryBox : Button
{
    [ExportCategory("Main Button")]
    [Export]
    protected Texture2D deliveryBoxEmpty;
    [Export]
    protected Texture2D deliveryBoxFull;

    [ExportCategory("Notifications")]
    [ExportSubgroup("Coins")]
    [Export]
    TextureRect coinsIcon;
    [Export]
    protected Texture2D goldIcon;
    [Export]
    protected Texture2D silverIcon;
    [Export]
    protected Texture2D copperIcon;

    [ExportSubgroup("Items")]
    [Export]
    Panel itemsBackground;
    [Export]
    Label itemsCountLabel;
    [Export]
    TextureRect tooManyItemsIcon;

    [ExportCategory("Preview")]
    [Export]
    DeliveryBoxPreview deliveryBoxPreview;
    [Export]
    Vector2 centeredPosition;
    [Export]
    Vector2 offsetPosition;

    bool updating;

    public override void _Ready()
    {
        ClearVisuals();
        deliveryBoxPreview.Hide();
    }

    public void RefreshData(CancellationToken cancelToken)
    {
        if (updating == true) return;
        updating = true;
        ClearVisuals();
        Task.Run(() =>
        {
            var deliveryBox = Main.MyClient.WebApi.V2.Commerce.Delivery.GetAsync().Result;
            var coins = deliveryBox.Coins;
            var items = deliveryBox.Items;

            if (cancelToken.IsCancellationRequested) return;

            Icon = items.Count > 0 || coins > 0 ? deliveryBoxFull : deliveryBoxEmpty;

            if (coins > 0)
            {
                coinsIcon.Show();
                coinsIcon.Texture = coins > 10000 ? goldIcon : coins > 100 ? silverIcon : copperIcon;
            }

            if (items.Count > 0)
            {
                itemsBackground.Show();
                if (items.Count > 99)
                {
                    itemsCountLabel.Hide();
                    tooManyItemsIcon.Show();
                }
                else
                    itemsCountLabel.Text = $"{items.Count}";
            }

            deliveryBoxPreview.Position = items.Count < 2 ? centeredPosition : offsetPosition;
            deliveryBoxPreview.Refresh(coins, items);
            updating = false;
        }, cancelToken);
    }

    void ClearVisuals()
    {
        Icon = deliveryBoxEmpty;
        coinsIcon.Hide();
        itemsBackground.Hide();
        tooManyItemsIcon.Hide();
        itemsCountLabel.Show();
    }

    public void OnMouseEntered()
    {
        deliveryBoxPreview.Show();
    }

    public void OnMouseExited()
    {
        deliveryBoxPreview.Hide();
    }
}
