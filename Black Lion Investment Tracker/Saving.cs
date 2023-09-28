using System;
using System.Collections;
using System.Linq;
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
                else
                    return false;
            }
        }
        else
            return false;
    }

    public static bool CreateBackup(string pathOfFileToBackup)
    {
        if (FileAccess.FileExists(pathOfFileToBackup))
        {
            using var readFile = FileAccess.Open(pathOfFileToBackup, FileAccess.ModeFlags.Read);
            using var writeFile = FileAccess.Open($"{pathOfFileToBackup}.{DateTime.Now.ToString().Replace('/', '_').Replace(':', '-')}.bak", FileAccess.ModeFlags.Write);
            writeFile.StoreString(readFile.GetAsText());

            var allBakFiles = DirAccess.GetFilesAt("user://").Where(fi => fi.EndsWith(".bak")).OrderBy(name => FileAccess.GetModifiedTime($"user://{name}")).ToList();
            var numtoDelete = allBakFiles.Count - Settings.Data.DatabaseBackupsToKeep;
            var queue = new Queue(allBakFiles);

            while (numtoDelete > 0)
            {
                var oldest = queue.Dequeue();
                OS.MoveToTrash(ProjectSettings.GlobalizePath($"user://{oldest}"));
                numtoDelete--;
            }

            return true;
        }
        return false;
    }
}
