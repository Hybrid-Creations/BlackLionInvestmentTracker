using System;
using BLIT.Investments;
using BLIT.UI;
using Godot;

public partial class RestorePopup : Control
{
    Main main;

    public event Action OnPopupCancelled;
    public event Action OnPopupAccepted;

    public void Setup(Main _main)
    {
        main = _main;
    }

    public void Cancel()
    {
        OnPopupCancelled?.Invoke();

        QueueFree();
    }

    public void Accept()
    {
        InvestmentsDatabase.RestoreMostRecentBackup();
        main.ReloadAndRefresh();

        OnPopupAccepted?.Invoke();

        QueueFree();
    }
}
