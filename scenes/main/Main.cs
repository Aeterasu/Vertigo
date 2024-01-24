using Godot;
using System;

namespace Vertigo
{
    public class Main : Node
    {
        [Export] private PackedScene _gameScene;

        private Game _game;

        public override void _Ready()
        {
            base._Ready();

            InitGame();
        }

        public override void _PhysicsProcess(float delta)
        {
            base._PhysicsProcess(delta);

            if (Input.IsActionJustPressed("pause"))
            {
                GetTree().Paused = !GetTree().Paused;
            }

            if (Input.IsActionJustPressed("restart"))
            {
                GetTree().Paused = false;
                _game.SafeQueueFree();
                InitGame();
            }
        }

        private void InitGame()
        {
            if (_gameScene != null)
            {
                var gameNode = _gameScene.Instance();

                if (gameNode is Game)
                {
                    _game = gameNode as Game;
                }

                AddChild(gameNode);
            }
        }
    }
}
