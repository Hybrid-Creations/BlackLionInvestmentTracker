
using System;
using Godot;
using Newtonsoft.Json;

namespace BLIT.Saving;

public class SaveSystem
{
    public static void SaveToFile<T>(string pathOfFileToSaveTo, T objectToSave)
    {
        if (FileAccess.FileExists(pathOfFileToSaveTo))
        {
            using var readFile = FileAccess.Open(pathOfFileToSaveTo, FileAccess.ModeFlags.Read);
            using var writeFile = FileAccess.Open($"{pathOfFileToSaveTo}.bak", FileAccess.ModeFlags.Write);
            writeFile.StoreString(JsonConvert.SerializeObject(objectToSave));
        }

        using var file = FileAccess.Open(pathOfFileToSaveTo, FileAccess.ModeFlags.Write);
        file.StoreString(JsonConvert.SerializeObject(objectToSave));
    }

    public static bool TryLoadFromFile<T>(string pathOfFileToLoadFrom, out T @object)
    {
        @object = default;
        if (FileAccess.FileExists(pathOfFileToLoadFrom))
        {
            try
            {
                using var dbFile = FileAccess.Open(pathOfFileToLoadFrom, FileAccess.ModeFlags.Read);
                @object = JsonConvert.DeserializeObject<T>(dbFile.GetAsText());
                return true;
            }
            catch (Exception e)
            {
                GD.PushError(e);

                string backupPath = $"{pathOfFileToLoadFrom}.bak";
                if (FileAccess.FileExists(backupPath))
                {
                    try
                    {
                        using var dbFile = FileAccess.Open(backupPath, FileAccess.ModeFlags.Read);
                        @object = JsonConvert.DeserializeObject<T>(dbFile.GetAsText());
                        return true;
                    }
                    catch (Exception ee)
                    {
                        GD.PushError(ee);
                        return false;
                    }
                }
                else return false;
            }
        }
        else return false;
    }
}
