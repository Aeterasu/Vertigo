using Godot;
using System;

namespace Vertigo
{
    public class ScreenShakeCamera2D : Camera2D
    {
        [Export] private readonly float _shakeFalloff = 8f;
        private float _force = 0f;

        public override void _PhysicsProcess(float delta)
        {
            base._PhysicsProcess(delta);

            if (!SettingsManager.Instance.IsValid())
            {
                return;
            }

            if (SettingsManager.Instance.IsScreenShakeEnabled)
            {
                Offset = Vector2.Zero;
            }

            float x = (float)GD.RandRange(-_force, _force);
            float y = (float)GD.RandRange(-_force, _force);

            Offset = new Vector2(x, y);

            _force = Mathf.Lerp(_force, 0f, _shakeFalloff * (delta / Engine.TimeScale));
        }

        public void AddShake(float amount)
        {
            if (SettingsManager.Instance.IsValid() && !SettingsManager.Instance.IsScreenShakeEnabled)
            {
                return;
            }

            _force += amount;
        }
    }
}
