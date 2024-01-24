using Godot;
using System;

namespace Vertigo
{
    public class PlayerGun : Node2D
    {
        [Export] private readonly PackedScene _attackScene;
        [Export] private readonly float _rateOfFire = 0.3f;
        [Export] public float VerticalStall { get; private set; } = -0.8f;
        [Export] public float HorizontalKnockback { get; private set; } = 8f;
        [Export] public float ScreenShakeStrength { get; private set; } = 2f;
        [Export] public float PowerConsumption { get; private set; } = 1f;
        [Export] private readonly AudioStream _shotAudio;

        private float _fireCooldown = 0f;

        public event Action OnFired;

        public override void _Ready()
        {
            base._Ready();
        }

        public override void _PhysicsProcess(float delta)
        {
            base._PhysicsProcess(delta);

            _fireCooldown = Mathf.Max(_fireCooldown - delta, 0f);
        }

        public void Fire(float directionAngleDegrees, PowerComponent power)
        {
            if (_attackScene == null || _fireCooldown > 0f)
            {
                return;
            }

            if (power.CurrentPower < PowerConsumption)
            {
                return;
            }

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

            _fireCooldown = _rateOfFire;

            power.ConsumePower(PowerConsumption);

            OnFired?.Invoke();

            DeconflictedSoundPlayer.PlayUnconflictingSound(_shotAudio, this, 0f, 1f);
        }
    }
}