using Godot;
using System;

namespace Vertigo
{
    public class GUIHeart : Control
    {
        private AnimationPlayer _animationPlayer;

        public override void _Ready()
        {
            base._Ready();

            _animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
            _animationPlayer.Play("Appear");
        }

        public void Delete()
        {
            _animationPlayer.Play("Deletion");
        }

        public void OnDeletionFinished()
        {
            this.SafeQueueFree();
        }

        public void PlayHealSound()
        {
            Game.Instance.PlayHealSound();
        }

        public void PlayHeartbreakSound()
        {
            Game.Instance.PlayHeartbreakSound();
        }
    }
}