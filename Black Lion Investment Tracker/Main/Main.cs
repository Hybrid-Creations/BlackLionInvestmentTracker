using System.Threading;
using System.Threading.Tasks;
using BLIT.Investments;
using Godot;
using Gw2Sharp;

namespace BLIT.UI;

public partial class Main : Node
{
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

    readonly int refreshDatabaseTimeSeconds = 300;
    readonly int refreshDeliveryBoxTimeSeconds = 30;
    CancellationTokenSource refreshTaskSource;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        Database.Load();
        Cache.Items.Load();

        MyClient = new Gw2Client(new Connection(Settings.Data.APIKey));

        refreshTaskSource = new CancellationTokenSource();
        RefreshDatabaseOnInterval();
        RefreshDeliveryBoxOnInverval();
    }

    void RefreshDatabaseOnInterval()
    {
        Task.Run(async () =>
       {
           do
           {
               if (refreshTaskSource.IsCancellationRequested)
                   break;
               RefreshDatabase();
               await Task.Delay(refreshDatabaseTimeSeconds * 1000);
           }
           while (true);

       }, refreshTaskSource.Token);
    }

    void RefreshDeliveryBoxOnInverval()
    {
        Task.Run(async () =>
       {
           do
           {
               if (refreshTaskSource.IsCancellationRequested)
                   break;
               DeliveryBox.RefreshData();
               await Task.Delay(refreshDeliveryBoxTimeSeconds * 1000);
           }
           while (true);

       }, refreshTaskSource.Token);
    }

    public void RefreshDatabase()
    {
        Database.RefreshData(() =>
        {
            CompletedInvestments.ListInvestmentDatas(Database.CollapsedCompletedInvestments, "Listing Completed Investments... ");
            PendingInvestments.ListInvestmentDatas(Database.CollapsedPendingInvestments, "Listing Pending Investments... ");
            PotentialInvestments.ListInvestmentDatas(Database.CollapsedPotentialInvestments, "Listing Potential Investments... ");
        });
    }

    public void RefreshDeliveryBox()
    {
        DeliveryBox.RefreshData();
    }

    public void MinimizeApp()
    {
        GetWindow().Mode = Window.ModeEnum.Minimized;
    }

    public void CloseApp()
    {
        GetTree().Quit();
    }

    public override void _Notification(int what)
    {
        if (what == NotificationWMCloseRequest)
        {
            GD.Print("Quitting");
            Database.Save();
            Settings.Save();
            Cache.Items.Save();
            MyClient.Dispose();
            GetTree().Quit(); // default behavior
        }
    }
}
