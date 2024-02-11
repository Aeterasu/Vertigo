using Godot;
using System;
using System.Collections.Generic;

namespace Vertigo
{
    public class HazardSpawner : Node
    {
        [Export] private float _canOnlySpawnAfter = 15f;
        [Export] private int _maxHazards = 16;
        [Export] private PackedScene _scene;
        [Export] private float _spawnRate = 5f;
        [Export] private float _spawnRateDifficultyStep = 0.5f;
        [Export] private float _spawnMinimalRate = 0.5f;
        
        private Timer _spawnTimer = new Timer();

        private List<SpawnPoint> _spawnPoints = new List<SpawnPoint>();

        private TimeManager _timeManager;

        private int _currentHazardCount = 0;

        private bool _init = false;

        public int Difficulty { get; private set; } = 1;

        public event Action<Hazard> OnHazardLeftThePlayArea;

        public override void _Ready()
        {
            base._Ready();

            AddChild(_spawnTimer);

            _timeManager = GetNodeOrNull<TimeManager>("%TimeManager");

            if (_timeManager.IsValid())
            {
                _timeManager.OnDifficultyStepReached += StepDifficulty;
            }
        }

        public override void _PhysicsProcess(float delta)
        {
            base._PhysicsProcess(delta);

            if (_timeManager.GetPersistentTimer >= _canOnlySpawnAfter && !_init)
            {
                _spawnPoints = GetSpawnPoints();

                _spawnTimer.WaitTime = _spawnRate;
                _spawnTimer.Connect("timeout", this, "OnSquareSpawnTimeout");
                _spawnTimer.Start();

                _init = true;
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
            if (_scene == null)
            {
                return;
            }

            if (_spawnPoints.Count <= 0)
            {
                return;
            }

            if (_currentHazardCount >= _maxHazards)
            {
                return;
            }

            Random random = new Random();
            int randomId = random.Next(_spawnPoints.Count);
            SpawnPoint spawnPoint = _spawnPoints[randomId];

            var scene = _scene.Instance();

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

            _currentHazardCount += 1;
        }

        private void ProcessSquare(Square square, SpawnPoint spawnPoint)
        {
            square.GlobalPosition = spawnPoint.GlobalPosition;
            square.Direction = spawnPoint.Direction;
            square.OnLeftThePlayArea += HazardLeftThePlayArea;   
            square.OnDestroyed += OnHazardDestroyed;

            Game.Instance.IfValid().GameLayer.IfValid().CallDeferred("add_child", square);       
        }

        private void ProcessSquareTelegraph(SquareTelegraph telegraph, SpawnPoint spawnPoint)
        {
            telegraph.GlobalPosition = spawnPoint.GlobalPosition;
            telegraph.GetNode<Sprite>("Sprite").Scale = new Vector2((float)spawnPoint.Direction, 1f);
            Square square = telegraph.GetNode<Square>(telegraph.SquarePath);
            square.Direction = spawnPoint.Direction;
            square.OnLeftThePlayArea += HazardLeftThePlayArea;
            square.OnDestroyed += OnHazardDestroyed;

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
            if (_timeManager.GetPersistentTimer < _canOnlySpawnAfter)
            {
                return;
            }

            Difficulty += 1;

            _spawnRate = Mathf.Clamp(_spawnRate - _spawnRateDifficultyStep, _spawnMinimalRate, 999f);
            _spawnTimer.WaitTime = _spawnRate;
            _spawnTimer.Start();
        }

        private void HazardLeftThePlayArea(Hazard hazard)
        {
            hazard.OnLeftThePlayArea -= OnHazardLeftThePlayArea;
            OnHazardLeftThePlayArea?.Invoke(hazard);

            _currentHazardCount -= 1;
        }

        private void OnHazardDestroyed(Hazard hazard)
        {
            _currentHazardCount -= 1;
        }
    }
}
