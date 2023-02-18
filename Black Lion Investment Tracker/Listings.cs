using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Godot;

namespace BLIT;

public partial class Listings : ScrollContainer
{
    CancellationTokenSource cancellationTokenSource;
    PackedScene itemScene;
    VBoxContainer listingsContainer;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        itemScene = GD.Load<PackedScene>("res://Item.tscn");
        cancellationTokenSource = new CancellationTokenSource();
        listingsContainer = GetChild<VBoxContainer>(0);
    }

    public void Continue()
    {
        CancellationToken token = cancellationTokenSource.Token;
        Task.Run(async () =>
        {
            do
            {
                await Task.Delay(500);
                if (token.IsCancellationRequested)
                {
                    break;
                }
                else
                    ShowListings();
                await Task.Delay(30000);

            }
            while (true);

        }, token);
    }

    public void Pause()
    {
        cancellationTokenSource.Cancel();
        cancellationTokenSource = new CancellationTokenSource();
    }

    void ShowListings()
    {
        Task.Run(() =>
        {
            var buys = Main.MyClient.WebApi.V2.Commerce.Transactions.Current.Buys.GetAsync().Result;
            GD.Print($"Listings: {buys.Count}");


            var list = buys.OrderBy(bo => bo.Created).Select(bo => (bo, Main.MyClient.WebApi.V2.Items.GetAsync(bo.ItemId).Result)).ToList();

            foreach (var child in listingsContainer.GetChildren())
                child.QueueFree();

            // foreach (var (buyOrder, item) in list)
            // {
            //     var instance = itemScene.Instantiate<Item>();
            //     instance.Init($"{item.Name}", null);
            //     listingsContainer.AddChild(instance);
            // }
        });
    }
}
