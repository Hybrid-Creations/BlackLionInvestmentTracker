using System;
using System.Text;
using BLIT.ConstantVariables;
using Godot;

namespace BLIT.Extensions;

public static partial class StringExtensions
{
    /// <summary>
    /// Transforms a <see cref="DateTimeOffset"/> to a string representing elapsed time since, e.g. 3 months ago or 35 seconds ago
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static string ToTimeSinceString(this DateTimeOffset value)
    {
        var offset = DateTime.Now - value.ToLocalTime();

        var sb = new StringBuilder();

        // If there are any days
        if (offset.Days > 0)
        {
            // then check if its over a month
            if (offset.Days >= DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month))
            {
                int months = offset.Months(DateTime.Now);
                sb.Append($"{months} month{(months > 1 ? "s" : "")}");
            }
            // If not just use the days
            else
                sb.Append($"{offset.Days} day{(offset.Days > 1 ? "s" : "")}");
        }
        // There are no days so make sure there are hours
        else if (offset.Hours > 0)
        {
            sb.Append($"{offset.Hours} hour{(offset.Hours > 1 ? "s" : "")}");
        }
        // There are no hours so make sure there are minutes
        else if (offset.Minutes > 0)
        {
            sb.Append($"{offset.Minutes} minute{(offset.Minutes > 1 ? "s" : "")}");
        }
        // There are no minutes so use seconds
        else
        {
            sb.Append($"{offset.Seconds} second{(offset.Seconds > 1 ? "s" : "")}");
        }

        sb.Append(" ago");
        return sb.ToString();
    }

    /// [02]
    /// <summary>
    /// Same as <see cref="ToCurrencyString(long, bool)"/> except that it rounds the double first
    /// </summary>
    /// <param name="amount"></param>
    /// <param name="richImage"></param>
    /// <returns></returns>
    public static string ToCurrencyString(this double amount, RichImageType richImage, int imageSize = 16) => ToCurrencyString((long)Math.Round(amount), richImage, imageSize);

    /// [01]
    ///<inheritdoc cref="ToCurrencyString(long,bool)"/>
    public static string ToCurrencyString(this int amount, RichImageType richImage, int imageSize = 16) => ToCurrencyString((long)amount, richImage, imageSize);

    ///[00]
    /// <summary>
    /// Returns a string in the format of "XXXg XXs XXc", e.g. 1345 => "13s 45c", e.g. 1232453 => "123g 24s 53c", it also insterts images where the "g, s, c" are representing gold, silver, and copper.
    /// </summary>
    /// <param name="amount">The number to convert</param>
    /// <param name="richImage">The size of image to use, i.e. 16x16, 64x64.</param>
    /// <param name="imageSize">The size of the images when inlined in the text.</param>
    /// <returns></returns>
    public static string ToCurrencyString(this long amount, RichImageType richImage, int imageSize = 16)
    {
        var positive = amount >= 0;
        var asStr = Mathf.Abs(amount).ToString();
        var sb = new StringBuilder();
        var copper = $"{(asStr.Length > 1 ? asStr[^2] : "0")}{asStr[^1]}";
        var silver = $"{(asStr.Length > 3 ? asStr[^4] : "0")}{(asStr.Length > 2 ? asStr[^3] : "0")}";
        var gold = $"{(asStr.Length > 6 ? asStr[..^6] : "")}{(asStr.Length > 5 ? asStr[^6] : "0")}{(asStr.Length > 4 ? asStr[^5] : "0")}";

        var goldSuffix = $"{(richImage == RichImageType.NONE ? "" : $"[img={imageSize}]Sprites/gold_{richImage.ToString().Replace("PX", "")}.png[/img]")}";
        var silverSuffix = $"{(richImage == RichImageType.NONE ? "" : $"[img={imageSize}]Sprites/silver_{richImage.ToString().Replace("PX", "")}.png[/img]")}";
        var copperSuffix = $"{(richImage == RichImageType.NONE ? "" : $"[img={imageSize}]Sprites/copper_{richImage.ToString().Replace("PX", "")}.png[/img]")}";

        // Show gold if there is any
        if (gold != "00")
        {
            // remove pre 0
            if (gold[0] == '0')
                sb.Append($"{gold[1]}{goldSuffix} ");
            else
                sb.Append($"{gold}{goldSuffix} ");
        }

        // Show silver if there is any, and if there is gold you should always show silver
        if (silver != "00" || gold != "00")
        {
            // remove pre 0
            if (gold == "00" && silver[0] == '0')
                sb.Append($"{silver[1]}{silverSuffix} ");
            else
                sb.Append($"{silver}{silverSuffix} ");
        }

        // Always show coppper, remove pre 0
        if (gold == "00" && silver == "00" && copper[0] == '0')
            sb.Append($"{copper[1]}{copperSuffix}");
        else
            sb.Append($"{copper}{copperSuffix}");

        if (positive)
            return sb.ToString();
        else
        {
            sb.Insert(0, "-");
            return sb.ToString();
        }
    }

    /// <summary>
    /// Wraps the given string in the Alignment BBCode, with choice of alignment.
    /// </summary>
    /// <param name="str">The string to modify.</param>
    /// <param name="alignment">The alignment to use.</param>
    /// <returns></returns>
    public static string AlignRichString(this string str, RichStringAlignment alignment)
    {
        return $"{$"[{alignment}]".ToLower()}{str}{$"[{alignment}]".ToLower()}";
    }

    /// <summary>
    /// Wraps a string in the BBCode for coloring the text.
    /// </summary>
    /// <param name="str">The string to modify.</param>
    /// <param name="htmlColorString">The color in html format, i.e. #FFF821.</param>
    /// <returns></returns>
    public static string ColorRichString(this string str, string htmlColorString)
    {
        if (htmlColorString.StartsWith("#") == false)
            throw new NotImplementedException();
        return $"[color={htmlColorString}]{str}[/color]";
    }
}
