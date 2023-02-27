using System.Collections.Concurrent;
using BLIT.UI;

namespace BLIT;

public static partial class Cache
{
    public class Prices
    {
        static ConcurrentDictionary<int, int> sellsDB = new();

        public static int GetPrice(int itemId)
        {
            if (sellsDB.TryGetValue(itemId, out var price) == false)
            {
                price = Main.MyClient.WebApi.V2.Commerce.Prices.GetAsync(itemId).Result.Sells.UnitPrice;
                sellsDB.TryAdd(itemId, price);
            }

            return price;
        }
    }
}
