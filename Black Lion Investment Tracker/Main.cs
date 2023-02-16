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
        MyClient = new Gw2Client(new Connection("612B84FA-3D70-EB49-8727-6930F323551DABDBC42E-376E-448B-9FB1-6815DF67CC43"));
        Database = new InvestmentsDatabase();
    }

    public override void _Notification(int what)
    {
        if (what == NotificationWMCloseRequest)
        {
            MyClient.Dispose();
            GetTree().Quit(); // default behavior
        }
    }
}
