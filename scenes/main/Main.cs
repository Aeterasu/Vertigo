using Godot;
using System;

namespace Vertigo
{
    public enum MainState
    {
        MAIN_MENU,
        GAME,
    }

    public class Main : Node
    {
        [Export] private MainState _state = MainState.MAIN_MENU;
        [Export] private readonly PackedScene _mainMenuScene;
        [Export] private readonly PackedScene _gameScene;

        private MainMenu _mainMenu;
        private Game _game;

        private ColorManager _colorManager;
        private HueShiftLayer _hueShiftLayer;

        public MainState State
        {
            get => _state;
            set
            {
                if (_state == value) // if the state would not be changed, do nothing
                {
                    return;
                }

                _state = value;

                Clear();

                switch (value)
                {
                    case MainState.MAIN_MENU:
                        InitMainMenu();
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

            InitMainMenu();

            MenuButton.OnButtonExecuted += OnMenuButtonExecuted;
            MenuButton.OnColorButtonPressedLeft += OnColorButtonPressedLeft;
            MenuButton.OnColorButtonPressedRight += OnColorButtonPressedRight;

            _colorManager = GetNode<ColorManager>("ColorManager");
            _hueShiftLayer = GetNode<HueShiftLayer>("HueShiftLayer");

            UpdateColorManager();
        }

        public override void _PhysicsProcess(float delta)
        {
            base._PhysicsProcess(delta);

            bool isGameStateContinuous = true;

            if (State == MainState.GAME)
            {
                isGameStateContinuous = _game.State == GameState.GAME;
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

        private void Clear()
        {
            // main menu

            if (_mainMenu.IsValid())
            {
                _mainMenu.SafeQueueFree();
            }

            _mainMenu = null;            

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
                State = MainState.GAME;
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
    }
}
