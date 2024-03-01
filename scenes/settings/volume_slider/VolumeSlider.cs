using Godot;
using System;

namespace Vertigo
{
    public class VolumeSlider : MenuButton
    {
        [Export] private float _step = 0.05f;
        [Export] private AudioBusEnum _audioBus = AudioBusEnum.SFX;

        private HSlider _hSlider;

        public override void _Ready()
        {
            base._Ready();

            _hSlider = GetNode<HSlider>("HSlider");

            _hSlider.Connect("value_changed", this, "OnVolumeDrag");
            
            switch (_audioBus)
            {
                case AudioBusEnum.SFX:
                    _hSlider.Value = SettingsManager.Instance.SfxVolume;
                    break;
                case AudioBusEnum.MUSIC:
                    _hSlider.Value = SettingsManager.Instance.MusicVolume;
                    break;
            }
        }

        private void OnVolumeDrag(float value)
        {
            UpdateVolume();
            PlaySelectAudio();
        }

        private void UpdateVolume()
        {
            switch (_audioBus)
            {
                case AudioBusEnum.SFX:
                    SettingsManager.Instance.SfxVolume = (float)_hSlider.Value;
                    break;
                case AudioBusEnum.MUSIC:
                    SettingsManager.Instance.MusicVolume = (float)_hSlider.Value;
                    break;
            }
        }

        public void StepLeft()
        {
            _hSlider.Value -= _step;

            UpdateVolume();
        }

        public void StepRight()
        {
            _hSlider.Value += _step;

            UpdateVolume();
        }
    }
}