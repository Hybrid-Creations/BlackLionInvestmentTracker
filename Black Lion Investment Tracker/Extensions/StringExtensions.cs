using System;
using System.Text;
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

    public static string ToCurrencyString(this int amount, bool richColoring) => ToCurrencyString((long)amount, richColoring);
    public static string ToCurrencyString(this long amount, bool richColoring)
    {
        var positive = amount > 0;
        var asStr = Mathf.Abs(amount).ToString("000000");
        var sb = new StringBuilder();
        var gold = asStr[0..2];
        var silver = asStr[2..4];
        var copper = asStr[4..6];

        var goldSuffix = $"{(richColoring ? "[color=gold]" : "")}g{(richColoring ? "[/color]" : "")}";
        var silverSuffix = $"{(richColoring ? "[color=silver]" : "")}s{(richColoring ? "[/color]" : "")}";
        var copperSuffix = $"{(richColoring ? "[color=orange]" : "")}c{(richColoring ? "[/color]" : "")}";

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
}
