using System;
using System.Collections.Generic;
using System.Linq;
using BLIT.Extensions;
using BLIT.Investments;
using Godot;
using Gw2Sharp.WebApi.Exceptions;

namespace BLIT.UI;

public partial class InvestmentPage<TCollapedInvestment, TInvestment, TInvestmentData, TBuyData, TSellData, TCollapedInvestmentItemScene, TinvestmentItemScene> : VBoxContainer
 where TCollapedInvestment : CollapsedInvestment<TInvestment, TInvestmentData, TBuyData, TSellData>
 where TInvestment : Investment<TInvestmentData, TBuyData, TSellData>
 where TInvestmentData : InvestmentData<TBuyData, TSellData>
 where TBuyData : BuyData
where TSellData : SellData
where TCollapedInvestmentItemScene : CollapsedInvestmentItem<TCollapedInvestment, TInvestment, TInvestmentData, TBuyData, TSellData, TinvestmentItemScene>
where TinvestmentItemScene : InvestmentItem<TInvestment, TInvestmentData, TBuyData, TSellData>
{
    [Export]
    protected PackedScene collapsedInvestmentScene;
    [Export]
    protected VBoxContainer investmentHolder;
    [Export]
    protected Label loadingLabel;

    protected void ListInvestmentDatas(List<TCollapedInvestment> investmentDatas, string baseStatusMessage)
    {
        int index = 0;
        AppStatusIndicator.ShowStatus($"{baseStatusMessage} ({index}/{investmentDatas.Count})");
        // Add New Investment Items To UI
        foreach (var investment in investmentDatas.OrderBy(ci => ci.OldestPurchaseDate))
        {
            try
            {
                var instance = collapsedInvestmentScene.Instantiate<TCollapedInvestmentItemScene>();
                instance.Init(Cache.Items.GetItemData(investment.ItemId), investment);
                investmentHolder.AddChildSafe(instance, 0);
            }
            catch (NotFoundException)
            {
                // Most likely a new item that Gw2Sharp doesn't understand so we'll just skip it
                GD.PushWarning($"Failed to retreive info on item {investment.ItemId}, most likely Gw2Sharp has not been updated yet to handle the item");
            }
            catch (Exception e)
            {
                GD.PushError(e);
            }
            AppStatusIndicator.ShowStatus($"{baseStatusMessage} ({index}/{investmentDatas.Count})");
            index++;
        }
        AppStatusIndicator.ShowStatus($"{baseStatusMessage} ({investmentDatas.Count}/{investmentDatas.Count})");
    }
}
