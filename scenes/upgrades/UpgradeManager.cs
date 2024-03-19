using Godot;
using System;
using System.Collections.Generic;

namespace Vertigo
{
    public class UpgradeManager : Node
    {
        [Export] public List<UpgradeCard> UpgradePool { get; private set; } = new List<UpgradeCard>();

        public List<int> UpgradeStacks { get; private set; } = new List<int>();

        public UpgradeCard LastNonPassionUpgradeCard { get; private set; } = null;

        public static UpgradeManager Instance { get; private set; }

        public override void _Ready()
        {
            base._Ready();

            Instance = this;

            foreach (UpgradeCard card in UpgradePool)
            {
                UpgradeStacks.Add(1);
            }
        }

        public void AddUpgrade(UpgradeCard card, bool checkForPassionOverflow = false)
        {
            Node node = card.UpgradeScene.Instance();

            if (!(node is Upgrade))
            {
                node.SafeQueueFree();
                return;
            }

            if (!(node is Passion))
            {
                LastNonPassionUpgradeCard = card;
            }

            foreach (Node child in GetChildren())
            {
                if (child.GetType() == node.GetType())
                {
                    (child as Upgrade).AddStack();
                    node.SafeQueueFree();
                }

                if (child is Aegis)
                {
                    (child as Aegis).ActivateImmunity();
                }
            }
            
            if (node.IsValid())
            {
                AddChild(node);
            }

            for (int i = 0; i < UpgradePool.Count; i++)
            {
                if (i == UpgradePool.IndexOf(card))
                {
                    UpgradeStacks[i] += 1;
                }
            }
        }
    }
}
