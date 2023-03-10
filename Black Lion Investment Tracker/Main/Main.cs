using System.Threading;
using System.Threading.Tasks;
using BLIT.Investments;
using Godot;
using Gw2Sharp;

namespace BLIT.UI;

public partial class Main : Node
{
    [ExportCategory("Settings")]
    [Export(PropertyHint.File, "*.tscn")]
    string settingsScene;

    [ExportCategory("Pages")]
    [Export]
    CompletedInvestmentsPage CompletedInvestments;
    [Export]
    PendingInvestmentsPage PendingInvestments;
    [Export]
    PotentialInvestmentsPage PotentialInvestments;

    [ExportCategory("Delivery Box")]
    [Export]
    DeliveryBox DeliveryBox;

    public static Gw2Client MyClient { get; private set; }

    public static InvestmentsDatabase Database { get; private set; } = new();

    CancellationTokenSource refreshCancelSource;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        Database.Load();
        Cache.Items.Load();

        MyClient?.Dispose();
        MyClient = new Gw2Client(new Connection(Settings.Data.APIKey));

        refreshCancelSource = new CancellationTokenSource();
        RefreshDatabaseOnInterval();
        RefreshDeliveryBoxOnInverval();
    }

    void RefreshDatabaseOnInterval()
    {
        Task.Run(async () =>
       {
           do
           {
               if (refreshCancelSource.IsCancellationRequested)
                   break;

               await Database.RefreshDataAsync(refreshCancelSource.Token);
               CompletedInvestments.ListInvestmentDatasAsync(Database.CollapsedCompletedInvestments, "Listing Completed Investments... ", refreshCancelSource.Token);
               PendingInvestments.ListInvestmentDatasAsync(Database.CollapsedPendingInvestments, "Listing Pending Investments... ", refreshCancelSource.Token);
               PotentialInvestments.ListInvestmentDatasAsync(Database.CollapsedPotentialInvestments, "Listing Potential Investments... ", refreshCancelSource.Token);

               await Task.Delay(Settings.Data.databaseInterval * 1000);
           }
           while (true);

       }, refreshCancelSource.Token);
    }

    void RefreshDeliveryBoxOnInverval()
    {
        Task.Run(async () =>
       {
           do
           {
               if (refreshCancelSource.IsCancellationRequested)
                   break;
               DeliveryBox.RefreshData(refreshCancelSource.Token);
               await Task.Delay(Settings.Data.deliveryBoxInterval * 1000);
           }
           while (true);

       }, refreshCancelSource.Token);
    }

    public void RefreshDatabase()
    {
        Task.Run(async () =>
        {

        });
    }

    public void RefreshDeliveryBox()
    {
        DeliveryBox.RefreshData(refreshCancelSource.Token);
    }

    public void OpenSettings()
    {
        GetTree().ChangeSceneToFile(settingsScene);
        refreshCancelSource.Cancel();
    }

    public void MinimizeApp()
    {
        GetWindow().Mode = Window.ModeEnum.Minimized;
    }

    public void CloseApp()
    {
        GD.Print("Quitting");
        Database.Save();
        Settings.Save();
        Cache.Items.Save();
        MyClient.Dispose();
        GetTree().Quit();
    }

    public override void _Notification(int what)
    {
        if (what == NotificationWMCloseRequest || what == NotificationCrash)
        {
            // CloseApp();
        }
    }
}
