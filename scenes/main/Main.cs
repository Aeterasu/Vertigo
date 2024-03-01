using Godot;
using System;

namespace Vertigo
{
    public enum MainState
    {
        MAIN_MENU,
        INTERLUDE,
        GAME,
        GAME_OVER,
        LEADERBOARD_MENU,
        OPTIONS_MENU,
    }

    public class Main : Node
    {
        [Export] private MainState _state = MainState.MAIN_MENU;
        [Export] private readonly PackedScene _mainMenuScene;
        [Export] private readonly PackedScene _interludeScene;
        [Export] private readonly PackedScene _gameScene;
        [Export] private readonly PackedScene _gameOverScene;
        [Export] private readonly PackedScene _leaderboardMenuScene;
        [Export] private readonly PackedScene _settingsMenuScene;

        private MainMenu _mainMenu;
        private Interlude _interlude;
        private Game _game;
        private GameOverScreen _gameOver;
        private LeaderboardMenu _leaderboardMenu;
        private SettingsMenu _settingsMenu;

        private ColorManager _colorManager;
        private HueShiftLayer _hueShiftLayer;

        public MainState State
        {
            get => _state;
            set
            {
                _state = value;

                Clear();

                switch (value)
                {
                    case MainState.MAIN_MENU:
                        InitMainMenu();
                        break;
                    case MainState.INTERLUDE:
                        InitInterlude();
                        break;
                    case MainState.GAME:
                        InitGame();
                        break;
                    case MainState.GAME_OVER:
                        InitGameOver();
                        break;
                    case MainState.LEADERBOARD_MENU:
                        InitLeaderboardMenu();
                        break;
                    case MainState.OPTIONS_MENU:
                        InitSettingsMenu();
                        break;
                }
            }
        }

        public ColorManager ColorManager { get => _colorManager; } 

        public bool IsPaused { get; private set; } = false;

        public static Main Instance { get; private set; }

        public override void _Ready()
        {
            base._Ready();

            Instance = this;

            MenuButton.OnButtonExecuted += OnMenuButtonExecuted;
            MenuButton.OnColorButtonPressedLeft += OnColorButtonPressedLeft;
            MenuButton.OnColorButtonPressedRight += OnColorButtonPressedRight;

            _colorManager = GetNode<ColorManager>("ColorManager");
            _hueShiftLayer = GetNode<HueShiftLayer>("HueShiftLayer");

            UpdateColorManager();

            State = _state;
        }

        public override void _PhysicsProcess(float delta)
        {
            base._PhysicsProcess(delta);

            bool isGameStateContinuous = true;

            if (State == MainState.GAME && _game.IsValid())
            {
                isGameStateContinuous = _game.State == GameState.GAME && !_game.IsInRestart;
            }
            else
            {
                _game = null;
            }

            if (Input.IsActionJustPressed(InputActions.INPUT_PAUSE) && isGameStateContinuous && State == MainState.GAME)
            {
                IsPaused = !IsPaused;
            }

            if (State == MainState.GAME)
            {
                if (_game.State == GameState.GAME)
                {
                    _game.PauseMenu.IsDisplayed = IsPaused;
                }
                else
                {
                    _game.PauseMenu.IsDisplayed = false;
                }
            }

            if (State != MainState.GAME || (_game.IsValid() && _game.Player.IsDead))
            {
                IsPaused = false;
            }

            GetTree().Paused = IsPaused || !isGameStateContinuous;
        }

        private void InitMainMenu()
        {
            if (_mainMenuScene != null)
            {
                var mainMenuNode = _mainMenuScene.Instance();

                if (mainMenuNode is MainMenu)
                {
                    _mainMenu = mainMenuNode as MainMenu;
                    MainMenu.Instance = _mainMenu;
                }

                AddChild(mainMenuNode);
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
                    Game.Instance = _game;
                }

                AddChild(gameNode);
            }
        }

        private void InitInterlude()
        {
            if (_interludeScene != null)
            {
                var interludeNode = _interludeScene.Instance();

                if (interludeNode is Interlude)
                {
                    _interlude = interludeNode as Interlude;
                    _interlude.OnInterludeAnimationFinished += OnInterludeAnimationFinished;
                }

                AddChild(_interlude);
            }
            else
            {
                State = MainState.GAME;
            }           
        }

        private void InitGameOver()
        {
            if (_gameOverScene != null)
            {
                var gameOverNode = _gameOverScene.Instance();

                if (gameOverNode is GameOverScreen)
                {
                    _gameOver = gameOverNode as GameOverScreen;
                }

                AddChild(_gameOver);
            }
            else
            {
                State = MainState.MAIN_MENU;
            }
        }

        private void InitLeaderboardMenu()
        {
            if (_leaderboardMenuScene != null)
            {
                var leaderboardMenuNode = _leaderboardMenuScene.Instance();

                if (leaderboardMenuNode is LeaderboardMenu)
                {
                    _leaderboardMenu = leaderboardMenuNode as LeaderboardMenu;
                }

                AddChild(_leaderboardMenu);
            }
            else
            {
                State = MainState.MAIN_MENU;
            }            
        }

        private void InitSettingsMenu()
        {
            if (_settingsMenuScene != null)
            {
                var settingsMenuNode = _settingsMenuScene.Instance();

                if (settingsMenuNode is SettingsMenu)
                {
                    _settingsMenu = settingsMenuNode as SettingsMenu;
                }

                AddChild(_settingsMenu);
            }
            else
            {
                State = MainState.MAIN_MENU;
            }                        
        }

        private void OnInterludeAnimationFinished()
        {
            State = MainState.GAME;
        }

        public void OnGameOverAnimationFinished(string animName)
        {
            State = MainState.GAME_OVER;
        }

        private void Clear()
        {
            // main menu

            _mainMenu.SafeQueueFree();
            _mainMenu = null;

            // interlude

            _interlude.SafeQueueFree();      
            _interlude = null;

            // game

            _game.SafeQueueFree();
            _game = null;

            // game over

            _gameOver.SafeQueueFree();
            _gameOver = null;

            // leaderboard

            _leaderboardMenu.SafeQueueFree();
            _leaderboardMenu = null;

            // settings

            _settingsMenu.SafeQueueFree();
            _settingsMenu = null;
        }

        private void OnMenuButtonExecuted(MenuButton button)
        {
            if (button.ButtonType == MenuButtonType.MENU_BUTTON_START)
            {
                State = MainState.INTERLUDE;
                return;
            }

            if (button.ButtonType == MenuButtonType.MENU_BUTTON_EXIT)
            {
                GetTree().Quit();
                return;
            }

            if (button.ButtonType == MenuButtonType.MENU_BUTTON_RESTART)
            {
                State = MainState.GAME;
                return;
            }

            if (button.ButtonType == MenuButtonType.MENU_BUTTON_BACK_TO_MENU)
            {
                State = MainState.MAIN_MENU;
                return;
            }

            if (button.ButtonType == MenuButtonType.MENU_BUTTON_LEADERBOARD_MENU)
            {
                State = MainState.LEADERBOARD_MENU;
                return;
            }

            if (button.ButtonType == MenuButtonType.MENU_BUTTON_OPTIONS)
            {
                State = MainState.OPTIONS_MENU;
                return;
            }

            if (button.ButtonType == MenuButtonType.MENU_BUTTON_OPTIONS_FULLSCREEN && SettingsManager.Instance.IsValid())
            {
                SettingsManager.Instance.IsFullscreen = !SettingsManager.Instance.IsFullscreen;
            }

            if (button.ButtonType == MenuButtonType.MENU_BUTTON_OPTIONS_SCREEN_SHAKE && SettingsManager.Instance.IsValid())
            {
                SettingsManager.Instance.IsScreenShakeEnabled = !SettingsManager.Instance.IsScreenShakeEnabled;
            }

            if (button.ButtonType == MenuButtonType.MENU_BUTTON_OPTIONS_TRAIL_EFFECT && SettingsManager.Instance.IsValid())
            {
                SettingsManager.Instance.IsTrailEffectEnabled = !SettingsManager.Instance.IsTrailEffectEnabled;
            }

            if (button.ButtonType == MenuButtonType.MENU_BUTTON_PAUSE_RESUME)
            {
                if (State == MainState.GAME)
                {
                    _game.AddPlayerInputDelay(0.25f);
                    IsPaused = false;
                }
            }

            if (button.ButtonType == MenuButtonType.MENU_BUTTON_LEADERBOARD_SCROLL_UP && State == MainState.LEADERBOARD_MENU)
            {
                _leaderboardMenu.ScrollUp(multiplier: 8f);
            }

            if (button.ButtonType == MenuButtonType.MENU_BUTTON_LEADERBOARD_SCROLL_DOWN && State == MainState.LEADERBOARD_MENU)
            {
                _leaderboardMenu.ScrollDown(multiplier: 8f);
            }
        }

        private void OnColorButtonPressedLeft(MenuButton button)
        {
            if (button.ButtonType != MenuButtonType.MENU_BUTTON_COLOR)
            {
                return;
            }

            _colorManager.CurrentColorId -= 1;
            UpdateColorManager();
        }

        private void OnColorButtonPressedRight(MenuButton button)
        {
            if (button.ButtonType != MenuButtonType.MENU_BUTTON_COLOR)
            {
                return;
            }

            _colorManager.CurrentColorId += 1;
            UpdateColorManager();
        }

        private void UpdateColorManager()
        {
            _hueShiftLayer.Gradient = _colorManager.Colors[_colorManager.CurrentColorId].Gradient;
        }

        public void GameRestart()
        {
            if (State != MainState.GAME)
            {
                return;
            }

            Engine.TimeScale = 1.0f;

            Clear();
            InitGame();
        }
    }
}
