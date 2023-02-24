using System.Collections.Generic;
using Godot;
using Gw2Sharp.WebApi.V2.Models;
using Newtonsoft.Json;

namespace BLIT;

public static class Cache
{
    public class Items
    {
        Dictionary<int, Item> itemDB = new();
        public static Items Instance { get; } = new();

        public static Item GetItem(int itemId)
        {
            if (Instance.itemDB.TryGetValue(itemId, out var item) == false)
            {
                item = Main.MyClient.WebApi.V2.Items.GetAsync(itemId).Result;
                Instance.itemDB.Add(itemId, item);
            }

            return item;
        }
    }

    public class Icons
    {
        Dictionary<int, ImageTexture> iconDB = new();
        public static Icons Instance { get; } = new();

        public static ImageTexture GetIcon(int itemId)
        {
            if (Instance.iconDB.TryGetValue(itemId, out var icon) == false)
            {
                var item = Cache.Items.GetItem(itemId);
                var iconBytes = Main.MyClient.WebApi.Render.DownloadToByteArrayAsync(item.Icon.Url).Result;

                var image = new Image();
                image.LoadPngFromBuffer(iconBytes);
                icon = ImageTexture.CreateFromImage(image);
                Instance.iconDB.Add(itemId, icon);
            }

            return icon;
        }
    }
}