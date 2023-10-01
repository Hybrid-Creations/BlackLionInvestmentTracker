using System;
using System.Collections.Generic;
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

    public static CreateBackupResult CreateBackupIfNeeded(string pathOfFileToBackup, TimeSpan interval)
    {
        if (FileAccess.FileExists(pathOfFileToBackup))
        {
            var allBakFiles = DirAccess
                .GetFilesAt("user://")
                .Where(fi => fi.EndsWith(".bak"))
                .Select(name => $"user://{name}")
                .OrderBy(fullName => FileAccess.GetModifiedTime(fullName))
                .ToList();

            ulong fileUnixTime = FileAccess.GetModifiedTime(allBakFiles.Last());
            double currentUnixTime = Time.GetUnixTimeFromSystem();
            double difference = currentUnixTime - fileUnixTime;

            GD.Print(difference);
            GD.Print(interval.TotalSeconds);

            // Skip making a backup if we are on the same day
            if (difference < interval.TotalSeconds)
                return CreateBackupResult.NOT_NEEDED;

            using var readFile = FileAccess.Open(pathOfFileToBackup, FileAccess.ModeFlags.Read);
            using var writeFile = FileAccess.Open($"{pathOfFileToBackup}.{DateTime.Now.ToString().Replace('/', '_').Replace(':', '-')}.bak", FileAccess.ModeFlags.Write);
            writeFile.StoreString(readFile.GetAsText());

            // We add one here as we just made a new backup
            var numtoDelete = allBakFiles.Count + 1 - Settings.Data.DatabaseBackupsToKeep;
            var queue = new Queue<string>(allBakFiles);

            while (numtoDelete > 0)
            {
                var oldest = queue.Dequeue();
                OS.MoveToTrash(ProjectSettings.GlobalizePath(oldest));
                numtoDelete--;
            }

            return CreateBackupResult.SUCCESS;
        }
        return CreateBackupResult.FAULURE;
    }

    public enum CreateBackupResult
    {
        SUCCESS,
        FAULURE,
        NOT_NEEDED
    }

    public static bool RestoreMostRecentBackup(string originalFileName)
    {
        var allBakFiles = DirAccess
            .GetFilesAt("user://")
            .Where(fi => fi.EndsWith(".bak"))
            .Where(fi => fi.Contains(originalFileName))
            .Select(name => $"user://{name}")
            .OrderByDescending(fullName => FileAccess.GetModifiedTime(fullName))
            .ToList();

        var mostRecent = allBakFiles.FirstOrDefault();

        GD.Print(mostRecent);

        if (FileAccess.FileExists(mostRecent))
        {
            using var readFile = FileAccess.Open(mostRecent, FileAccess.ModeFlags.Read);
            using var writeFile = FileAccess.Open($"user://{originalFileName}", FileAccess.ModeFlags.Write);
            writeFile.StoreString(readFile.GetAsText());

            OS.MoveToTrash(ProjectSettings.GlobalizePath(mostRecent));

            return true;
        }

        return false;
    }

    public static bool BackupsExist(string originalFileName)
    {
        var allBakFiles = DirAccess
            .GetFilesAt("user://")
            .Where(fi => fi.EndsWith(".bak"))
            .Where(fi => fi.Contains(originalFileName))
            .Select(name => $"user://{name}")
            .OrderByDescending(fullName => FileAccess.GetModifiedTime(fullName))
            .ToList();

        return allBakFiles.Count > 0;
    }
}
