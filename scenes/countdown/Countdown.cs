using Godot;
using System;

namespace Vertigo
{
    public class Countdown : Control
    {
        [Export] private readonly float _animationFinishThreshold = 4.5f;
        [Export] private readonly float _speedUpRatio = 3f;

        private AnimationPlayer _animationPlayer;

        private float _ticks = 0;

        private bool _animLock = false;

        public event Action OnCountdownFinished;

        public override void _Ready()
        {
            base._Ready();

            _animationPlayer = GetNode<AnimationPlayer>("%AnimationPlayer");

            _animationPlayer.Play("Countdown");
        }

        public override void _PhysicsProcess(float delta)
        {
            base._PhysicsProcess(delta);

            _ticks += delta;

            if (_animationPlayer.CurrentAnimationPosition >= _animationFinishThreshold && !_animLock)
            {
                OnAnimationFinished("Countdown");
                _animLock = true;
            }
        }

        public override void _Input(InputEvent @event)
        {
            base._Input(@event);

            if (!(_animationPlayer.CurrentAnimationPosition < 3.6f && _ticks >= 0.3f))
            {
                return;
            }

            if (@event is InputEventKey || @event is InputEventJoypadButton || @event is InputEventMouseButton)
            {
                if (@event.IsPressed())
                {
                    _animationPlayer.PlaybackSpeed = _speedUpRatio;
                }
            }
        }

        private void OnAnimationFinished(string animName)
        {
            if (animName == "Countdown")
            {
                OnCountdownFinished?.Invoke();
            }
        }
    }
}