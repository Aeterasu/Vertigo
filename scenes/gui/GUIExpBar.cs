using Godot;
using System;

namespace Vertigo
{
    public class GUIExpBar : TextureProgress
    {
        public override void _Ready()
        {
            base._Ready();
        }

        public override void _PhysicsProcess(float delta)
        {
            base._PhysicsProcess(delta);

            /*

            if (!_timeManager.IsValid())
            {
                return;
            }

            MinValue = 0f;
            MaxValue = _timeManager.GetMaxTimer;
            Value = _timeManager.GetTimer;

            if (_segment.IsValid())
            {
                _segment.RectPosition = new Vector2(24 + 333 * (_timeManager.GetMinTimer / _timeManager.GetMaxTimer), _segment.RectPosition.y);
            }

            */
        }
    }
}
