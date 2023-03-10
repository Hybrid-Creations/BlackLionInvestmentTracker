using Godot;

namespace BLIT;

public partial class Settings : Panel
{
    [Export]
    LineEdit apiKeyField;
    [Export]
    SpinBox databaseIntervalField;
    [Export]
    SpinBox deliveryBoxIntervalField;
    [Export]
    PackedScene mainScene;

    public static SettingsData Data { get; private set; } = new SettingsData();

    static bool firstLoad = true;

    public override void _Ready()
    {
        Load();

        apiKeyField.Text = Data.APIKey;
        databaseIntervalField.Value = Data.databaseInterval;
        deliveryBoxIntervalField.Value = Data.deliveryBoxInterval;

        if (firstLoad)
        {
            firstLoad = false;

            if (string.IsNullOrWhiteSpace(Data.APIKey) == false)
                ContinueToMainScene();
        }
    }

    public void SaveSettings()
    {
        if (string.IsNullOrWhiteSpace(apiKeyField.Text) == false)
        {
            Data.APIKey = apiKeyField.Text;

            Save();
            ContinueToMainScene();
        }
        Data.databaseInterval = (int)databaseIntervalField.Value;
        Data.deliveryBoxInterval = (int)deliveryBoxIntervalField.Value;
    }

    void ContinueToMainScene()
    {
        GetTree().ChangeSceneToPacked(mainScene);
    }

    public class SettingsData
    {
        public string APIKey;
        public int databaseInterval;
        public int deliveryBoxInterval;
    }

    const string settingsPath = "user://settings";
    public static void Save()
    {
        var config = new ConfigFile();
        config.SetValue("Settings", nameof(Settings.SettingsData.APIKey), Data.APIKey);
        config.SetValue("Settings", nameof(Settings.SettingsData.databaseInterval), Data.databaseInterval);
        config.SetValue("Settings", nameof(Settings.SettingsData.deliveryBoxInterval), Data.deliveryBoxInterval);
        config.Save(settingsPath);
    }

    public static bool Load()
    {
        var config = new ConfigFile();

        if (config.Load(settingsPath) == Error.Ok)
        {
            Data.APIKey = config.GetValue("Settings", nameof(Settings.SettingsData.APIKey)).AsString();
            Data.databaseInterval = config.GetValue("Settings", nameof(Settings.SettingsData.databaseInterval), 15).AsInt32();
            Data.deliveryBoxInterval = config.GetValue("Settings", nameof(Settings.SettingsData.deliveryBoxInterval), 300).AsInt32();
            return true;
        }
        return false;
    }
}
