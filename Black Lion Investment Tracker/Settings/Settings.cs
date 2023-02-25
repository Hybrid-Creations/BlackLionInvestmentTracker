using BLIT.Saving;
using Godot;

namespace BLIT;

public partial class Settings : Panel
{
    [Export]
    LineEdit apiKeyField;
    [Export]
    PackedScene mainScene;

    public static SettingsData Data { get; private set; } = new SettingsData();

    public override void _Ready()
    {
        Load();
        if (string.IsNullOrWhiteSpace(Data.APIKey) == false)
            GetTree().ChangeSceneToPacked(mainScene);
    }

    public void SetAPIKey()
    {
        if (string.IsNullOrWhiteSpace(apiKeyField.Text) == false)
        {
            Data.APIKey = apiKeyField.Text;

            Save();
            GetTree().ChangeSceneToPacked(mainScene);
        }
    }

    public class SettingsData
    {
        public string APIKey;
    }

    const string settingsPath = "user://settings";
    public static void Save()
    {
        var config = new ConfigFile();
        config.SetValue("Settings", nameof(Settings.SettingsData.APIKey), Data.APIKey);
        config.Save(settingsPath);
    }

    public static bool Load()
    {
        var config = new ConfigFile();

        if (config.Load(settingsPath) == Error.Ok)
        {
            Data.APIKey = config.GetValue("Settings", nameof(Settings.SettingsData.APIKey)).AsString();
            return true;
        }
        return false;
    }
}
