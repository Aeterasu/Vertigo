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

            _label.Text = StringFormat.SecondsToStringFormatted(_timeManager.GetElapsedTime);
        }
    }
}
