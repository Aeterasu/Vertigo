using Godot;
using System;

namespace Vertigo
{
    public class PlayerGrazeFx : Node
    {
        private Sprite _sprite;
        private CPUParticles2D _particles;

        private Color _spriteTargetModulate = new Color(1f, 1f, 1f, 1f);

        public bool IsEnabled { get; set; } = false;

        public override void _Ready()
        {
            base._Ready();

            _sprite = GetNode<Sprite>("Sprite");
            _particles = GetNode<CPUParticles2D>("CPUParticles2D");

            _spriteTargetModulate = _sprite.Modulate;
            _sprite.Modulate = new Color(1f, 1f, 1f, 0f);
            _particles.Emitting = false;
        }

        public override void _PhysicsProcess(float delta)
        {
            base._PhysicsProcess(delta);

            if (IsEnabled)
            {
                _sprite.Modulate = _sprite.Modulate.LinearInterpolate(_spriteTargetModulate, 6f * delta);
            }
            else
            {
                _sprite.Modulate = _sprite.Modulate.LinearInterpolate(new Color(1f, 1f, 1f, 0f), 6f * delta);
            }

            _particles.Emitting = IsEnabled;
        }
    }
}