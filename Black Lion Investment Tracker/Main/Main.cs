using System;
using System.Threading;
using System.Threading.Tasks;
using BLIT.Investments;
using BLIT.Timers;
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

    CancellationTokenSource refreshCancelSource;

    ThreadedTimer refreshDatabaseTimer;
    ThreadedTimer refreshDeliveryBoxTimer;

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

    private void RefreshDatabaseOnInterval()
    {
        refreshDatabaseTimer = new ThreadedTimer
        {
            Interval = TimeSpan.FromSeconds(Settings.Data.databaseInterval),
            Repeat = true
        };
        refreshDatabaseTimer.Elapsed += async () =>
        {
            if (refreshCancelSource.IsCancellationRequested)
            {
                refreshDatabaseTimer.Stop();
                return;
            }

            await Database.RefreshDataAsync(refreshCancelSource.Token);

            await CompletedInvestments.ListInvestmentDatasAsync(Database.CollapsedCompletedInvestments, "Listing Completed Investments... ", refreshCancelSource.Token);
            await PendingInvestments.ListInvestmentDatasAsync(Database.CollapsedPendingInvestments, "Listing Pending Investments... ", refreshCancelSource.Token);
            await PotentialInvestments.ListInvestmentDatasAsync(Database.CollapsedPotentialInvestments, "Listing Potential Investments... ", refreshCancelSource.Token);

            GD.Print("Done");
        };
        refreshDatabaseTimer.Start(true);
    }

    private void RefreshDeliveryBoxOnInverval()
    {
        refreshDeliveryBoxTimer = new ThreadedTimer
        {
            Interval = TimeSpan.FromSeconds(Settings.Data.deliveryBoxInterval),
            Repeat = true
        };
        refreshDeliveryBoxTimer.Elapsed += async () =>
       {
           if (refreshCancelSource.IsCancellationRequested)
               return;

           await DeliveryBox.RefreshDataAsync(refreshCancelSource.Token);
       };
        refreshDeliveryBoxTimer.Start(true);
    }

    public void RefreshNow()
    {
        refreshDatabaseTimer.InvokeASAP();
        refreshDeliveryBoxTimer.InvokeASAP();
    }

    public void CloseApp()
    {
        Cleanup();
        GetTree().Quit();
    }

    private void Cleanup()
    {
        GD.Print("Main Cleanup");
        Database.Save();
        Settings.Save();
        Cache.Items.Save();
        MyClient.Dispose();

        //refreshCancelSource might still leak here :(
        refreshCancelSource.Cancel();
        refreshDatabaseTimer.Stop();
        refreshDeliveryBoxTimer.Stop();
    }

    public override void _ExitTree()
    {
        Cleanup();
    }

    public override void _Notification(int what)
    {
        if (what == NotificationWMCloseRequest)
            CloseApp();
        if (what == NotificationCrash)
            GD.PrintErr("AHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHH");
    }
}
