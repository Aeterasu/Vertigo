using Godot;
using System;
using System.Collections.Generic;

namespace Vertigo
{
    public class UpgradeMenu : Control
    {
        [Export(PropertyHint.Range, "1, 8, 1")] private int _upgradeCount = 3;
        [Export] private float _quitTimer = 1.5f;
        [Export] private float _quitAnimationDuration = 0.25f;

        private Control _buttonHolder;

        private int _currentSelection = 1;
        private UpgradeButton _currentButton;
        private bool _selectionActivated = false;
        
        private bool _disableInput = false;

        private bool _upgradeConfirmed = false;

        private float _inputDelay = 1f;

        private Label _upgradeNameLabel;
        private Label _upgradeDescriptionLabel;

        private Tween _tween = new Tween();

        private AnimationPlayer _animationPlayer;

        public event Action<UpgradeCard> OnUpgradeSelected;
        public event Action OnMenuClosed;

        public int CurrentSelection
        {
            get => _currentSelection;
            set
            {
                if (value < 0)
                {
                    value = _upgradeCount - 1;
                }
                else if (value > _upgradeCount - 1)
                {
                    value = 0;
                }

                _currentSelection = value;

                foreach (Node node in _buttonHolder.GetChildren())
                {
                    if (node is UpgradeButton)
                    {
                        UpgradeButton button = node as UpgradeButton;
                        if (button.Id == value)
                        {
                            button.IsFocused = true;
                            _currentButton = button;

                            _upgradeNameLabel.Visible = true;
                            _upgradeNameLabel.Text = Tr(button.UpgradeCard.NameKey);

                            _upgradeDescriptionLabel.Visible = true;
                            _upgradeDescriptionLabel.Text = Tr(button.UpgradeCard.DescriptionKey);
                        }
                        else
                        {
                            button.IsFocused = false;
                        }
                    }
                }
            }
        }

        public override void _Ready()
        {
            base._Ready();

            _buttonHolder = GetNode<Control>("%Buttons");

            _upgradeNameLabel = GetNode<Label>("%UpgradeNameLabel");
            _upgradeDescriptionLabel = GetNode<Label>("%UpgradeDescriptionLabel");
            _upgradeNameLabel.Visible = false;
            _upgradeDescriptionLabel.Visible = false;

            AddChild(_tween);

            _animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
            _animationPlayer.Play("Appear");
            _inputDelay = _animationPlayer.CurrentAnimationLength;
            _animationPlayer.Connect("animation_finished", this, "OnAnimationFinished");

            if (UpgradeManager.Instance.IsValid())
            {
                RandomizeUpgrades(UpgradeManager.Instance);
            }
        }

        public override void _PhysicsProcess(float delta)
        {
            base._PhysicsProcess(delta);

            _inputDelay = Mathf.Max(_inputDelay - delta, 0);

            if (_upgradeConfirmed)
            {
                _quitTimer -= delta;
            }

            if (_quitTimer <= 0)
            {
                if (_currentButton.IsValid())
                {
                    OnUpgradeSelected?.Invoke(_currentButton.UpgradeCard);
                }

                OnMenuClosed?.Invoke();
                QueueFree();
            }

            if (_disableInput)
            {
                return;
            }

            if (Input.IsActionJustPressed(InputActions.INPUT_ACTION_MOVE_LEFT) && _inputDelay <= 0)
            {
                CurrentSelection -= 1;
                _selectionActivated = true;
            }

            if (Input.IsActionJustPressed(InputActions.INPUT_ACTION_MOVE_RIGHT) && _inputDelay <= 0)
            {
                CurrentSelection += 1;
                _selectionActivated = true;
            }
            
            if (Input.IsActionJustPressed(InputActions.INPUT_ACTION_CONFIRM) && _selectionActivated && _inputDelay <= 0)
            {
                _disableInput = true;
                _upgradeConfirmed = true;

                if (!_currentButton.IsValid())
                {
                    return;
                }

                _currentButton.Confirm();

                foreach (Node node in _buttonHolder.GetChildren())
                {
                    if (node is UpgradeButton)
                    {
                        UpgradeButton button = node as UpgradeButton;
                        if (button != _currentButton)
                        {
                            button.Discard();
                        }
                    }
                }

                _tween.InterpolateProperty(this, "modulate", new Color(1, 1, 1, 1), new Color(1, 1, 1, 0), _quitAnimationDuration, Tween.TransitionType.Linear, Tween.EaseType.InOut, _quitTimer - _quitAnimationDuration);
                _tween.Start();
            }
        }

        private void RandomizeUpgrades(UpgradeManager upgradeManager)
        {
            List<UpgradeCard> pool = new List<UpgradeCard>(upgradeManager.UpgradePool);

            if (pool.Count == 0)
            {
                return;
            }

            Random random = new Random();

            foreach (Node node in _buttonHolder.GetChildren())
            {
                if (node is UpgradeButton)
                {
                    UpgradeButton button = node as UpgradeButton;
                    int randomUpgradeId = random.Next(pool.Count);
                    button.UpgradeCard = pool[randomUpgradeId];
                    pool.RemoveAt(randomUpgradeId);

                    if (pool.Count <= 0)
                    {
                        pool = new List<UpgradeCard>(upgradeManager.UpgradePool);
                    }
                }
            }
        }

        private void OnAnimationFinished(string animName)
        {
            _disableInput = false;
        }
    }
}