using Godot;
using System;

namespace Vertigo
{
    public class Opulency : Upgrade
    {
        [Export] private readonly int _initialIncrease = 2;
        [Export] private readonly int _increasePerStack = 1;

        public static int AdditionalGems { get; private set; } = 0;

        public override void _Ready()
        {
            base._Ready();

            AdditionalGems += _initialIncrease;
        }

        public override void AddStack()
        {
            base.AddStack();

            AdditionalGems += _increasePerStack;
        }
    }
}
