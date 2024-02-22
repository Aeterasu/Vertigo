using Godot;
using System;

namespace Vertigo
{
    public class GUITimer : Control
    {
        [Export] public bool Enabled = true;

        private TimeManager _timeManager;
        
        private Label _label;

        public override void _Ready()
        {
            base._Ready();

            _timeManager = GetNodeOrNull<TimeManager>("%TimeManager");

            _label = GetNode<Label>("Label");
        }

        public override void _PhysicsProcess(float delta)
        {
            base._PhysicsProcess(delta);

            if (!Enabled)
            {
                return;
            }

            if (!_timeManager.IsValid())
            {
                return;
            }

            double seconds = _timeManager.GetSecElapsed;
            double secondsRounded = Math.Round(seconds);
            double milliseconds = Math.Floor((Math.Round(seconds, 2) - Math.Floor(seconds)) * 100);

            string formattedMilliseconds = milliseconds.ToString();

            if (milliseconds > 99)
            {
                formattedMilliseconds = "99";
            }
            else if (milliseconds < 10)
            {
                formattedMilliseconds = $"0{milliseconds}";
            }

            string formattedSeconds;

            if (secondsRounded < 10)
            {
                formattedSeconds = $"0{secondsRounded}";
            }
            else
            {
                formattedSeconds = secondsRounded.ToString();
            }

            _label.Text = $"{formattedSeconds}.{formattedMilliseconds}";
        }
    }
}
