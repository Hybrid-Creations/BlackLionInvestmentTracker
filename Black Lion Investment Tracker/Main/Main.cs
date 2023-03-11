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

    BetterTimer refreshDatabaseTimer;
    BetterTimer refreshDeliveryBoxTimer;

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
        refreshDatabaseTimer = new BetterTimer
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

            var completeListTask = CompletedInvestments.ListInvestmentDatasAsync(Database.CollapsedCompletedInvestments, "Listing Completed Investments... ", refreshCancelSource.Token);
            var pendingListTask = PendingInvestments.ListInvestmentDatasAsync(Database.CollapsedPendingInvestments, "Listing Pending Investments... ", refreshCancelSource.Token);
            var potentialListTask = PotentialInvestments.ListInvestmentDatasAsync(Database.CollapsedPotentialInvestments, "Listing Potential Investments... ", refreshCancelSource.Token);

            try
            {
                await Task.WhenAll(completeListTask, pendingListTask, potentialListTask);
            }
            catch (Exception e)
            {
                GD.PushError(e);
            }
        };
        refreshDatabaseTimer.Start(true);
    }

    void RefreshDeliveryBoxOnInverval()
    {
        refreshDeliveryBoxTimer = new BetterTimer
        {
            Interval = TimeSpan.FromSeconds(Settings.Data.databaseInterval),
            Repeat = true
        };
        refreshDeliveryBoxTimer.Elapsed += async () =>
       {
           do
           {
               if (refreshCancelSource.IsCancellationRequested)
                   break;

               DeliveryBox.RefreshData(refreshCancelSource.Token);

               await Task.Delay(Settings.Data.deliveryBoxInterval * 1000);
           }
           while (true);

       };
        refreshDeliveryBoxTimer.Start(true);
    }

    public void RefreshNow()
    {
        refreshDatabaseTimer.InvokeASAP();
        refreshDeliveryBoxTimer.InvokeASAP();
    }

    public void RefreshDeliveryBox()
    {
        DeliveryBox.RefreshData(refreshCancelSource.Token);
    }

    public void CloseApp()
    {
        Cleanup();
        GetTree().Quit();
    }

    void Cleanup()
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
        if (what == NotificationWMCloseRequest || what == NotificationCrash)
            CloseApp();
    }
}
