using System.Collections.Generic;
using System.Linq;
using Godot;
using Newtonsoft.Json;

namespace BLIT;

public partial class InvestmentsDatabase
{
    public List<InvestmentData> Investments { get; private set; } = new List<InvestmentData>();
    public List<long> NotInvestments { get; private set; } = new List<long>();

    [JsonIgnore] public List<CollapsedInvestmentData> CollapsedInvestments { get; private set; } = new List<CollapsedInvestmentData>();
    [JsonIgnore] public long TotalInvested => Investments.Sum(i => i.TotalBuyPrice);
    [JsonIgnore] public long TotalReturn => Investments.SelectMany(i => i.SellDatas).Sum(s => s.TotalSellPrice);
    [JsonIgnore] public long TotalProfit => Investments.Sum(i => i.Profit);
    [JsonIgnore] public double ROI => TotalProfit / (double)TotalInvested * 100;

    public void GenerateCollapsed()
    {
        List<CollapsedInvestmentData> groups = new();

        foreach (var investment in Investments)
        {
            var readyGroup = groups.FirstOrDefault(ci => ci.ItemId == investment.ItemId && ci.IndividualPrice == investment.IndividualPrice && ci.Quantity + investment.Quantity <= Constants.MaxItemStack);
            if (readyGroup is not null)
            {
                readyGroup.SubInvestments.Add(investment);
            }
            else
            {
                var newCollapsedInvestment = new CollapsedInvestmentData(investment);
                groups.Add(newCollapsedInvestment);
            }
        }

        groups.ForEach(c => CollapsedInvestments.Add(c));
    }
}

public static partial class Saving
{
    const string databasePath = "user://database";

    public static void SaveDatabase(InvestmentsDatabase database)
    {
        using var dbFile = FileAccess.Open(databasePath, FileAccess.ModeFlags.Write);
        dbFile.StoreString(JsonConvert.SerializeObject(database));
    }

    public static bool TryLoadDatabase(out InvestmentsDatabase database)
    {
        database = new InvestmentsDatabase();
        if (FileAccess.FileExists(databasePath))
        {
            using var dbFile = FileAccess.Open(databasePath, FileAccess.ModeFlags.Read);
            database = JsonConvert.DeserializeObject<InvestmentsDatabase>(dbFile.GetAsText());
            return true;
        }
        else return false;
    }
}
