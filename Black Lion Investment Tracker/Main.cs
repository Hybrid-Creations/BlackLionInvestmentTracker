using BLIT.Investments;
using Godot;
using Gw2Sharp;

namespace BLIT.UI;

public partial class Main : Node
{
    [Export]
    VBoxContainer itemHolder;

    [Export]
    CompletedInvestmentsPage CompletedInvestments;
    [Export]
    PendingInvestmentsPage PendingInvestments;
    [Export]
    PotentialInvestmentsPage PoptentiaInvestments;

    public static Gw2Client MyClient { get; private set; }

    public static InvestmentsDatabase Database { get; private set; } = new();

    double refreshTimer = 1;
    readonly int refreshTimeSeconds = 300;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        Database.Load();
        Cache.Items.Load();

        MyClient = new Gw2Client(new Connection(Settings.Data.APIKey));
    }

    public override void _Process(double delta)
    {
        refreshTimer -= delta;
        if (refreshTimer <= 0)
        {
            RefreshDatabase();
            refreshTimer = refreshTimeSeconds;
        }
    }

    public void RefreshDatabase()
    {
        Database.Update(() =>
        {
            CompletedInvestments.ListInvestmentDatas(Database.CollapsedCompletedInvestments, "Listing Completed Investments... ");
            // PendingInvestments.ListInvestmentDatas(Database.CollapsedPendingInvestments, "Listing Pending Investments... ");
            // PoptentiaInvestments.ListInvestmentDatas(Database.CollapsedPotentialInvestments, "Listing Potential Investments... ");
        });
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
