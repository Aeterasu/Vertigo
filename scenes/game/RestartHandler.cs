using Godot;
using System;

namespace Vertigo
{
    public class RestartHandler : Node
    {
        [Export] private readonly float _restartTime = 2.0f;
        [Export] private readonly float _flashThreshold = 1.5f;

        private GUIRestart _guiRestart;
        private float _currentRestartTicks = 0f;

        public event Action OnRestart;

        public override void _Ready()
        {
            base._Ready();
        }

        public override void _PhysicsProcess(float delta)
        {
            base._PhysicsProcess(delta);

            if (!_guiRestart.IsValid())
            {
                _guiRestart = Game.Instance.GetNode<GUIRestart>("GUILayer/GUIRestart");
                _guiRestart.TextureProgress.MaxValue = _restartTime;
                _guiRestart.Modulate = new Color(1, 1, 1, 0);
            }

            if (Input.IsActionPressed(InputActions.INPUT_RESTART) && Game.Instance.State == GameState.GAME)
            {
                _currentRestartTicks += delta;
                _guiRestart.Modulate = _guiRestart.Modulate.LinearInterpolate(new Color(1, 1, 1, 1), 8 * delta);
                Game.Instance.IsInRestart = true;
            }
            else
            {
                _currentRestartTicks = 0;
                _guiRestart.Modulate = _guiRestart.Modulate.LinearInterpolate(new Color(1, 1, 1, 0), 12 * delta);
                Game.Instance.IsInRestart = false;
            }

            _guiRestart.TextureProgress.Value = _currentRestartTicks;
            
            if (_currentRestartTicks >= _flashThreshold)
            {
                _guiRestart.ColorFlash.Modulate = new Color(1, 1, 1, (_currentRestartTicks - _flashThreshold) / (_restartTime - _flashThreshold));
            }
            else
            {
                _guiRestart.ColorFlash.Modulate = new Color(1, 1, 1, 0);
            }

            if (_currentRestartTicks >= _restartTime)
            {
                OnRestart?.Invoke();
            }
        }
    }
}