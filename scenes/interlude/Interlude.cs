using Godot;
using System;

namespace Vertigo
{
    public class Interlude : Control
    {
        private AnimationPlayer _animationPlayer;

        public override void _Ready()
        {
            base._Ready();

            _animationPlayer = GetNode<AnimationPlayer>("%AnimationPlayer");
            _animationPlayer.Play("Interlude");
        }
    }
}