using Godot;
using System;

public static class StringFormat
{
    public static string SecondsToStringFormatted(double seconds)
    {
        if (seconds < 60f)
        {
            return TimeSpan.FromSeconds(seconds).ToString("ss'.'ff");
        }
        else if (seconds / 60.0f < 60f)
        {
            return TimeSpan.FromSeconds(seconds).ToString("mm':'ss'.'ff");
        }
        else
        {
            return TimeSpan.FromSeconds(seconds).ToString("hh':'mm':'ss'.'ff");
        }
    }
}