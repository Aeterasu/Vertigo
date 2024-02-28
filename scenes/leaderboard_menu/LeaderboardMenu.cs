using Godot;
using System;

namespace Vertigo
{
    public class LeaderboardMenu : Control
    {
        private Leaderboard _leaderboard;

        private float _alpha = 1f;

        private ColorRect _blackout;

        public override void _Ready()
        {
            base._Ready();

            _leaderboard = GetNode<Leaderboard>("Leaderboard");

            _leaderboard.Visible = true;

            _blackout = GetNode<ColorRect>("Blackout");
        }

        public override void _PhysicsProcess(float delta)
        {
            base._PhysicsProcess(delta);

            _alpha = Mathf.Max(_alpha - 5f * delta, 0f);

            _blackout.Modulate = new Color(1, 1, 1, _alpha);
        }
    }
}