using Godot;
using System;

namespace Vertigo
{
    public class ColorScheme : Resource
    {
        [Export] public GradientTexture2D Gradient { get; private set; }
        [Export] public string NameKey { get; private set; } = "";
        [Export] public string UpgradeFluffTextKey { get; private set; } = "";
    }

}