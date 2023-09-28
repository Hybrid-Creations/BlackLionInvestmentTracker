using System;
using System.Threading;
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
        ThreadsHelper.SetMainThreadId(System.Threading.Thread.CurrentThread.ManagedThreadId);

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
        refreshDatabaseTimer = new ThreadedTimer { Interval = TimeSpan.FromSeconds(Settings.Data.DatabaseInterval), Repeat = true };
        refreshDatabaseTimer.Elapsed += async () =>
        {
            if (refreshCancelSource.IsCancellationRequested)
            {
                refreshDatabaseTimer.Stop();
                return;
            }

            await Database.RefreshDataAsync(refreshCancelSource.Token);

            GD.Print("Listing Completed Investments");
            CompletedInvestments.ListInvestmentDatas(Database.CollapsedCompletedInvestments, "Listing Completed Investments... ");
            GD.Print("Listing Pending Investments");
            PendingInvestments.ListInvestmentDatas(Database.CollapsedPendingInvestments, "Listing Pending Investments... ");
            GD.Print("Listing Potential Investments");
            PotentialInvestments.ListInvestmentDatas(Database.CollapsedPotentialInvestments, "Listing Potential Investments... ");

            GD.Print("Databse & UI Update Done");
        };
        refreshDatabaseTimer.Start(true);
    }

    private void RefreshDeliveryBoxOnInverval()
    {
        refreshDeliveryBoxTimer = new ThreadedTimer { Interval = TimeSpan.FromSeconds(Settings.Data.DeliveryBoxInterval), Repeat = true };
        refreshDeliveryBoxTimer.Elapsed += async () =>
        {
            if (refreshCancelSource.IsCancellationRequested)
                return;

            await DeliveryBox.RefreshAsync(refreshCancelSource.Token);
        };
        refreshDeliveryBoxTimer.Start(true);
    }

    public void RefreshNow()
    {
        refreshDatabaseTimer.InvokeASAP();
        refreshDeliveryBoxTimer?.InvokeASAP();
        APIStatusIndicator.ClearStatus();
    }

    public void CloseApp()
    {
        Cleanup(true);
        GetTree().Quit();
    }

    private void Cleanup(bool createDatabaseBackup)
    {
        GD.Print("Main Cleanup");
        Database.Save(createDatabaseBackup);
        Settings.Save();
        Cache.Items.Save();
        MyClient.Dispose();

        //refreshCancelSource might still leak here :(
        // This should be fine however as we intend to quit when this is called
        refreshCancelSource.Cancel();
        refreshDatabaseTimer?.Stop();
        refreshDeliveryBoxTimer?.Stop();
    }

    public override void _ExitTree()
    {
        Cleanup(false);
    }

    public override void _Notification(int what)
    {
        if (what == NotificationWMCloseRequest)
            CloseApp();
        if (what == NotificationCrash)
            GD.PrintErr("AHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHH");
    }
}
