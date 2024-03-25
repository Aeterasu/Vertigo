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

        private Control _gradient;

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

            _gradient = GetNode<Control>("Gradient");
            _gradient.Modulate = new Color(1, 1, 1, 0);

            GetNode<Label>("PowerLabel").Text = Tr("GUI_POWER");
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
            
            if (_playerPowerComponent.IsGrazing)
            {
                _gradient.Modulate = new Color(1, 1, 1, Mathf.Lerp(_gradient.Modulate.a, 1f, 8 * delta));
            }
            else
            {
                _gradient.Modulate = new Color(1, 1, 1, Mathf.Lerp(_gradient.Modulate.a, 0f, 8 * delta));
            }
        }
    }
}
