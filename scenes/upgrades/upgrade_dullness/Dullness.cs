using Godot;
using System;

namespace Vertigo
{
    // DULLNESS - slow down hazards on hit
    public class Dullness : Upgrade
    {
        [Export] private float _initialSlowdown = 0.2f;
        [Export] private float _slowdownPerStack = 0.08f;

        public float TotalSlowdown { get; private set; } = 1.0f;

        public static Dullness Instance { get; private set; }

        public override void _Ready()
        {
            base._Ready();

            Instance = this;

            TotalSlowdown += _initialSlowdown;
        }

        public override void AddStack()
        {
            TotalSlowdown += _slowdownPerStack;
        }
    }
}