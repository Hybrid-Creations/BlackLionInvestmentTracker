
namespace BLIT;

public partial class Saving
{
    public static void SaveAll()
    {
        SaveDatabase(Main.Database);
        SaveSettings(Settings.Data);
    }
}
