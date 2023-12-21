using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Godot;
using Gw2Sharp.WebApi.V2.Models;
using BLIT.Status;
using System.Linq;
using BLIT.Tools;

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

    static IReadOnlyList<(int, int)> itemsInDeliveryBox;

    const string StatusKey = $"{nameof(DeliveryBox)}";

    public override void _Ready()
    {
        ClearVisuals();
        deliveryBoxPreview.Hide();
    }

    public async Task RefreshAsync(CancellationToken cancelToken)
    {
        if (updating == true)
            return;
        updating = true;

        AppStatusManager.ShowStatus(StatusKey, $"Updating Delivery Box...");
        CommerceDelivery deliveryBox;
        try
        {
            deliveryBox = await Main.MyClient.WebApi.V2.Commerce.Delivery.GetAsync(cancelToken);
        }
        catch (System.Exception e)
        {
            GD.PushError(e.Message);
            AppStatusManager.ClearStatus(StatusKey);
            APIStatusIndicator.ShowStatus("Failed to get Delivery Box");
            updating = false;
            return;
        }

        var coins = deliveryBox.Coins;
        var items = deliveryBox.Items;

        if (cancelToken.IsCancellationRequested)
            return;

        ThreadsHelper.CallOnMainThread(() =>
        {
            ClearVisuals();
            UpdateVisuals(coins, items);
            itemsInDeliveryBox = items.Select(item => (item.Id, item.Count)).ToList();

            deliveryBoxPreview.Position = items.Count < 2 ? centeredPosition : offsetPosition;
            deliveryBoxPreview.Refresh(coins, items);

            AppStatusManager.ClearStatus(StatusKey);
        });
        updating = false;
    }

    void ClearVisuals()
    {
        Icon = deliveryBoxEmpty;
        coinsIcon.Hide();
        itemsBackground.Hide();
        tooManyItemsIcon.Hide();
        itemsCountLabel.Show();
    }

    private void UpdateVisuals(int coins, IReadOnlyList<CommerceDeliveryItem> items)
    {
        Icon = items.Count > 0 || coins > 0 ? deliveryBoxFull : deliveryBoxEmpty;

        if (coins > 0)
        {
            coinsIcon.Show();
            coinsIcon.Texture =
                coins > 10000
                    ? goldIcon
                    : coins > 100
                        ? silverIcon
                        : copperIcon;
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
    }

    public void OnMouseEntered()
    {
        deliveryBoxPreview.Show();
    }

    public void OnMouseExited()
    {
        deliveryBoxPreview.Hide();
    }

    public static bool IsInDeliveryBox(int itemId, int quantity)
    {
        return itemsInDeliveryBox.Contains((itemId, quantity));
    }
}
