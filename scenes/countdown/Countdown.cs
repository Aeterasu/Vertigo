using Godot;
using System;

namespace Vertigo
{
    public class Countdown : Control
    {
        [Export] private readonly float _animationFinishThreshold = 4.5f;

        private AnimationPlayer _animationPlayer;

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

            if (_animationPlayer.CurrentAnimationPosition >= _animationFinishThreshold && !_animLock)
            {
                OnAnimationFinished("Countdown");
                _animLock = true;
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