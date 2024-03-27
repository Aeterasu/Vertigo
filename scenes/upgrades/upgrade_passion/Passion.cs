using Godot;
using System;

namespace Vertigo
{
    public class Passion : Upgrade
    {
        [Export] private readonly bool _enableBlankEffect = false;
        [Export] private readonly float _blankEffectDuration = 1.0f;
        [Export] private readonly uint _damageToBoss = 12;

        private float _blankEffectTicks = 0.0f;

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

        public override void _PhysicsProcess(float delta)
        {
            base._PhysicsProcess(delta);

            if (!_enableBlankEffect)
            {
                return;
            }

            _blankEffectTicks = Mathf.Max(_blankEffectTicks - delta, 0.0f);

            if (_blankEffectTicks > 0)
            {
                foreach (Area2D area in _player.GetBlankArea.GetOverlappingAreas())
                {
                    if (area is Square && !(area is Boss))
                    {
                        (area as Square).Destroy();
                    }
                }                
            }
        }

        public override void AddStack()
        {
            AddHealth();

            if (!UpgradeManager.Instance.IsValid())
            {
                return;
            }

            if (UpgradeManager.Instance.LastNonPassionUpgradeCard == null)
            {
                return;
            }

            UpgradeManager.Instance.AddUpgrade(UpgradeManager.Instance.LastNonPassionUpgradeCard);
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
            foreach (Area2D area in _player.GetBlankArea.GetOverlappingAreas())
            {
                if (area is Boss)
                {
                    (area as Boss).GetNode<HealthComponent>("%HealthComponent").DealDamage((int)_damageToBoss);
                }
            }  

            _player = Game.Instance.GetNodeOrNull<Player>("%Player");

            if (!_player.IsValid())
            {
                return;
            }

            _player.AnimateShockwaveEffect();

            _blankEffectTicks = _blankEffectDuration;
        }
    }
}
