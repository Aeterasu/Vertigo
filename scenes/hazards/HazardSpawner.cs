using Godot;
using System;
using System.Collections.Generic;

namespace Vertigo
{
    public class HazardSpawner : Node
    {
        [Export] private PackedScene _squareScene;
        [Export] private float _squareSpawnRate = 5f;

        private Timer _squareSpawnTimer = new Timer();

        private List<SpawnPoint> _spawnPoints = new List<SpawnPoint>();

        public override void _Ready()
        {
            base._Ready();

            _spawnPoints = GetSpawnPoints();

            AddChild(_squareSpawnTimer);
            _squareSpawnTimer.WaitTime = _squareSpawnRate;
            _squareSpawnTimer.Connect("timeout", this, "OnSquareSpawnTimeout");
            _squareSpawnTimer.Start();
        }

        private List<SpawnPoint> GetSpawnPoints()
        {
            List<SpawnPoint> points = new List<SpawnPoint>();

            foreach (Node node in GetChildren())
            {
                if (node is SpawnPoint)
                {
                    points.Add(node as SpawnPoint);
                }
            }

            return points;
        }

        private void OnSquareSpawnTimeout()
        {
            if (_squareScene == null)
            {
                return;
            }

            if (_spawnPoints.Count <= 0)
            {
                return;
            }

            Random random = new Random();
            int randomId = random.Next(_spawnPoints.Count);
            SpawnPoint spawnPoint = _spawnPoints[randomId];

            var scene = _squareScene.Instance();

            if (!(scene is Square))
            {
                scene.SafeQueueFree();
                return;
            }
            
            Square square = scene as Square;
            square.GlobalPosition = spawnPoint.GlobalPosition;
            square.Direction = spawnPoint.Direction;

            Game.Instance.IfValid().GameLayer.IfValid().CallDeferred("add_child", square);
        }
    }
}
