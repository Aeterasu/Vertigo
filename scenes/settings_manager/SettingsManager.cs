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
        private const string CONFIG_KEY_COLOR = "color";
        private const string CONFIG_KEY_SFX_VOLUME = "sfx_volume";
        private const string CONFIG_KEY_MUSIC_VOLUME = "music_volume";
        private const string CONFIG_KEY_LOCALE = "locale";

        private const int CONFIG_FULLSCREEN_DEFAULT = 0;
        private const int CONFIG_SCREEN_SHAKE_DEFAULT = 1;
        private const int CONFIG_TRAIL_EFFECT_DEFAULT = 1;
        private const int CONFIG_COLOR_DEFAULT = 0;
        private const float CONFIG_SFX_VOLUME_DEFAULT = 0.3f;
        private const float CONFIG_MUSIC_VOLUME_DEFAULT = 0.3f;
        private const string CONFIG_LOCALE_DEFAULT = LocalizationManager.LOCALE_EN;

        private bool _isFullscreen = false;
        private bool _isScreenShakeEnabled = true;
        private bool _isTrailEffectEnabled = true;
        private float _sfxVolume = CONFIG_SFX_VOLUME_DEFAULT;
        private float _musicVolume = CONFIG_MUSIC_VOLUME_DEFAULT;

        public bool IsFullscreen
        {
            get => _isFullscreen;
            set
            {
                _isFullscreen = value;

                OS.WindowFullscreen = value;

                SaveSettings();
            }
        }

        public bool IsScreenShakeEnabled
        {
            get => _isScreenShakeEnabled;
            set
            {
                _isScreenShakeEnabled = value;

                SaveSettings();
            }
        }

        public bool IsTrailEffectEnabled
        {
            get => _isTrailEffectEnabled;
            set
            {
                _isTrailEffectEnabled = value;

                SaveSettings();
            }
        }

        public float SfxVolume
        {
            get => _sfxVolume;
            set
            {
                _sfxVolume = Mathf.Clamp(value, 0.0f, 1.0f);

                AudioServer.SetBusVolumeDb(AudioServer.GetBusIndex(AudioBuses.AUDIO_BUS_SFX), GD.Linear2Db(value));

                SaveSettings();
            }
        }

        public float MusicVolume
        {
            get => _musicVolume;
            set
            {
                _musicVolume = Mathf.Clamp(value, 0.0f, 1.0f);

                AudioServer.SetBusVolumeDb(AudioServer.GetBusIndex(AudioBuses.AUDIO_BUS_MUSIC), GD.Linear2Db(value));

                SaveSettings();
            }
        }

        public static SettingsManager Instance { get; private set; }

        public override void _Ready()
        {
            base._Ready();

            Instance = this;

            LoadSettings();
        }

        public void SaveSettings()
        {
            var config = new ConfigFile();

            config.SetValue(CONFIG_SECTION, CONFIG_KEY_FULLSCREEN, Convert.ToInt32(IsFullscreen));
            config.SetValue(CONFIG_SECTION, CONFIG_KEY_SCREEN_SHAKE, Convert.ToInt32(IsScreenShakeEnabled));
            config.SetValue(CONFIG_SECTION, CONFIG_KEY_TRAIL_EFFECT, Convert.ToInt32(IsTrailEffectEnabled));
            config.SetValue(CONFIG_SECTION, CONFIG_KEY_COLOR, Convert.ToInt32(GetParent<Main>().GetNode<ColorManager>("ColorManager").CurrentColorId));

            config.SetValue(CONFIG_SECTION, CONFIG_KEY_SFX_VOLUME, SfxVolume);
            config.SetValue(CONFIG_SECTION, CONFIG_KEY_MUSIC_VOLUME, MusicVolume);

            config.SetValue(CONFIG_SECTION, CONFIG_KEY_LOCALE, LocalizationManager.LanguageEnumToString(GetParent<Main>().GetNode<LocalizationManager>("LocalizationManager").CurrentLocale));

            config.Save(CONFIG_FILE_PATH);
        }

        public void LoadSettings()
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

            // color scheme

            var colorManager = GetParent<Main>().GetNode<ColorManager>("ColorManager");

            int loadedColor = (int)config.GetValue(CONFIG_SECTION, CONFIG_KEY_COLOR, CONFIG_COLOR_DEFAULT);
            loadedColor = Mathf.Clamp(loadedColor, 0, colorManager.Colors.Count - 1);
            colorManager.CurrentColorId = loadedColor;

            // sfx volume

            float loadedSfxVolume = (float)config.GetValue(CONFIG_SECTION, CONFIG_KEY_SFX_VOLUME, CONFIG_SFX_VOLUME_DEFAULT);
            loadedSfxVolume = Mathf.Clamp(loadedSfxVolume, 0.0f, 1.0f);
            SfxVolume = loadedSfxVolume;

            // music volume

            float loadedMusicVolume = (float)config.GetValue(CONFIG_SECTION, CONFIG_KEY_MUSIC_VOLUME, CONFIG_MUSIC_VOLUME_DEFAULT);
            loadedMusicVolume = Mathf.Clamp(loadedMusicVolume, 0.0f, 1.0f);
            MusicVolume = loadedMusicVolume;

            // locale

            string loadedLocale = (string)config.GetValue(CONFIG_SECTION, CONFIG_KEY_LOCALE, CONFIG_LOCALE_DEFAULT);
            GetParent<Main>().GetNode<LocalizationManager>("LocalizationManager").CurrentLocale = LocalizationManager.LanguageStringToEnum(loadedLocale);
        }
    }
}