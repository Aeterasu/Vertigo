using Godot;
using System;

namespace Vertigo
{
    public enum SettingsMenuState
    {
        DEFAULT,
        VISUALS,
        SOUND,
    }

    public class SettingsMenu : Control
    {
        [Export] private SettingsMenuState _state = SettingsMenuState.DEFAULT;

        private MenuCategory _defaultCategory;
        private MenuCategory _visualsCategory;
        private MenuCategory _soundCategory;

        public override void _Ready()
        {
            base._Ready();

            _defaultCategory = GetNode<MenuCategory>("DefaultCategory");
            _visualsCategory = GetNode<MenuCategory>("VisualsCategory");
            _soundCategory = GetNode<MenuCategory>("SoundCategory");

            MenuButton.OnButtonExecuted += OnMenuButtonExecuted;
        }

        public override void _PhysicsProcess(float delta)
        {
            base._PhysicsProcess(delta);

            _defaultCategory.Visible = _state == SettingsMenuState.DEFAULT;
            _defaultCategory.DisableInput = !(_state == SettingsMenuState.DEFAULT);

            _visualsCategory.Visible = _state == SettingsMenuState.VISUALS;
            _visualsCategory.DisableInput = !(_state == SettingsMenuState.VISUALS);

            _soundCategory.Visible = _state == SettingsMenuState.SOUND;
            _soundCategory.DisableInput = !(_state == SettingsMenuState.SOUND);
        }

        private void OnMenuButtonExecuted(MenuButton button)
        {
            if (button.ButtonType == MenuButtonType.MENU_BUTTON_OPTIONS_BACK_TO_OPTIONS_MENU)
            {
                _state = SettingsMenuState.DEFAULT;
            }

            if (button.ButtonType == MenuButtonType.MENU_BUTTON_VISUALS_OPTIONS)
            {
                _state = SettingsMenuState.VISUALS;
            }

            if (button.ButtonType == MenuButtonType.MENU_BUTTON_SOUND_OPTIONS)
            {
                _state = SettingsMenuState.SOUND;
            }
        }
    }
}