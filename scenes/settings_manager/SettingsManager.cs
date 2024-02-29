using Godot;
using System;

namespace Vertigo
{
    public class SettingsManager : Node
    {
        private const string CONFIG_FILE_PATH = "user://config.cfg";
        private const string CONFIG_SECTION = "config";

        private const string CONFIG_KEY_FULLSCREEN = "fullscreen";
        private const string CONFIG_KEY_SCREEN_SHAKE = "screen_shake";
        private const string CONFIG_KEY_TRAIL_EFFECT = "trail_effect";

        private const int CONFIG_FULLSCREEN_DEFAULT = 0;
        private const int CONFIG_SCREEN_SHAKE_DEFAULT = 1;
        private const int CONFIG_TRAIL_EFFECT_DEFAULT = 1;

        [Export] private bool _isFullscreen = false;
        [Export] private bool _isScreenShakeEnabled = true;
        [Export] private bool _isTrailEffectEnabled = true;
        [Export] private LocaleEnum _locale = LocaleEnum.EN;

        public bool IsFullscreen
        {
            get => _isFullscreen;
            set
            {
                _isFullscreen = value;

                OS.WindowFullscreen = value;

                SaveSettings(this);
            }
        }

        public bool IsScreenShakeEnabled
        {
            get => _isScreenShakeEnabled;
            set
            {
                _isScreenShakeEnabled = value;

                SaveSettings(this);
            }
        }

        public bool IsTrailEffectEnabled
        {
            get => _isTrailEffectEnabled;
            set
            {
                _isTrailEffectEnabled = value;

                SaveSettings(this);
            }
        }

        public static SettingsManager Instance { get; private set; }

        public override void _Ready()
        {
            base._Ready();

            Instance = this;

            LoadSettings();
        }

        private void SaveSettings(SettingsManager settings)
        {
            var config = new ConfigFile();

            config.SetValue(CONFIG_SECTION, CONFIG_KEY_FULLSCREEN, Convert.ToInt32(IsFullscreen));
            config.SetValue(CONFIG_SECTION, CONFIG_KEY_SCREEN_SHAKE, Convert.ToInt32(IsScreenShakeEnabled));
            config.SetValue(CONFIG_SECTION, CONFIG_KEY_TRAIL_EFFECT, Convert.ToInt32(IsTrailEffectEnabled));

            config.Save(CONFIG_FILE_PATH);
        }

        private void LoadSettings()
        {
            var config = new ConfigFile();

            Error err = config.Load(CONFIG_FILE_PATH);

            if (err != Error.Ok)
            {
                GD.Print($"Failed loading config file {CONFIG_FILE_PATH}");
                return;
            }

            // fullscreen

            int loadedFullscreen = (int)config.GetValue(CONFIG_SECTION, CONFIG_KEY_FULLSCREEN, CONFIG_FULLSCREEN_DEFAULT);
            loadedFullscreen = Mathf.Clamp(loadedFullscreen, 0, 1);
            IsFullscreen = Convert.ToBoolean(loadedFullscreen);

            // screenshake

            int loadedScreenShake = (int)config.GetValue(CONFIG_SECTION, CONFIG_KEY_SCREEN_SHAKE, CONFIG_SCREEN_SHAKE_DEFAULT);
            loadedScreenShake = Mathf.Clamp(loadedScreenShake, 0, 1);
            IsScreenShakeEnabled = Convert.ToBoolean(loadedScreenShake);

            // trail effect

            int loadedTrailEffect = (int)config.GetValue(CONFIG_SECTION, CONFIG_KEY_TRAIL_EFFECT, CONFIG_TRAIL_EFFECT_DEFAULT);
            loadedTrailEffect = Mathf.Clamp(loadedTrailEffect, 0, 1);
            IsTrailEffectEnabled = Convert.ToBoolean(loadedTrailEffect);
        }
    }
}