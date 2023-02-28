using System;
using System.Collections.Generic;
using System.Linq;

namespace BLIT;

// public class CollapsedInvestmentData<IvestmentData>
// {
//     public int ItemId;
//     public int IndividualPrice;

//     public int IndividualSellPrice => SubInvestments.First().SellDatas.First().IndividualPrice;
//     public int Quantity => SubInvestments.Sum(s => s.Quantity);
//     public int TotalBuyPrice => SubInvestments.Sum(i => i.TotalBuyPrice);
//     public int TotalSellPrice => SubInvestments.Sum(i => i.TotalSellPrice);
//     // The sub investments already account for the BLTP 15% tax
//     public int TotalProfit => SubInvestments.Sum(i => i.Profit);
//     public DateTimeOffset OldestPurchaseDate => SubInvestments.OrderByDescending(i => i.PurchaseDate).First().PurchaseDate;
//     public DateTimeOffset LatestSellDate => SubInvestments.SelectMany(i => i.SellDatas).OrderBy(d => d.SellDate).First().SellDate;

//     public virtual List<InvestmentData> SubInvestments { get; } = new();

//     public CollapsedInvestmentData(InvestmentData firstInvestment)
//     {
//         ItemId = firstInvestment.ItemId;
//         IndividualPrice = firstInvestment.IndividualPrice;
//         SubInvestments = new List<InvestmentData>
//         {
//             firstInvestment
//         };
//     }
// }
