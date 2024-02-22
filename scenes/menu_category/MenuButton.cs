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
        MENU_BUTTON_RESTART,
        MENU_BUTTON_BACK_TO_MENU,
    }

    public class MenuButton : Control
    {
        [Export] public MenuButtonType ButtonType { get; private set; } = MenuButtonType.MENU_BUTTON_START;
        [Export] public bool HasTooltip { get; private set; }
        [Export] public string TooltipTextKey { get; private set; } = "TOOLTIP_DEFAULT";
        [Export] public bool Active { get; private set; } = true;

        private bool _isFocused = false;

        private bool _isHoveredOver = false;

        private Label _label;

        private AnimationPlayer _animationPlayer;

        private TextureRect _tooltip;
        private Label _tooltipLabel;
        private Vector2 _tooltipDefaultPosition = Vector2.Zero;
        private bool _isTooltipHidden = true;

        public event Action<MenuButton> OnButtonMouseEntered;
        public event Action<MenuButton> OnButtonMouseExited;

        public static Action<MenuButton> OnButtonExecuted;
        public static Action<MenuButton> OnColorButtonPressedLeft;
        public static Action<MenuButton> OnColorButtonPressedRight;

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

            _tooltip = GetNode<TextureRect>("Tooltip");
            _tooltipLabel = _tooltip.GetNode<Label>("Label");
            _tooltip.Modulate = new Color(1, 1, 1, 0);
            _tooltipDefaultPosition = _tooltip.RectPosition;
        }

        public override void _PhysicsProcess(float delta)
        {
            base._PhysicsProcess(delta);

            _label.Text = Tr(Enum.GetName(typeof(MenuButtonType), ButtonType));

            if (_isTooltipHidden)
            {
                _tooltip.Modulate = _tooltip.Modulate.LinearInterpolate(new Color(1, 1, 1, 0), 10 * delta);
                _tooltip.RectPosition = _tooltip.RectPosition.LinearInterpolate(_tooltipDefaultPosition + Vector2.Left * 10, 10 * delta);
            }
            else
            {
                _tooltip.Modulate = _tooltip.Modulate.LinearInterpolate(new Color(1, 1, 1, 1), 10 * delta);
                _tooltip.RectPosition = _tooltip.RectPosition.LinearInterpolate(_tooltipDefaultPosition, 10 * delta);
            }

            if (ButtonType == MenuButtonType.MENU_BUTTON_COLOR)
            {
                _tooltipLabel.Text = Tr(TooltipTextKey) + System.Environment.NewLine + Tr(ColorManager.GetCurrentColor.NameKey);
            }
            else if (_isTooltipHidden)
            {
                _tooltipLabel.Text = Tr(TooltipTextKey);
            }
        }

        public void Focus()
        {
            IsFocused = true;

            if (_animationPlayer.IsValid())
            {
                _animationPlayer.Play("Focus");
            }

            if (ButtonType == MenuButtonType.MENU_BUTTON_COLOR || HasTooltip)
            {
                ShowTooltip();
            }
        }

        public void Unfocus()
        {
            IsFocused = false;

            if (_animationPlayer.IsValid())
            {
                _animationPlayer.Play("Unfocus");
            }

            if (ButtonType == MenuButtonType.MENU_BUTTON_COLOR || HasTooltip)
            {
                HideTooltip();
            }
        }

        public void Execute()
        {

        }

        public void ShowTooltip()
        {
            _isTooltipHidden = false;
        }

        public void HideTooltip()
        {
            _isTooltipHidden = true;
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
