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
        MENU_BUTTON_LEADERBOARD_MENU,
        MENU_BUTTON_SOUND_OPTIONS,
        MENU_BUTTON_VISUALS_OPTIONS,
        MENU_BUTTON_LANGUAGE_OPTIONS,
        MENU_BUTTON_OPTIONS_FULLSCREEN,
        MENU_BUTTON_OPTIONS_SCREEN_SHAKE,
        MENU_BUTTON_OPTIONS_TRAIL_EFFECT,
        MENU_BUTTON_OPTIONS_SHOW_TUTORIAL,
        MENU_BUTTON_OPTIONS_SFX_VOLUME,
        MENU_BUTTON_OPTIONS_MUSIC_VOLUME,
        MENU_BUTTON_OPTIONS_BACK_TO_OPTIONS_MENU,
        MENU_BUTTON_PAUSE_RESUME,
        MENU_BUTTON_PAUSE_RESTART,
    }

    public class MenuButton : Control
    {
        [Export] public MenuButtonType ButtonType { get; private set; } = MenuButtonType.MENU_BUTTON_START;
        [Export] public bool HasTooltip { get; private set; }
        [Export] public string TooltipTextKey { get; private set; } = "TOOLTIP_DEFAULT";
        [Export] public bool Active { get; private set; } = true;
        [Export] public AudioStream SelectSound { get; private set; }
        [Export] public AudioStream ConfirmSound { get; private set; }

        private bool _isFocused = false;

        private bool _isHoveredOver = false;

        private Label _label;

        private AnimationPlayer _animationPlayer;

        private TextureRect _tooltip;
        private Label _tooltipLabel;
        private Vector2 _tooltipDefaultPosition = Vector2.Zero;
        private bool _isTooltipHidden = true;

        public bool IsHoveredOver { get => _isHoveredOver; }

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

        public int Id { get; set; } = 0;

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

            if (ButtonType == MenuButtonType.MENU_BUTTON_OPTIONS_FULLSCREEN && OS.GetName() == "HTML5")
            {
                Active = false;
            }
        }

        public override void _PhysicsProcess(float delta)
        {
            base._PhysicsProcess(delta);

            _label.Text = Tr(Enum.GetName(typeof(MenuButtonType), ButtonType));

            if (ButtonType == MenuButtonType.MENU_BUTTON_EXIT && OS.GetName() == "HTML5")
            {
                Active = false;
            }

            if (!Active)
            {
                Modulate = new Color(1, 1, 1, 0.3f);
                return;
            }

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
            else if (ButtonType == MenuButtonType.MENU_BUTTON_LANGUAGE_OPTIONS)
            {
                _tooltipLabel.Text = Tr("MENU_BUTTON_LANGUAGE_OPTIONS") + ":" + System.Environment.NewLine + Tr("LOCALE_" + Enum.GetName(typeof(LocaleEnum), LocalizationManager.Instance.CurrentLocale));
            }
            else if (_isTooltipHidden)
            {
                _tooltipLabel.Text = Tr(TooltipTextKey);
            }
        }

        public void Focus()
        {
            if (!Active)
            {
                return;
            }

            IsFocused = true;

            if (_animationPlayer.IsValid())
            {
                _animationPlayer.Play("Focus");
            }

            if (ButtonType == MenuButtonType.MENU_BUTTON_COLOR || HasTooltip)
            {
                ShowTooltip();
            }

            if (SelectSound != null)
            {
                DeconflictedSoundPlayer.PlayUnconflictingSound(SelectSound, GetNode<Node>("/root/"), 0, 1);
            }
        }

        public void Unfocus()
        {
            if (!Active)
            {
                return;
            }

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

            _isHoveredOver = true;
        }

        private void OnMouseExited()
        {
            OnButtonMouseExited?.Invoke(this);

            _isHoveredOver = false;
        }

        public void PlayConfirmAudio()
        {
            if (ConfirmSound != null)
            {
                DeconflictedSoundPlayer.PlayUnconflictingSound(ConfirmSound, GetNode<Node>("/root/"), 0, 1);
            }
        }
    }
}
