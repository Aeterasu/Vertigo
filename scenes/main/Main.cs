using Godot;
using System;

namespace Vertigo
{
    public enum MainState
    {
        MAIN_MENU,
        INTERLUDE,
        GAME,
    }

    public class Main : Node
    {
        [Export] private MainState _state = MainState.MAIN_MENU;
        [Export] private readonly PackedScene _mainMenuScene;
        [Export] private readonly PackedScene _interludeScene;
        [Export] private readonly PackedScene _gameScene;

        private MainMenu _mainMenu;
        private Interlude _interlude;
        private Game _game;

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

            if (State != MainState.GAME)
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

        private void OnInterludeAnimationFinished()
        {
            GD.Print("MEOW");

            State = MainState.GAME;
        }

        private void Clear()
        {
            // main menu

            if (_mainMenu.IsValid())
            {
                _mainMenu.SafeQueueFree();
            }

            _mainMenu = null;

            // interlude

            if (_interlude.IsValid())
            {
                _interlude.SafeQueueFree();
            }            

            _interlude = null;

            // game

            if (_game.IsValid())
            {
                _game.SafeQueueFree();
            }

            _game = null;
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
