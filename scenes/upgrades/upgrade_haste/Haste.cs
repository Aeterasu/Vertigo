using Godot;
using System;

namespace Vertigo
{
    public class Haste : Upgrade
    {
        [Export] private readonly float _initalIncrease = 0.2f;
        [Export] private readonly float _increasePerStack = 0.1f;

        private Player _player;
        private PowerComponent _playerPowerComponent;

        public float TotalIncrease { get; private set; } = 1.0f;

        public override void _Ready()
        {
            base._Ready();

            TotalIncrease += _initalIncrease;

            _player = GetNodeOrNull<Player>("%Player");

            if (!_player.IsValid())
            {
                return;
            }

            _playerPowerComponent = _player.GetNodeOrNull<PowerComponent>("%PowerComponent");
        }

        public override void _PhysicsProcess(float delta)
        {
            base._PhysicsProcess(delta);

            _playerPowerComponent.HasteRegenSpeedMultiplier = TotalIncrease;
        }

        public override void AddStack()
        {
            base.AddStack();

            TotalIncrease += _increasePerStack;
        }
    }
}
