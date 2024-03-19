using Godot;
using System;

namespace Vertigo
{
    public class UpgradeCard : Resource
    {
        [Export] public PackedScene UpgradeScene { get; private set; }
        [Export] public int TextureAtlasId { get; private set; }
        [Export] public string NameKey { get; private set; }
        [Export] public string DescriptionKey { get; private set; }
        [Export] public string StackDescriptionKey { get; private set; }
        [Export] public bool ShowLastNonPassionUpgradeName { get; private set; } = false;
    }
}