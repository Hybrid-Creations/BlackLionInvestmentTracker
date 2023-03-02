using System.Collections.Concurrent;
using BLIT.UI;

namespace BLIT;

public static partial class Cache
{
    public class Prices
    {
        static readonly ConcurrentDictionary<int, int> pricesDB = new();

        public static int GetPrice(int itemId)
        {
            if (pricesDB.TryGetValue(itemId, out var price) == false)
            {
                price = Main.MyClient.WebApi.V2.Commerce.Prices.GetAsync(itemId).Result.Sells.UnitPrice;
                pricesDB.TryAdd(itemId, price);
            }

            return price;
        }

        public static void Clear()
        {
            pricesDB.Clear();
        }
    }
}
