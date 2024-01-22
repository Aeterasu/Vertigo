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
