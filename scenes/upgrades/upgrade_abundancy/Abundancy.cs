using Godot;
using System;

namespace Vertigo
{
    // ABUNDANCY - each 3 shots fire a triple shot. each stack adds an additional projectile.
    public class Abundancy : Upgrade
    {
        [Export] private readonly int _shotsRequired = 2;

        private int _currentShots = 1;

        private Player _player;
        private PlayerGun _playerGun;

        public int AdditionalProjectiles { get; private set; } = 2;
        public bool IsActive { get; private set; } = false;

        public override void _Ready()
        {
            base._Ready();

            _player = GetNodeOrNull<Player>("%Player");

            if (!_player.IsValid())
            {
                return;
            }

            _playerGun = _player.GetNodeOrNull<PlayerGun>("%PlayerGun");

            if (_playerGun.IsValid())
            {
                _playerGun.Abundancy = this;
                _playerGun.OnFired += OnGunFired;
            }
        }

        public override void AddStack()
        {
            base.AddStack();

            AdditionalProjectiles += 1;
        }

        private void OnGunFired()
        {
            if (_currentShots >= _shotsRequired)
            {
                _currentShots = 0;
                IsActive = true;
            }
            else
            {
                _currentShots += 1;
                IsActive = false;
            }
        }
    }
}
