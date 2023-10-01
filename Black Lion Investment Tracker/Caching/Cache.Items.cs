using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.Serialization;
using BLIT.Saving;
using BLIT.UI;
using Godot;
using Gw2Sharp.WebApi.V2.Models;
using Newtonsoft.Json;

namespace BLIT;

public static partial class Cache
{
    public static class Items
    {
        static ConcurrentDictionary<int, ItemData> itemDB = new();

        public static ItemData GetItemData(int itemId)
        {
            if (itemDB.TryGetValue(itemId, out var itemData) == false)
            {
                var item = Main.MyClient.WebApi.V2.Items.GetAsync(itemId).Result;
                var iconBytes = Main.MyClient.WebApi.Render.DownloadToByteArrayAsync(item.Icon.Url).Result;
                itemData = new ItemData(item, iconBytes);
                itemDB.TryAdd(itemId, itemData);
            }
            return itemData;
        }

        private const string pathToItems = "user://cache.items";

        public static void Save()
        {
            var saved = new SavedItems();
            saved.itemDB = itemDB;
            SaveSystem.SaveToFile(pathToItems, saved);
        }

        public static void Load()
        {
            if (SaveSystem.TryLoadFromFile(pathToItems, out SavedItems saved))
                itemDB = new ConcurrentDictionary<int, ItemData>(saved.itemDB);
            GD.Print($"Loaded Item Database => items: {itemDB.Count}");
        }
    }

    public class SavedItems
    {
        public IDictionary<int, ItemData> itemDB = new Dictionary<int, ItemData>();
    }
}

[DataContract]
public class ItemData
{
    [DataMember]
    public string Name { get; private set; }

    [DataMember]
    private byte[] iconBytes;

    private ImageTexture icon;
    public ImageTexture Icon
    {
        get
        {
            if (icon is null)
            {
                var image = new Image();
                image.LoadPngFromBuffer(iconBytes);
                icon = ImageTexture.CreateFromImage(image);
            }

            return icon;
        }
    }

    public ItemData(Item gw2Item, byte[] _iconBytes)
    {
        Name = gw2Item.Name;
        iconBytes = _iconBytes;
    }

    [JsonConstructor]
    public ItemData() { }
}
