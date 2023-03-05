
namespace BLIT.ConstantVariables;

public static class Constants
{
    public static readonly int MaxItemStack = 250;
    public static readonly double MultiplyTax = 0.85;
    public static readonly double MultiplyInverseTax = 1.17647;
    public const string EmptyItemPropertyEntry = "[right]-----[/right]";
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