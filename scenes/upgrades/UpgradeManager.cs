using Godot;
using System;
using System.Collections.Generic;

namespace Vertigo
{
    public class UpgradeManager : Node
    {
        [Export] public List<UpgradeCard> UpgradePool { get; private set; } = new List<UpgradeCard>();

        public static UpgradeManager Instance { get; private set; }

        public override void _Ready()
        {
            base._Ready();

            Instance = this;
        }

        public void AddUpgrade(UpgradeCard card)
        {
            Node node = card.UpgradeScene.Instance();

            if (!(node is Upgrade))
            {
                node.SafeQueueFree();
                return;
            }
            
            AddChild(node);
        }
    }
}
