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
    SpinBox databaseBackupsToKeepField;

    [Export]
    PackedScene mainScene;

    public static SettingsData Data { get; private set; }

    static bool firstLoad = true;

    public override void _Ready()
    {
        Data = new SettingsData(databaseIntervalField.MinValue, deliveryBoxIntervalField.MinValue, databaseBackupsToKeepField.MinValue);
        Load();

        apiKeyField.Text = Data.APIKey;
        databaseIntervalField.Value = Data.DatabaseInterval;
        deliveryBoxIntervalField.Value = Data.DeliveryBoxInterval;
        databaseBackupsToKeepField.Value = Data.DatabaseBackupsToKeep;

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
            Data.DatabaseInterval = (int)databaseIntervalField.Value;
            Data.DeliveryBoxInterval = (int)deliveryBoxIntervalField.Value;
            Data.DatabaseBackupsToKeep = (int)databaseBackupsToKeepField.Value;

            Save();
            ContinueToMainScene();
        }
    }

    void ContinueToMainScene()
    {
        GetTree().ChangeSceneToPacked(mainScene);
    }

    public class SettingsData
    {
        readonly int DatabaseIntervalMin = 30;
        readonly int DeliveryBoxIntervalMin = 15;
        readonly int DatabaseBackupsToKeepMin = 1;

        private int databaseIntervalBackingField;
        private int deliveryBoxIntervalBackingField;
        private int databaseBackupsToKeepBackingField;

        public int DatabaseInterval
        {
            get => databaseIntervalBackingField;
            set { databaseIntervalBackingField = Mathf.Max(value, DatabaseIntervalMin); }
        }
        public int DeliveryBoxInterval
        {
            get => deliveryBoxIntervalBackingField;
            set { deliveryBoxIntervalBackingField = Mathf.Max(value, DeliveryBoxIntervalMin); }
        }
        public int DatabaseBackupsToKeep
        {
            get => databaseBackupsToKeepBackingField;
            set { databaseBackupsToKeepBackingField = Mathf.Max(value, DatabaseBackupsToKeepMin); }
        }

        public string APIKey;

        public const int DatabaseIntervalDefault = 300;
        public const int DeliveryBoxIntervalDefault = 30;
        public const int DatabaseBackupsToKeepDefault = 10;

        public SettingsData(double dbInvervalMinValue, double boxIntervalMinValue, double backupMinValue)
        {
            DatabaseIntervalMin = (int)dbInvervalMinValue;
            DeliveryBoxIntervalMin = (int)boxIntervalMinValue;
            DatabaseBackupsToKeepMin = (int)backupMinValue;
        }
    }

    const string settingsPath = "user://settings";

    public static void Save()
    {
        var config = new ConfigFile();
        config.SetValue("Settings", nameof(Settings.SettingsData.APIKey), Data.APIKey);
        config.SetValue("Settings", nameof(Settings.SettingsData.DatabaseInterval), Data.DatabaseInterval);
        config.SetValue("Settings", nameof(Settings.SettingsData.DeliveryBoxInterval), Data.DeliveryBoxInterval);
        config.SetValue("Settings", nameof(Settings.SettingsData.DatabaseBackupsToKeep), Data.DatabaseBackupsToKeep);
        config.Save(settingsPath);
    }

    public static void Load()
    {
        var config = new ConfigFile();

        if (config.Load(settingsPath) == Error.Ok)
        {
            Data.APIKey = config.GetValue("Settings", nameof(Settings.SettingsData.APIKey)).AsString();
            Data.DatabaseInterval = config.GetValue("Settings", nameof(Settings.SettingsData.DatabaseInterval), SettingsData.DatabaseIntervalDefault).AsInt32();
            Data.DeliveryBoxInterval = config.GetValue("Settings", nameof(Settings.SettingsData.DeliveryBoxInterval), SettingsData.DeliveryBoxIntervalDefault).AsInt32();
            Data.DatabaseBackupsToKeep = config.GetValue("Settings", nameof(Settings.SettingsData.DatabaseBackupsToKeep), SettingsData.DatabaseBackupsToKeepDefault).AsInt32();
        }
        else
        {
            Data.APIKey = "";
            Data.DatabaseInterval = SettingsData.DatabaseIntervalDefault;
            Data.DeliveryBoxInterval = SettingsData.DeliveryBoxIntervalDefault;
            Data.DatabaseBackupsToKeep = SettingsData.DatabaseBackupsToKeepDefault;
        }

        GD.Print(
            $"Loaded Settings => API Key Exists: {string.IsNullOrWhiteSpace(Data.APIKey) == false} Database Interval: {Data.DatabaseInterval} Delivery Box Interval: {Data.DeliveryBoxInterval}"
        );
    }
}
