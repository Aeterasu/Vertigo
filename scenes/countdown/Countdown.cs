using Godot;
using System;

namespace Vertigo
{
    public class Countdown : Control
    {
        private AnimationPlayer _animationPlayer;

        private bool _animLock = false;

        public event Action OnCountdownFinished;

        public override void _Ready()
        {
            base._Ready();

            _animationPlayer = GetNode<AnimationPlayer>("%AnimationPlayer");

            _animationPlayer.Play("Countdown");

            //_animationPlayer.Connect("animation_finished", this, "OnAnimationFinished");
        }

        public override void _PhysicsProcess(float delta)
        {
            base._PhysicsProcess(delta);

            if (_animationPlayer.CurrentAnimationPosition >= 4.5f && !_animLock)
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