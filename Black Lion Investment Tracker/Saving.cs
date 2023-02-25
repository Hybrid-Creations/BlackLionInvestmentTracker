
using Godot;
using Newtonsoft.Json;

namespace BLIT.Saving;

public class SaveSystem
{
    public static void SaveToFile<T>(string pathOfFileToSaveTo, T objectToSave)
    {
        using var file = FileAccess.Open(pathOfFileToSaveTo, FileAccess.ModeFlags.Write);
        file.StoreString(JsonConvert.SerializeObject(objectToSave));
    }

    public static bool TryLoadFromFile<T>(string pathOfFileToLoadFrom, out T @object)
    {
        @object = default;
        if (FileAccess.FileExists(pathOfFileToLoadFrom))
        {
            using var dbFile = FileAccess.Open(pathOfFileToLoadFrom, FileAccess.ModeFlags.Read);
            @object = JsonConvert.DeserializeObject<T>(dbFile.GetAsText());
            return true;
        }
        else return false;
    }
}
