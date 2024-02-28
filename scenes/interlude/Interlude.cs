using Godot;
using System;

namespace Vertigo
{
    public class Interlude : Control
    {
        private float _delay = 0.1f;

        private bool _isSkipped = false;

        private AnimationPlayer _animationPlayer;

        public event Action OnInterludeAnimationFinished;

        public override void _Ready()
        {
            base._Ready();

            _animationPlayer = GetNode<AnimationPlayer>("%AnimationPlayer");
            _animationPlayer.Play("Interlude");
            _animationPlayer.Connect("animation_finished", this, "OnAnimationFinished");
        }

        public override void _PhysicsProcess(float delta)
        {
            base._PhysicsProcess(delta);

            _delay = Mathf.Max(_delay - delta, 0f);
        }

        public override void _Input(InputEvent @event)
        {
            base._Input(@event);

            if (_delay > 0.0f)
            {
                return;
            }

            if (@event is InputEventKey || @event is InputEventJoypadButton || @event is InputEventMouseButton)
            {
                if (!_isSkipped)
                {
                    _animationPlayer.Play("Skip");
                }
            }
        }


        private void OnAnimationFinished(string animName)
        {
            OnInterludeAnimationFinished?.Invoke();
        }
    }
}