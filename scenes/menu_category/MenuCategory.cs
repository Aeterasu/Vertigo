using Godot;
using System;
using System.Collections.Generic;

namespace Vertigo
{
    public class MenuCategory : Control
    {
        [Export] public bool DisableInput { get; set; }

        private List<MenuButton> _buttons = new List<MenuButton>();

        private int _currentButtonId = 0;

        private MenuButton _currentButton;

        public int CurrentButtonId
        {
            get => _currentButtonId;
            set
            {
                if (_currentButton.IsValid() && _currentButton.IsFocused && _currentButtonId != -1)
                {
                    _currentButton.Unfocus();
                }

                if (value == -1)
                {
                    _currentButtonId = value;
                    return;
                }

                _currentButtonId = value;
                
                _currentButton = _buttons[_currentButtonId];
                
                if (!_currentButton.IsFocused)
                {
                    _currentButton.Focus();
                }
            }
        }

        public bool FirstInput { get; private set; } = true;

        public override void _Ready()
        {
            base._Ready();

            foreach (Node node in GetChildren())
            {
                if (node is MenuButton)
                {
                    MenuButton button = node as MenuButton;

                    _buttons.Add(button);
                    button.OnButtonMouseEntered += OnButtonMouseEntered;
                    button.OnButtonMouseExited += OnButtonMouseExited;
                }
            }
        }

        public override void _PhysicsProcess(float delta)
        {
            base._PhysicsProcess(delta);

            HandleInput();
        }

        private void HandleInput()
        {
            if (DisableInput)
            {
                CurrentButtonId = -1;
                return;
            }

            if (Input.IsActionJustPressed(InputActions.INPUT_ACTION_MOVE_UP))
            {
                if (FirstInput)
                {
                    CurrentButtonId = 0;
                    FirstInput = false;
                    return;
                }

                if (CurrentButtonId - 1 >= 0)
                {
                    CurrentButtonId -= 1;
                }
                else
                {
                    CurrentButtonId = _buttons.Count - 1;
                }

                if (!_buttons[CurrentButtonId].Active)
                {
                    CurrentButtonId -= 1;
                }
            }

            if (Input.IsActionJustPressed(InputActions.INPUT_ACTION_MOVE_DOWN))
            {
                if (FirstInput)
                {
                    CurrentButtonId = 0;
                    FirstInput = false;
                    return;
                }

                if (CurrentButtonId + 1 <= _buttons.Count - 1)
                {
                    CurrentButtonId += 1;
                }
                else
                {
                    CurrentButtonId = 0;
                }

                if (!_buttons[CurrentButtonId].Active)
                {
                    CurrentButtonId += 1;
                }
            }

            if (Input.IsActionJustPressed(InputActions.INPUT_ACTION_CONFIRM) && !FirstInput && _currentButton.IsValid() && _currentButton.IsFocused)
            {
                MenuButton.OnButtonExecuted?.Invoke(_currentButton);
                _currentButton.PlayConfirmAudio();
            }

            if ((Input.IsActionJustPressed(InputActions.INPUT_ACTION_MOVE_LEFT) || Input.IsActionJustPressed(InputActions.INPUT_ACTION_DISCARD)) 
            && !FirstInput && _currentButton.IsValid() && _currentButton.ButtonType == MenuButtonType.MENU_BUTTON_COLOR)
            {
                MenuButton.OnColorButtonPressedLeft?.Invoke(_currentButton);
            }

            if ((Input.IsActionJustPressed(InputActions.INPUT_ACTION_MOVE_RIGHT) || Input.IsActionJustPressed(InputActions.INPUT_ACTION_CONFIRM)) 
            && !FirstInput && _currentButton.IsValid() && _currentButton.ButtonType == MenuButtonType.MENU_BUTTON_COLOR)
            {
                MenuButton.OnColorButtonPressedRight?.Invoke(_currentButton);
            }
        }

        private void OnButtonMouseEntered(MenuButton button)
        {
            if (DisableInput)
            {
                return;
            }

            if (!_buttons.Contains(button))
            {
                return;
            }

            if (_currentButton == button)
            {
                return;
            }

            CurrentButtonId = _buttons.IndexOf(button);
            FirstInput = false;
        }

        private void OnButtonMouseExited(MenuButton button)
        {
            if (!_buttons.Contains(button))
            {
                return;
            }

            FirstInput = true;
            CurrentButtonId = -1;
            _currentButton = null;
        }
    }
}