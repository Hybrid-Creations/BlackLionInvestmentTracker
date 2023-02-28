using System;
using System.Collections.Generic;
using System.Linq;

namespace BLIT;

// public class CollapsedPendingInvestmentData : CollapsedInvestmentData<PendingInvestmentData>
// {
// public Lazy<int> CurrentIndividualSellPrice;

// public int TotalPotentialSellPrice => SubInvestments.Sum(i => i.TotalPotentialSellPrice);
// // The sub investments already account for the BLTP 15% tax
// public int TotalPotentialProfit => SubInvestments.Sum(i => i.PotentialProfit);

// public CollapsedPendingInvestmentData(PendingInvestmentData firstInvestment, Lazy<int> currentSellPrice)
// {
//     ItemId = firstInvestment.ItemId;
//     IndividualPrice = firstInvestment.IndividualPrice;
//     SubInvestments = new List<PendingInvestmentData>
//     {
//         firstInvestment
//     };
//     CurrentIndividualSellPrice = currentSellPrice;
// }
// }