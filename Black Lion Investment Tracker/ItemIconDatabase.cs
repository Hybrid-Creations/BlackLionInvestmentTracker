using System.Collections.Generic;
using BLIT.UI;
using Godot;
using Gw2Sharp.WebApi.V2.Models;

namespace BLIT;

public static class Cache
{
    public class Items
    {
        static Dictionary<int, Item> itemDB = new();

        public static Item GetItem(int itemId)
        {
            if (itemDB.TryGetValue(itemId, out var item) == false)
            {
                item = Main.MyClient.WebApi.V2.Items.GetAsync(itemId).Result;
                itemDB.Add(itemId, item);
            }

            return item;
        }
    }

    public class Icons
    {
        static Dictionary<int, ImageTexture> iconDB = new();

        public static ImageTexture GetIcon(int itemId)
        {
            if (iconDB.TryGetValue(itemId, out var icon) == false)
            {
                var item = Cache.Items.GetItem(itemId);
                var iconBytes = Main.MyClient.WebApi.Render.DownloadToByteArrayAsync(item.Icon.Url).Result;

                var image = new Image();
                image.LoadPngFromBuffer(iconBytes);
                icon = ImageTexture.CreateFromImage(image);
                iconDB.Add(itemId, icon);
            }

            return icon;
        }
    }
}