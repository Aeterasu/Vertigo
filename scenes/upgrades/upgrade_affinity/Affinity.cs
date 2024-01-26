using Godot;
using System;

namespace Vertigo
{
    // AFFINITY - increases the scale of graze areas.
    public class Affinity : Upgrade
    {
        [Export] private float _initialIncrease = 0.3f;
        [Export] private float _increasePerStack = 0.15f;

        public float TotalIncrease { get; private set; } = 1.0f;

        public static Affinity Instance { get; private set; }

        public override void _Ready()
        {
            base._Ready();

            Instance = this;

            TotalIncrease += _initialIncrease;
        }

        public override void AddStack()
        {
            TotalIncrease += _increasePerStack;
        }
    }
}