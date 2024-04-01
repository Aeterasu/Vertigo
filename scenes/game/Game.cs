using Godot;
using System;

namespace Vertigo
{
    public enum GameState
    {
        INTRO,
        GAME,
        UPGRADE_MENU,
        POST_MORTEM,
    }

    public class Game : Node2D
    {
        [Export] public PackedScene UpgradeMenuScene { get; private set; }
        [Export] public PackedScene DiamondScene { get; private set; }
        [Export] public PackedScene ConvergenceProjectileScene { get; private set; }
        [Export] public PackedScene GUIUpgradeScene { get; private set; }

        [Export] public float OnHitSlowMotionRatio { get; private set; } = 0.2f;
        [Export] public float OnHitSlowMotionDuration { get; private set; } = 1f;

        [Export] public AudioStream GemPickupSound { get; private set; }
        [Export] private readonly float _minGemPickupPitch = 0.8f;
        [Export] private readonly float _maxGemPickupPitch = 1.75f;
        [Export] private readonly float _gemPickupPitchStep = 0.1f;
        [Export] private readonly float _gemPickupPitchDecay = 1f;

        [Export] public bool IsBossEnabled { get; private set; } = false;
        [Export] public PackedScene BossScene { get; private set; }
        [Export] public PackedScene SubBossScene { get; private set; }
        [Export] public float PreBossFightSpawnStopDuration { get; private set; } = 15.0f;
        [Export] public float SpawnBossEvery { get; private set; } = 180.0f;
        [Export] public float BossEscapeTime { get; private set; } = 15.0f;
        [Export] public uint BossSuccessDifficultyIncrease { get; private set; } = 2;
        [Export] public uint BossFailDifficultyIncrease { get; private set; } = 5;

        [Export] public AudioStream HealSound { get; private set; }
        [Export] public AudioStream HeartbreakSound { get; private set; }

        private Player _player;
        private ExperienceComponent _playerExperienceComponent;

        private Control _gui;
        private Countdown _countdown;

        private Control _guiUpgradeBar;

        private TimeManager _timeManager;

        private UpgradeManager _upgradeManager;

        private RestartHandler _restartHandler;

        private float _gemPickupPitch = 0.8f;

        public bool IsInRestart { get; set; } = false;

        public float OnHitSlowMotionTimeLeft { get; private set; } = 0f;
        public float CurrentOnHitSlowMotionRatio { get; private set; } = 1f;
        public bool IsInOnHitSlowMotion { get; private set; } = false;

        public float RopeTopPosition { get; private set; }
        public float RopeBottomPosition { get; private set; }

        public Boss Boss { get; private set; } = null;
        public float BossSpawnTicks { get; private set; } = 0.0f;
        public bool IsBossSpawned { get; private set; } = false;
        public float BossEscapeTimerTicks { get; private set; } = 0.0f; 
        public bool AreSpawnsDisabled { get; private set; } = false;
        public bool SecondBossFight { get; private set; } = false;
        public int BossCount { get; private set; } = 0;

        public Player Player { get => _player; }
        
        public CanvasLayer GameLayer { get; private set; }
        public CanvasLayer GUILayer { get; private set; }

        public ScreenShakeCamera2D ScreenShakeCamera2D { get; private set; }

        public PauseMenu PauseMenu { get; private set; }

        public GameState State { get; private set; } = GameState.INTRO;

        public static Game Instance { get; set; }

        public override void _Ready()
        {
            base._Ready();

            GameLayer = GetNode<CanvasLayer>("%GameLayer");
            GUILayer = GetNode<CanvasLayer>("%GUILayer");

            PauseMenu = GetNode<PauseMenu>("GUILayer/PauseMenu");

            _player = GetNode<Player>("%Player");
            _playerExperienceComponent = _player.GetNode<ExperienceComponent>("%ExperienceComponent");
            _playerExperienceComponent.OnLevelUp += OnPlayerLevelUp;
            _player.OnPlayerHit += OnPlayerHit;
            _player.OnPlayerDeath += OnPlayerDeath;

            ScreenShakeCamera2D = GetNodeOrNull<ScreenShakeCamera2D>("%ScreenShakeCamera2D");

            var topShape = GetNode<JumpRope>("%JumpRopeTop").GetNode<CollisionShape2D>("CollisionShape2D").Shape as RectangleShape2D;

            RopeTopPosition = GetNode<JumpRope>("%JumpRopeTop").GlobalPosition.y + topShape.Extents.y * 2;
            RopeBottomPosition = GetNode<JumpRope>("%JumpRopeBottom").GlobalPosition.y;

            _timeManager = GetNode<TimeManager>("%TimeManager");

            _upgradeManager = GetNode<UpgradeManager>("%UpgradeManager");

            _gui = GetNode<Control>("%GUI");
            _gui.Visible = false;

            _restartHandler = GetNode<RestartHandler>("RestartHandler");
            _restartHandler.OnRestart += OnRestart;

            _countdown = GetNode<Countdown>("%Countdown");

            if (_countdown.IsValid())
            {
                _countdown.OnCountdownFinished += OnCountdownFinished;
            }

            OnHitSlowMotionTimeLeft = 0;

            _gemPickupPitch = _minGemPickupPitch;

            GetNode<CanvasLayer>("GUILayer").Visible = true;

            _guiUpgradeBar = GetNode<Control>("GUILayer/GUI/GUIUpgradeBar");
        }

        public override void _PhysicsProcess(float delta)
        {
            base._PhysicsProcess(delta);

            HandleOnHitSlowMotion(delta);

            Engine.TimeScale = State == GameState.GAME ? CurrentOnHitSlowMotionRatio : 1f;

            _gemPickupPitch = Mathf.Max(_gemPickupPitch - _gemPickupPitchDecay * delta, _minGemPickupPitch);

            if (IsBossEnabled)
            {
                HandleBoss(delta);
            }
        }

        public override void _Process(float delta)
        {
            base._Process(delta);

            _gui.Visible = State == GameState.GAME;
        }

        public void SetStateToGame()
        {
            State = GameState.GAME;
            _timeManager.StartTimer();            
        }

        private void OnCountdownFinished()
        {
            SetStateToGame();
        }

        private void HandleOnHitSlowMotion(float delta)
        {
            OnHitSlowMotionTimeLeft = Mathf.Max(OnHitSlowMotionTimeLeft - (delta / Engine.TimeScale), 0f);

            IsInOnHitSlowMotion = OnHitSlowMotionTimeLeft > 0f;

            if (IsInOnHitSlowMotion)
            {
                CurrentOnHitSlowMotionRatio = Mathf.Lerp(CurrentOnHitSlowMotionRatio, OnHitSlowMotionRatio, 5 * (delta / Engine.TimeScale));
            }
            else
            {
                CurrentOnHitSlowMotionRatio = Mathf.Lerp(CurrentOnHitSlowMotionRatio, 1f, 5 * (delta / Engine.TimeScale));
            }
        }

        private void OnPlayerHit()
        {
            OnHitSlowMotionTimeLeft = OnHitSlowMotionDuration;
        }

        private void OnPlayerDeath()
        {
            var gameOverLayer = GetNode<GameOverLayer>("GameOverLayer");

            gameOverLayer.Animate(_player);
            gameOverLayer.GetNode<AnimationPlayer>("AnimationPlayer").Connect("animation_finished", Main.Instance, "OnGameOverAnimationFinished");
            
            GetNode<CanvasLayer>("GUILayer").Visible = false;

            _restartHandler.Enabled = false;

            if (GameResultsManager.Instance.IsValid())
            {
                GameResultsManager.Instance.CurrentGameResults = new GameResults(_timeManager.GetElapsedTime);
            }
        }

        private void OnPlayerLevelUp(ExperienceComponent exp)
        {
            if (UpgradeMenuScene == null)
            {
                return;
            }

            State = GameState.UPGRADE_MENU;
            _timeManager.StopTimer();

            var node = UpgradeMenuScene.Instance();

            if (!(node is UpgradeMenu))
            {
                node.SafeQueueFree();
                SetStateToGame();
                return;
            }

            Engine.TimeScale = 1f;
            
            UpgradeMenu upgradeMenu = node as UpgradeMenu;
            upgradeMenu.OnMenuClosed += OnUpgradeMenuClosed;
            upgradeMenu.OnUpgradeSelected += OnUpgradeSelected;
            GUILayer.AddChild(upgradeMenu);
        }

        private void OnUpgradeSelected(UpgradeCard upgradeCard)
        {
            _upgradeManager.AddUpgrade(upgradeCard);
        }

        private void OnUpgradeMenuClosed()
        {
            SetStateToGame();
        }

        private void OnRestart()
        {
            Main.Instance.GameRestart();
        }

        private void StopSpawnsPreBoss()
        {
            ToggleSpawns(isEnabled: false);

            AreSpawnsDisabled = true;
        }

        private void HandleBoss(float delta)
        {
            if (!IsBossEnabled)
            {
                return;
            }

            if (!IsBossSpawned)
            {
                BossSpawnTicks += delta;
            }

            if (!AreSpawnsDisabled && BossSpawnTicks >= SpawnBossEvery - PreBossFightSpawnStopDuration && !AreSpawnsDisabled)
            {
                StopSpawnsPreBoss();
            }

            if (BossSpawnTicks >= SpawnBossEvery && !IsBossSpawned)
            {
                BossSpawnTicks = 0.0f;
                IsBossSpawned = true;
                SpawnBoss();
            }

            if (IsBossSpawned)
            {
                BossEscapeTimerTicks += delta;

                if (BossEscapeTimerTicks >= BossEscapeTime)
                {
                    TriggerBossEscape();
                }
            }
        }

        private void SpawnBoss()
        {
            Node boss = BossScene.Instance();

            BossCount += 1;
            
            Node subBoss = null;

            if (SecondBossFight)
            {
                subBoss = SubBossScene.Instance();

                if (subBoss is Boss)
                {
                    (subBoss as Boss).OnDestroyed += OnBossDestroyed;
                }

                BossCount += 1;
            }
            
            if (boss is Boss)
            {
                (boss as Boss).OnDestroyed += OnBossDestroyed;
            }

            uint random = GD.Randi() % 2;

            if (random == 0)
            {
                (boss as Boss).SetDirection(1);

                if (SecondBossFight)
                {
                    (subBoss as Boss).SetDirection(-1);
                }
            }
            else
            {
                (boss as Boss).SetDirection(-1);

                if (SecondBossFight)
                {
                    (subBoss as Boss).SetDirection(1);
                }
            }

            GameLayer.AddChild(boss);

            if (SecondBossFight)
            {
                GameLayer.AddChild(subBoss);
            }

            Boss = boss as Boss;

            SecondBossFight = true;
        }

        private void OnBossDestroyed(Hazard hazard)
        {
            BossCount -= 1;

            hazard.OnDestroyed -= OnBossDestroyed;

            if (BossCount == 0)
            {
                ToggleSpawns(isEnabled: true);

                AreSpawnsDisabled = false;

                IsBossSpawned = false;

                Boss = null;

                _timeManager.StepDifficulty((uint)BossSuccessDifficultyIncrease);

                BossEscapeTimerTicks = 0.0f;
            }
        }

        private void TriggerBossEscape()
        {
            BossEscapeTimerTicks = 0.0f;

            if (!Boss.IsValid())
            {
                Boss = null;
                return;
            }

            Boss.TriggerEscape();

            IsBossSpawned = false;
            ToggleSpawns(isEnabled: true);

            _timeManager.StepDifficulty((uint)BossFailDifficultyIncrease);
        }

        public void ToggleSpawns(bool isEnabled)
        {
            foreach (Node node in GameLayer.GetChildren())
            {
                if (node is HazardSpawner)
                {
                    (node as HazardSpawner).Enabled = isEnabled;
                }
            }
        }

        public void PlayGemPickupSound()
        {
            if (GemPickupSound != null)
            {
                DeconflictedSoundPlayer.PlayUnconflictingSound(GemPickupSound, this, 0f, _gemPickupPitch);
                _gemPickupPitch = Mathf.Min(_gemPickupPitch + _gemPickupPitchStep, _maxGemPickupPitch);
            }    
        }

        public void PlayHealSound()
        {
            if (HealSound != null)
            {
                DeconflictedSoundPlayer.PlayUnconflictingSound(HealSound, this, 0f, 1f);
            }    
        }

        public void PlayHeartbreakSound()
        {
            if (HeartbreakSound != null)
            {
                DeconflictedSoundPlayer.PlayUnconflictingSound(HeartbreakSound, this, 0f, 1f);
            }    
        }

        public void AddPlayerInputDelay(float amount)
        {
            _player.InputDelay += amount;
        }

        public void CreateGuiUpgrade(UpgradeCard card, Vector2 offset, Upgrade upgrade)
        {
            GUIUpgrade node = GUIUpgradeScene.Instance() as GUIUpgrade;

            _guiUpgradeBar.AddChild(node);
            node.RectPosition = offset;
            node.PlayAnimation();
            node.SetStack(0);
            node.SetUpgrade(card);
            node.Upgrade = upgrade;
        }

        public void UpgradeGuiUpgrade(Upgrade upgrade)
        {
            foreach (Node node in _guiUpgradeBar.GetChildren())
            {
                if (node is GUIUpgrade)
                {
                    GUIUpgrade guiUpgrade = node as GUIUpgrade;
                    
                    if (guiUpgrade.Upgrade == upgrade)
                    {
                        guiUpgrade.PlayAnimation();
                        guiUpgrade.SetStack(upgrade.Stack);
                    }
                }
            }
        }
    }
}
