using Godot;
using System;
using System.Globalization;

namespace Vertigo
{
    public static class LabelExtensions
    {
        public static void SetOverrideFontSize(this Label label, int fontSize)
        {
            label.Set("font_size", fontSize);
        }
    }
}