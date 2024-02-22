using Godot;
using System;

namespace Vertigo
{
    public class GameOverScreen : Control
    {
        private AnimationPlayer _animationPlayer;

        public GameResults GameResults { get; set; } = new GameResults(0);

        public override void _Ready()
        {
            base._Ready();

            _animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
            _animationPlayer.Play("Init");
        }
    }

}