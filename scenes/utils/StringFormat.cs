using Godot;
using System;

public static class StringFormat
{
    public static string SecondsToStringFormatted(double seconds)
    {
            double secondsRounded = Math.Round(seconds);
            double milliseconds = Math.Floor((Math.Round(seconds, 2) - Math.Floor(seconds)) * 100);

            string formattedMilliseconds = milliseconds.ToString();

            if (milliseconds > 99)
            {
                formattedMilliseconds = "99";
            }
            else if (milliseconds < 10)
            {
                formattedMilliseconds = $"0{milliseconds}";
            }

            string formattedSeconds;

            if (secondsRounded < 10)
            {
                formattedSeconds = $"0{secondsRounded}";
            }
            else
            {
                formattedSeconds = secondsRounded.ToString();
            }

            return $"{formattedSeconds}.{formattedMilliseconds}";
    }
}