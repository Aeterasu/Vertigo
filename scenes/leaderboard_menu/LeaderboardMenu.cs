using Godot;
using System;

namespace Vertigo
{
    public class LeaderboardMenu : Control
    {
        [Export] private float _scrollSpeed = 2f;
        [Export] private float _lerpWeight = 8f;
        private float _targetYOffset = 0f;
        private float _currentYOffset = 0f;

        private Leaderboard _leaderboard;
        private Control _leaderboardPositions;
        private Vector2 _defaultRectPosition = Vector2.Zero;

        private float _alpha = 1f;

        private ColorRect _blackout;

        public override void _Ready()
        {
            base._Ready();

            _leaderboard = GetNode<Leaderboard>("Leaderboard");
            _leaderboardPositions = GetNode<Control>("Leaderboard/TopTenPositions");
            _defaultRectPosition = _leaderboardPositions.RectPosition;

            _leaderboard.Visible = true;

            _blackout = GetNode<ColorRect>("Blackout");

            Label label = GetNodeOrNull<Label>("Leaderboard/Backdrop2/Label");

            if (label.IsValid())
            {
                label.Text = Tr("LEADERBOARD_TEXT");
            }
        }

        public override void _PhysicsProcess(float delta)
        {
            base._PhysicsProcess(delta);

            _alpha = Mathf.Max(_alpha - 5f * delta, 0f);

            _blackout.Modulate = new Color(1, 1, 1, _alpha);

            _targetYOffset = Mathf.Clamp(_targetYOffset, 0, _leaderboard.TotalYOffset);

            _currentYOffset = Mathf.Lerp(_currentYOffset, _targetYOffset, _lerpWeight * delta);
            _leaderboardPositions.RectPosition = _defaultRectPosition + Vector2.Up * _currentYOffset;
        }

        public override void _Input(InputEvent @event)
        {
            base._Input(@event);

            if (@event is InputEventMouseButton)
            {
                InputEventMouseButton mouseEvent = @event as InputEventMouseButton;

                if (mouseEvent.ButtonIndex == 4) // 4 = BUTTON_WHEEL_UP 
                {
                    ScrollUp();
                }

                if (mouseEvent.ButtonIndex == 5) // 5 = BUTTON_WHEEL_DOWN  
                {
                    ScrollDown();
                }
            }
        }

        public void ScrollUp(float multiplier = 1.0f)
        {
            _targetYOffset -= _scrollSpeed * multiplier;
        }

        public void ScrollDown(float multiplier = 1.0f)
        {
            _targetYOffset += _scrollSpeed * multiplier;
        }
    }
}