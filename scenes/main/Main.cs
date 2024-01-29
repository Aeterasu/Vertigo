using Godot;
using System;

namespace Vertigo
{
    public class Main : Node
    {
        [Export] private PackedScene _gameScene;

        private Game _game;

        public bool IsPaused { get; private set; } = false;

        public override void _Ready()
        {
            base._Ready();

            InitGame();
        }

        public override void _PhysicsProcess(float delta)
        {
            base._PhysicsProcess(delta);

            if (Input.IsActionJustPressed(InputActions.INPUT_RESTART))
            {
                IsPaused = false;
                _game.SafeQueueFree();
                InitGame();
            }

            bool isGameStateContinuous = true;

            if (_game.IsValid())
            {
                isGameStateContinuous = _game.State == GameState.GAME;
            }

            if (Input.IsActionJustPressed(InputActions.INPUT_PAUSE) && isGameStateContinuous)
            {
                IsPaused = !IsPaused;
            }

            GetTree().Paused = IsPaused || !isGameStateContinuous;
        }

        private void InitGame()
        {
            if (_gameScene != null)
            {
                var gameNode = _gameScene.Instance();

                if (gameNode is Game)
                {
                    _game = gameNode as Game;
                    Game.Instance = _game;
                }

                AddChild(gameNode);
            }
        }
    }
}
