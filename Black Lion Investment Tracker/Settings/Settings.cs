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
		databaseIntervalField.Value = Data.DatabaseInterval;
		deliveryBoxIntervalField.Value = Data.DeliveryBoxInterval;

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
		Data.DatabaseInterval = (int)databaseIntervalField.Value;
		Data.DeliveryBoxInterval = (int)deliveryBoxIntervalField.Value;
	}

	void ContinueToMainScene()
	{
		GetTree().ChangeSceneToPacked(mainScene);
	}

	public class SettingsData
	{
		public string APIKey;
		public int DatabaseInterval;
		public int DeliveryBoxInterval;

		public const int DatabaseIntervalDefault = 300;
		public const int DeliveryBoxIntervalDefault = 30;
	}

	const string settingsPath = "user://settings";
	public static void Save()
	{
		var config = new ConfigFile();
		config.SetValue("Settings", nameof(Settings.SettingsData.APIKey), Data.APIKey);
		config.SetValue("Settings", nameof(Settings.SettingsData.DatabaseInterval), Data.DatabaseInterval);
		config.SetValue("Settings", nameof(Settings.SettingsData.DeliveryBoxInterval), Data.DeliveryBoxInterval);
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
		}
		else
		{
			Data.APIKey = "";
			Data.DatabaseInterval = SettingsData.DatabaseIntervalDefault;
			Data.DeliveryBoxInterval = SettingsData.DeliveryBoxIntervalDefault;
		}

		GD.Print($"Loaded Settings => API Key Exists: {string.IsNullOrWhiteSpace(Data.APIKey) == false} Database Interval: {Data.DatabaseInterval} Delivery Box Interval: {Data.DeliveryBoxInterval}");
	}
}
