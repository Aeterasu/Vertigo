using Godot;
using System;

namespace Vertigo
{
    public class PlayerGun : Node2D
    {
        [Export] private readonly PackedScene _attackScene;
        [Export] private readonly float _rateOfFire = 0.3f;
        [Export] private readonly float _mashingRateOfFire = 0.05f;
        [Export] private readonly float _spread = 8f;
        [Export] public float VerticalStall { get; private set; } = -0.8f;
        [Export] public float HorizontalKnockback { get; private set; } = 8f;
        [Export] public float ScreenShakeStrength { get; private set; } = 2f;
        [Export] public float PowerConsumption { get; private set; } = 1f;
        [Export] private readonly AudioStream _shotAudio;
        [Export] private readonly AudioStream _superShotAudio;

        private float _fireCooldown = 0f;
        private float _mashingCooldown = 0f;

        public event Action OnFired;

        public Abundancy Abundancy { get; set; } = null;

        public override void _Ready()
        {
            base._Ready();
        }

        public override void _PhysicsProcess(float delta)
        {
            base._PhysicsProcess(delta);

            _fireCooldown = Mathf.Max(_fireCooldown - delta, 0f);
            _mashingCooldown = Mathf.Max(_mashingCooldown - delta, 0f);
        }

        public void ResetCooldownOnMash()
        {
            if (_fireCooldown > _mashingCooldown)
            {
                _fireCooldown = _mashingCooldown;
            }
            else
            {
                _fireCooldown = 0f;
            }
        }

        public void Fire(float directionAngleDegrees, PowerComponent power, int projectileCount = 1)
        {
            if (_attackScene == null)
            {
                return;
            }

            if (power.CurrentPower < PowerConsumption)
            {
                return;
            }

            if (_fireCooldown > 0f)
            {
                return;
            }

            if (Abundancy.IsValid())
            {
                if (Abundancy.IsActive)
                {
                    projectileCount = Abundancy.AdditionalProjectiles + 1;
                }
            }

            if (projectileCount % 2 != 0)
            {
                for (int i = 0; i < projectileCount; i++)
                {
                    CreateAttack(directionAngleDegrees + (i - (projectileCount / 2)) * _spread);
                }
            }
            else
            {
                for (int i = 0; i < projectileCount + 1; i++)
                {
                    if (i != projectileCount / 2)
                    {
                        CreateAttack(directionAngleDegrees + (i - (projectileCount / 2)) * _spread / 2);
                    }
                }
            }
            
            _fireCooldown = _rateOfFire;
            _mashingCooldown = _mashingRateOfFire;

            power.ConsumePower(PowerConsumption);

            OnFired?.Invoke();

            if (projectileCount == 1)
            {
                DeconflictedSoundPlayer.PlayUnconflictingSound(_shotAudio, this, 0f, 1f);
            }
            else if (projectileCount > 1)
            {
                DeconflictedSoundPlayer.PlayUnconflictingSound(_superShotAudio, this, 0f, 1f);
            }
        }

        private void CreateAttack(float directionAngleDegrees)
        {
            var attack = _attackScene.Instance();

            if (attack is Node2D)
            {
                (attack as Node2D).GlobalPosition = GlobalPosition;
                (attack as Node2D).RotationDegrees = directionAngleDegrees;
            }

            if (Game.Instance.IsValid() && Game.Instance.GameLayer.IsValid())
            {
                Game.Instance.GameLayer.AddChild(attack);
            }
        }
    }
}