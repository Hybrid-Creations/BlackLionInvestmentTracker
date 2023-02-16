using System;
using System.Collections.Generic;
using BLIT;
using Godot;

public partial class MainTabs : TabContainer
{
    [Export]
    Investments Investments;

    [Export]
    Listings Listings;

    public void OnSelectTab(int tabIndex)
    {
        switch (tabIndex)
        {
            default:
            case 0:
                Investments.Continue();
                Listings.Pause();
                break;
            case 1:
                Listings.Continue();
                Investments.Pause();
                break;
        }
    }
}
