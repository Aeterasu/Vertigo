using Godot;
using System;

namespace Vertigo
{
    public class LeaderboardPosition : Control
    {   
        [Export] private Color _highlightColor = new Color(1, 1, 1, 1);

        private Label _playerNameLabel;
        private Label _scoreLabel;

        private TextureRect _gradient;

        public int Position { get; set; } = 1;
        public string PlayerName { get; set; } = "";
        public float Score { get; set; } = 0.0f;

        public override void _Ready()
        {
            base._Ready();

            _playerNameLabel = GetNode<Label>("PlayerNameLabel");
            _scoreLabel = GetNode<Label>("ScoreLabel");

            _gradient = GetNode<TextureRect>("Gradient");
            _gradient.Visible = false;
        }

        public override void _PhysicsProcess(float delta)
        {
            base._PhysicsProcess(delta);

            if (!_playerNameLabel.IsValid() || !_scoreLabel.IsValid())
            {
                return;
            }

            if (PlayerName == "" && Score == 0.0f)
            {
                Visible = false;
                return;
            }
            else
            {
                Visible = true;
            }

            _playerNameLabel.Text = $"{Position}. {PlayerName}";
            _scoreLabel.Text = StringFormat.SecondsToStringFormatted(Score);
        }

        public void Highlight()
        {
            _playerNameLabel.Modulate = _highlightColor;
            _scoreLabel.Modulate = _highlightColor;
            _gradient.Visible = true;
        }
    }
}
