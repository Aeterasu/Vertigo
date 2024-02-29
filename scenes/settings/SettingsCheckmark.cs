using Godot;
using System;

namespace Vertigo
{
    public enum SettingsCheckmarkEnum
    {
        FULLSCREEN,
        SCREEN_SHAKE,
        TRAIL_EFFECT,
        SHOW_TUTORIAL,
    }

    public class SettingsCheckmark : TextureRect
    {
        [Export] private SettingsCheckmarkEnum _setting;
        [Export] private Texture _enabledTexture;
        [Export] private Texture _disabledTexture;

        public override void _Ready()
        {
            base._Ready();
        }

        public override void _Process(float delta)
        {
            base._Process(delta);

            if (!SettingsManager.Instance.IsValid())
            {
                return;
            }
            
            switch (_setting)
            {
                case SettingsCheckmarkEnum.FULLSCREEN:
                    Texture = SettingsManager.Instance.IsFullscreen ? _enabledTexture : _disabledTexture;
                    break;
                case SettingsCheckmarkEnum.SCREEN_SHAKE:
                    Texture = SettingsManager.Instance.IsScreenShakeEnabled ? _enabledTexture : _disabledTexture;
                    break;
                case SettingsCheckmarkEnum.TRAIL_EFFECT:
                    Texture = SettingsManager.Instance.IsTrailEffectEnabled ? _enabledTexture : _disabledTexture;
                    break;
            }
        }
    }
}