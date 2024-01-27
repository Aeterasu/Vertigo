using Godot;
using System;

namespace Vertigo
{
    public class Passion : Upgrade
    {
        private Player _player;
        private HealthComponent _playerHealthComponent;

        public override void _Ready()
        {
            base._Ready();

            AddHealth();
        }

        public override void AddStack()
        {
            AddHealth();
        }

        private void AddHealth()
        {
            if (!Game.Instance.IsValid())
            {
                return;
            }

            _player = Game.Instance.GetNodeOrNull<Player>("%Player");

            if (!_player.IsValid())
            {
                return;
            }
            _playerHealthComponent = _player.GetNodeOrNull<HealthComponent>("%HealthComponent");

            if (_playerHealthComponent.IsValid())
            {
                _playerHealthComponent.Health += 1;
            }
        }
    }
}
