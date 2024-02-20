using Godot;
using System;

namespace Vertigo
{
    public enum MenuButtonType
    {
        MENU_BUTTON_START,
        MENU_BUTTON_COLOR,
        MENU_BUTTON_OPTIONS,
        MENU_BUTTON_LANGUAGE,
        MENU_BUTTON_CREDITS,
        MENU_BUTTON_EXIT,
        MENU_BUTTON_HOW_TO_PLAY,
    }

    public class MenuButton : Control
    {
        [Export] public MenuButtonType ButtonType { get; private set; } = MenuButtonType.MENU_BUTTON_START;
        [Export] public bool Active { get; private set; } = true;

        private bool _isFocused = false;

        private Label _label;

        private AnimationPlayer _animationPlayer;

        public event Action<MenuButton> OnButtonMouseEntered;
        public event Action<MenuButton> OnButtonMouseExited;

        public static Action<MenuButton> OnButtonExecuted;

        public bool IsFocused
        {
            get => _isFocused;
            set
            {
                if (!Active)
                {
                    _isFocused = false;
                    return;
                }

                _isFocused = value;
            }
        }

        public override void _Ready()
        {
            base._Ready();

            Connect("mouse_entered", this, "OnMouseEntered");
            Connect("mouse_exited", this, "OnMouseExited");

            _label = GetNode<Label>("%Label");

            _animationPlayer = GetNodeOrNull<AnimationPlayer>("AnimationPlayer");
        }

        public override void _PhysicsProcess(float delta)
        {
            base._PhysicsProcess(delta);

            _label.Text = Tr(Enum.GetName(typeof(MenuButtonType), ButtonType));
        }

        public void Focus()
        {
            IsFocused = true;

            if (_animationPlayer.IsValid())
            {
                _animationPlayer.Play("Focus");
            }
        }

        public void Unfocus()
        {
            IsFocused = false;

            if (_animationPlayer.IsValid())
            {
                _animationPlayer.Play("Unfocus");
            }
        }

        public void Execute()
        {

        }

        private void OnMouseEntered()
        {
            OnButtonMouseEntered?.Invoke(this);
        }

        private void OnMouseExited()
        {
            OnButtonMouseExited?.Invoke(this);
        }
    }
}
