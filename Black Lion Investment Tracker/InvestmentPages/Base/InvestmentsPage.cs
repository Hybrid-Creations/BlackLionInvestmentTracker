using System;
using System.Collections.Generic;
using System.Linq;
using BLIT.Extensions;
using BLIT.Investments;
using Godot;
using Gw2Sharp.WebApi.Exceptions;

namespace BLIT.UI;

public partial class InvestmentsPage : VBoxContainer
{
    [Export]
    protected PackedScene collapsedInvestmentScene;
    [Export]
    protected VBoxContainer investmentHolder;
    [Export]
    protected Label loadingLabel;
}
