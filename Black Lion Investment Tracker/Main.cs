using System;
using Godot;
using Gw2Sharp;

namespace BLIT;

public partial class Main : Node
{
    [Export]
    VBoxContainer itemHolder;

    public static Gw2Client MyClient { get; private set; }

    public static InvestmentsDatabase Database { get; private set; }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        try
        {
            if (Saving.TryLoadDatabase(out var newData))
            {
                GD.Print("Sucessfully loaded database");
                Database = newData;
            }
            else
            {
                GD.Print("Failed to loaded database");
                Database = new InvestmentsDatabase();
            }

            GD.Print($"{Database.Investments.Count}, {newData.Investments.Count}, {newData.CollapsedInvestments.Count}, {newData.NotInvestments.Count}");

            MyClient = new Gw2Client(new Connection(Settings.Data.APIKey));
        }
        catch (Exception e)
        {
            GD.PrintErr(e);
            GetTree().Quit();
        }
    }

    public override void _Notification(int what)
    {
        if (what == NotificationWMCloseRequest)
        {
            GD.Print("Quitting");
            Saving.SaveAll();
            MyClient.Dispose();
            GetTree().Quit(); // default behavior
        }
    }
}
