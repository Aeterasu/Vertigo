using Godot;
using System;
using System.Collections.Generic;

namespace Vertigo
{
    public class UpgradeManager : Node
    {
        public static UpgradeManager Instance { get; private set; }

        public override void _Ready()
        {
            base._Ready();

            Instance = this;
        }
    }
}
