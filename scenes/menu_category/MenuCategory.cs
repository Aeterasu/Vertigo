using Godot;
using System;
using System.Collections.Generic;

namespace Vertigo
{
    public enum MenuCategoryOrientation
    {
        VERTICAL,
        HORIZONTAL,
    }

    public enum MenuCategoryInputType
    {
        NORMAL,
        MOUSE_ONLY,
    }

    public class MenuCategory : Control
    {
        [Export] public MenuCategoryOrientation Orientation { get; private set; } = MenuCategoryOrientation.VERTICAL;
        [Export] public MenuCategoryInputType InputType { get; private set; } = MenuCategoryInputType.NORMAL;
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

        public MenuButton GetCurrentButton { get => _currentButton; }

        public bool FirstInput { get; set; } = true;

        public override void _Ready()
        {
            base._Ready();

            foreach (Node node in GetChildren())
            {
                if (node is MenuButton)
                {
                    MenuButton button = node as MenuButton;

                    button.Id = _buttons.Count;

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

            if (((Input.IsActionJustPressed(InputActions.INPUT_ACTION_MOVE_UP) && Orientation == MenuCategoryOrientation.VERTICAL) 
            || (Input.IsActionJustPressed(InputActions.INPUT_ACTION_MOVE_LEFT) && Orientation == MenuCategoryOrientation.HORIZONTAL))
            && InputType == MenuCategoryInputType.NORMAL)
            {
                if (FirstInput)
                {
                    if (_buttons[0].Active)
                    {
                        CurrentButtonId = 0;
                    }
                    else
                    {
                        CurrentButtonId = 1;
                    }

                    FirstInput = false;
                    return;
                }

                int targetId = CurrentButtonId - 1;
                int nextButtonId = 0; // if all else fails

                if (targetId < 0)
                {
                    targetId = _buttons.Count - 1;
                }

                if (_buttons[targetId].Active)
                {
                    CurrentButtonId = targetId;
                }
                else if (targetId == 0 && !_buttons[0].Active)
                {
                    CurrentButtonId = _buttons.Count - 1;
                }
                else
                {
                    if (CurrentButtonId > 0)
                    {
                        for (int i = CurrentButtonId - 1; i > 0; i--)
                        {
                            if (i < 0)
                            {
                                i = _buttons.Count - 1;
                            }

                            if (_buttons[i].Active)
                            {
                                nextButtonId = i;
                                break;
                            }
                        }

                        CurrentButtonId = nextButtonId;
                    }
                    else
                    {
                        for (int i = _buttons.Count - 1; i > 0; i--)
                        {
                            if (i < 0)
                            {
                                i = _buttons.Count - 1;
                            }

                            if (_buttons[i].Active)
                            {
                                nextButtonId = i;
                                break;
                            }
                        }

                        CurrentButtonId = nextButtonId;                        
                    }
                }
            }

            if (((Input.IsActionJustPressed(InputActions.INPUT_ACTION_MOVE_DOWN) && Orientation == MenuCategoryOrientation.VERTICAL)
            || (Input.IsActionJustPressed(InputActions.INPUT_ACTION_MOVE_RIGHT) && Orientation == MenuCategoryOrientation.HORIZONTAL))
            && InputType == MenuCategoryInputType.NORMAL)
            {
                if (FirstInput)
                {
                    if (_buttons[0].Active)
                    {
                        CurrentButtonId = 0;
                    }
                    else
                    {
                        CurrentButtonId = 1;
                    }
                    
                    FirstInput = false;
                    return;
                }

                int targetId = CurrentButtonId + 1;
                int nextButtonId = 0; // if all else fails

                if (targetId > _buttons.Count - 1)
                {
                    targetId = 0;
                }

                if (_buttons[targetId].Active)
                {
                    CurrentButtonId = targetId;
                }
                else if (targetId == 0 && !_buttons[0].Active)
                {
                    CurrentButtonId = 1;
                }
                else
                {
                    for (int i = CurrentButtonId + 1; i < _buttons.Count; i++)
                    {
                        if (i > _buttons.Count - 1)
                        {
                            i = 0;
                        }
                        
                        if (_buttons[i].Active)
                        {
                            nextButtonId = i;
                            break;
                        }
                    }

                    CurrentButtonId = nextButtonId;
                }
            }

            // normal button

            if ((Input.IsActionJustPressed(InputActions.INPUT_MENU_CONFIRM) && !FirstInput && _currentButton.IsValid() && _currentButton.IsFocused) ||
            (Input.IsActionJustPressed(InputActions.INPUT_MENU_LEFT_CLICK) && !FirstInput && _currentButton.IsValid() && _currentButton.Active && _currentButton.IsHoveredOver))
            {
                _currentButton.PlayConfirmAudio();
                MenuButton.OnButtonExecuted?.Invoke(_currentButton);
            }

            // color

            if (((Input.IsActionJustPressed(InputActions.INPUT_ACTION_MOVE_LEFT) || Input.IsActionJustPressed(InputActions.INPUT_MENU_DISCARD)) 
            && !FirstInput && _currentButton.IsValid() && _currentButton.ButtonType == MenuButtonType.MENU_BUTTON_COLOR) ||
            (Input.IsActionJustPressed(InputActions.INPUT_MENU_RIGHT_CLICK) 
            && !FirstInput && _currentButton.IsValid() && _currentButton.IsHoveredOver && _currentButton.ButtonType == MenuButtonType.MENU_BUTTON_COLOR))
            {
                MenuButton.OnColorButtonPressedLeft?.Invoke(_currentButton);
                _currentButton.PlayConfirmAudio();
            }

            if (((Input.IsActionJustPressed(InputActions.INPUT_ACTION_MOVE_RIGHT) || Input.IsActionJustPressed(InputActions.INPUT_MENU_CONFIRM)) 
            && !FirstInput && _currentButton.IsValid() && _currentButton.ButtonType == MenuButtonType.MENU_BUTTON_COLOR) ||
            (Input.IsActionJustPressed(InputActions.INPUT_MENU_LEFT_CLICK) 
            && !FirstInput && _currentButton.IsValid() && _currentButton.IsHoveredOver && _currentButton.ButtonType == MenuButtonType.MENU_BUTTON_COLOR))
            {
                MenuButton.OnColorButtonPressedRight?.Invoke(_currentButton);
                _currentButton.PlayConfirmAudio();
            }

            // volume

            if (Input.IsActionJustPressed(InputActions.INPUT_ACTION_MOVE_LEFT) && !FirstInput && _currentButton.IsValid() && _currentButton is VolumeSlider)
            {
                (_currentButton as VolumeSlider).StepLeft();
                _currentButton.PlayConfirmAudio();
            }

            if (Input.IsActionJustPressed(InputActions.INPUT_ACTION_MOVE_RIGHT) && !FirstInput && _currentButton.IsValid() && _currentButton is VolumeSlider)
            {
                (_currentButton as VolumeSlider).StepRight();
                _currentButton.PlayConfirmAudio();
            }

            // locale change

            if (((Input.IsActionJustPressed(InputActions.INPUT_ACTION_MOVE_RIGHT) || Input.IsActionJustPressed(InputActions.INPUT_MENU_CONFIRM)) 
            && !FirstInput && _currentButton.IsValid() && _currentButton.ButtonType == MenuButtonType.MENU_BUTTON_LANGUAGE_OPTIONS) ||
            (Input.IsActionJustPressed(InputActions.INPUT_MENU_LEFT_CLICK) 
            && !FirstInput && _currentButton.IsValid() && _currentButton.IsHoveredOver && _currentButton.ButtonType == MenuButtonType.MENU_BUTTON_LANGUAGE_OPTIONS))
            {
                LocalizationManager.Instance.CurrentLocale += 1;
                _currentButton.PlayConfirmAudio();
            }

            if (((Input.IsActionJustPressed(InputActions.INPUT_ACTION_MOVE_LEFT)) 
            && !FirstInput && _currentButton.IsValid() && _currentButton.ButtonType == MenuButtonType.MENU_BUTTON_LANGUAGE_OPTIONS) ||
            (Input.IsActionJustPressed(InputActions.INPUT_MENU_RIGHT_CLICK) 
            && !FirstInput && _currentButton.IsValid() && _currentButton.IsHoveredOver && _currentButton.ButtonType == MenuButtonType.MENU_BUTTON_LANGUAGE_OPTIONS))
            {
                LocalizationManager.Instance.CurrentLocale -= 1;
                _currentButton.PlayConfirmAudio();
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