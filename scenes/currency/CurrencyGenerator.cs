using Godot;
using System;

namespace Vertigo
{
    public class CurrencyGenerator : Node2D
    {
        [Export] private int _amount = 3;

        public override void _Ready()
        {
            base._Ready();
        }

        public void Generate()
        {
            int totalAmount = _amount;

            if (Opulency.Instance.IsValid())
            {
                totalAmount += Opulency.Instance.AdditionalGems;
            }

            for (int i = 0; i < totalAmount; i++)
            {
                var diamond = Game.DiamondScene.Instance() as Diamond;
                diamond.GlobalPosition = GlobalPosition + new Vector2((float)GD.RandRange(-8, 8), (float)GD.RandRange(-8, 8));

                if (Game.Instance.IsValid() && Game.Instance.GameLayer.IsValid())
                {
                    Game.Instance.GameLayer.CallDeferred("add_child", diamond);
                }
            }
        }
    }
}