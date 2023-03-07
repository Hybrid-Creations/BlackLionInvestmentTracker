using Godot;

namespace BLIT.Extensions;

public static class Vector2Extensions
{
    public static Vector2I ToVector2I(this Vector2 v2, RoundingType rounding = RoundingType.NEAREST_EVEN)
    {
        return rounding switch
        {
            RoundingType.CEIL => (Vector2I)v2.Ceil(),
            RoundingType.FLOOR => (Vector2I)v2.Floor(),
            // NEAREST_EVEN
            _ => new Vector2I(Mathf.RoundToInt(v2.X), Mathf.RoundToInt(v2.Y)),
        };
    }

    public enum RoundingType
    {
        NEAREST_EVEN,
        CEIL,
        FLOOR
    }
}