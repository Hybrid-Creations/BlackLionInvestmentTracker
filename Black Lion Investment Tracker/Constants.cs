using Godot;

namespace BLIT.ConstantVariables;

public static class Constants
{
    public static readonly int MaxItemStack = 250;
    public static readonly double MultiplyInverseTax = 1.17647;
    public const string EmptyItemPropertyEntry = "[right]-----[/right]";

    public static int ApplySellingFees(int inPrice)
    {
        var listingFee = Mathf.Max(1, Mathf.RoundToInt(inPrice * 0.05f));
        var exchangeFee = Mathf.Max(1, Mathf.RoundToInt(inPrice * 0.1f));

        return inPrice - listingFee - exchangeFee;
    }
}

public enum RichStringAlignment
{
    LEFT,
    CENTER,
    RIGHT,
}

public enum RichImageType
{
    NONE,
    PX18,
    PX32,
    PX64,
    PX256
}
