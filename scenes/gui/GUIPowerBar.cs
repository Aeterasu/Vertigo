using Godot;
using System;

namespace Vertigo
{
    public class GUIPowerBar : TextureProgress
    {
        private Player _player;
        private PowerComponent _playerPowerComponent;

        private TextureProgress _powerChargeBar;

        private CPUParticles2D _grazeParticles1;
        private CPUParticles2D _grazeParticles2;

        public override void _Ready()
        {
            base._Ready();

            _player = GetNodeOrNull<Player>("%Player");

            if (!_player.IsValid())
            {
                return;
            }

            _playerPowerComponent = _player.GetNodeOrNull<PowerComponent>("%PowerComponent");

            _powerChargeBar = GetNode<TextureProgress>("PowerChargeBar");

            _grazeParticles1 = GetNode<CPUParticles2D>("GrazeParticles1");
            _grazeParticles2 = GetNode<CPUParticles2D>("GrazeParticles2");
        }

        public override void _PhysicsProcess(float delta)
        {
            base._PhysicsProcess(delta);

            if (!_player.IsValid() || !_playerPowerComponent.IsValid())
            {
                return;
            }

            MinValue = 0f;
            MaxValue = _playerPowerComponent.MaxPower;
            Value = Mathf.Floor(_playerPowerComponent.CurrentPower);

            _powerChargeBar.MinValue = MinValue;
            _powerChargeBar.MaxValue = _playerPowerComponent.MaxPower;
            _powerChargeBar.Value = _playerPowerComponent.CurrentPower + _playerPowerComponent.CurrentRegen;

            _grazeParticles1.Emitting = _playerPowerComponent.IsGrazing;
            _grazeParticles2.Emitting = _playerPowerComponent.IsGrazing;
        }
    }
}
