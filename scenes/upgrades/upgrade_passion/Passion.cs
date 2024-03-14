using Godot;
using System;

namespace Vertigo
{
    public class Passion : Upgrade
    {
        [Export] private bool _enableBlankEffect = false;

        private Player _player;
        private HealthComponent _playerHealthComponent;

        public static Passion Instance { get; private set; }

        public override void _Ready()
        {
            base._Ready();

            if (!Instance.IsValid())
            {
                Instance = this;
            }

            AddHealth();

            if (_enableBlankEffect)
            {
                _player = Game.Instance.GetNodeOrNull<Player>("%Player");

                if (!_player.IsValid())
                {
                    return;
                }

                _player.OnPlayerHit += OnBlankEffect;  
            }
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

        private void OnBlankEffect()
        {
            _player = Game.Instance.GetNodeOrNull<Player>("%Player");

            if (!_player.IsValid())
            {
                return;
            }

            _player.AnimateShockwaveEffect();

            foreach (Area2D area in _player.GetBlankArea.GetOverlappingAreas())
            {
                if (area is Square)
                {
                    (area as Square).Destroy();
                }
            }
        }
    }
}
