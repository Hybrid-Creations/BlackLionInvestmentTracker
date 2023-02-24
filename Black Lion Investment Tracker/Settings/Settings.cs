using System.IO;
using System.Text.Json;
using BLIT;
using Godot;

public partial class Settings : Panel
{
    [Export]
    LineEdit apiKeyField;
    [Export]
    PackedScene mainScene;

    public static SettingsData Data { get; private set; } = new SettingsData();

    public override void _Ready()
    {
        base._Ready();

        if (Saving.TryLoadSettings(out var savedData))
        {
            Data = savedData;
            if (string.IsNullOrWhiteSpace(Data.APIKey) == false)
                GetTree().ChangeSceneToPacked(mainScene);
        };
    }

    public void SetAPIKey()
    {
        if (string.IsNullOrWhiteSpace(apiKeyField.Text) == false)
        {
            Data.APIKey = apiKeyField.Text;

            Saving.SaveSettings(Data);
            GetTree().ChangeSceneToPacked(mainScene);
        }
    }

    public class SettingsData
    {
        public string APIKey;
    }
}

public static partial class Saving
{
    const string settingsPath = "user://settings";

    public static void SaveSettings(Settings.SettingsData data)
    {
        var config = new ConfigFile();
        config.SetValue("Settings", nameof(Settings.SettingsData.APIKey), data.APIKey);
        config.Save(settingsPath);
    }

    public static bool TryLoadSettings(out Settings.SettingsData data)
    {
        data = new Settings.SettingsData();

        var config = new ConfigFile();

        if (config.Load(settingsPath) == Error.Ok)
        {
            data.APIKey = config.GetValue("Settings", nameof(Settings.SettingsData.APIKey)).AsString();
            return true;
        }
        return false;
    }
}
