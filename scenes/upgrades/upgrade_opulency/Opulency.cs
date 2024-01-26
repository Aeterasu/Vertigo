using Godot;
using System;

namespace Vertigo
{
    // OPULENCY - additional gems drop from enemies
    public class Opulency : Upgrade
    {
        [Export] private readonly int _initialIncrease = 2;
        [Export] private readonly int _increasePerStack = 1;

        public int AdditionalGems { get; private set; } = 0;

        public static Opulency Instance { get; private set; }

        public override void _Ready()
        {
            base._Ready();

            Instance = this;

            AdditionalGems += _initialIncrease;
        }

        public override void AddStack()
        {
            base.AddStack();

            AdditionalGems += _increasePerStack;
        }
    }
}
