using Godot;
using System;
using System.Collections.Generic;

namespace Vertigo
{
    public class HazardSpawner : Node
    {
        [Export] private PackedScene _squareScene;
        [Export] private float _squareSpawnRate = 5f;
        [Export] private float _squareSpawnRateDifficultyStep = 0.5f;
        [Export] private float _squareSpawnMinimalRate = 0.5f;

        private Timer _squareSpawnTimer = new Timer();

        private List<SpawnPoint> _spawnPoints = new List<SpawnPoint>();

        public int Difficulty { get; private set; } = 1;

        public event Action<Hazard> OnHazardLeftThePlayArea;

        public override void _Ready()
        {
            base._Ready();

            _spawnPoints = GetSpawnPoints();

            AddChild(_squareSpawnTimer);
            _squareSpawnTimer.WaitTime = _squareSpawnRate;
            _squareSpawnTimer.Connect("timeout", this, "OnSquareSpawnTimeout");
            _squareSpawnTimer.Start();

            TimeManager _timeManager = GetNodeOrNull<TimeManager>("%TimeManager");

            if (_timeManager.IsValid())
            {
                _timeManager.OnDifficultyStepReached += StepDifficulty;
            }
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

            if (!(scene is Square) && !(scene is SquareTelegraph))
            {
                scene.SafeQueueFree();
                return;
            }

            if (scene is Square)
            {
                ProcessSquare(scene as Square, spawnPoint);
            }
            else if (scene is SquareTelegraph)
            {
                ProcessSquareTelegraph(scene as SquareTelegraph, spawnPoint);
            }
        }

        private void ProcessSquare(Square square, SpawnPoint spawnPoint)
        {
            square.GlobalPosition = spawnPoint.GlobalPosition;
            square.Direction = spawnPoint.Direction;
            square.OnLeftThePlayArea += HazardLeftThePlayArea;     

            Game.Instance.IfValid().GameLayer.IfValid().CallDeferred("add_child", square);       
        }

        private void ProcessSquareTelegraph(SquareTelegraph telegraph, SpawnPoint spawnPoint)
        {
            telegraph.GlobalPosition = spawnPoint.GlobalPosition;
            telegraph.GetNode<Sprite>("Sprite").Scale = new Vector2((float)spawnPoint.Direction, 1f);
            Square square = telegraph.GetNode<Square>(telegraph.SquarePath);
            square.Direction = spawnPoint.Direction;
            square.OnLeftThePlayArea += HazardLeftThePlayArea;

            if (Game.Instance.IsValid())
            {
                if (Game.Instance.GameLayer.IsValid())
                {
                    Game.Instance.IfValid().GameLayer.IfValid().CallDeferred("add_child", telegraph);
                }
            }
        }

        public void StepDifficulty()
        {
            Difficulty += 1;

            _squareSpawnRate = Mathf.Clamp(_squareSpawnRate - _squareSpawnRateDifficultyStep, _squareSpawnMinimalRate, 999f);
            _squareSpawnTimer.WaitTime = _squareSpawnRate;
            _squareSpawnTimer.Start();
        }

        private void HazardLeftThePlayArea(Hazard hazard)
        {
            hazard.OnLeftThePlayArea -= OnHazardLeftThePlayArea;
            OnHazardLeftThePlayArea?.Invoke(hazard);
        }
    }
}
