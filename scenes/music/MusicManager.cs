using Godot;
using System;

namespace Vertigo
{
    public class MusicManager : Node
    {
        [Export] private readonly float _fadeOutTime = 0.5f;
        [Export] private readonly float _fadeInTime = 0.25f;

        private AudioStreamPlayer _musicMenu;
        private float _menuMusicDefaultVolume = 0.0f;

        private AudioStreamPlayer _musicGame;
        private float _gameMusicDefaultVolume = 0.0f;

        private Tween _tween = new Tween();

        public bool Muffle { get; set; } = false;

        public bool MenuMusicStopped { get; set; } = true;
        public bool GameMusicStopped { get; set; } = true;
 
        public override void _Ready()
        {
            base._Ready();

            _musicMenu = GetNode<AudioStreamPlayer>("MusicMenu");
            _menuMusicDefaultVolume = _musicMenu.VolumeDb;
            
            _musicGame = GetNode<AudioStreamPlayer>("MusicGame");
            _gameMusicDefaultVolume = _musicGame.VolumeDb;

            AddChild(_tween);
        }

        public override void _PhysicsProcess(float delta)
        {
            base._PhysicsProcess(delta);

            AudioServer.SetBusEffectEnabled(AudioServer.GetBusIndex(AudioBuses.AUDIO_BUS_MUSIC), 0, Muffle);
        }

        public void StartMusicMenu()
        {
            if (!MenuMusicStopped)
            {
                return;
            }

            _musicMenu.VolumeDb = -80;

            _tween.InterpolateProperty(_musicMenu, "volume_db", -80, _menuMusicDefaultVolume, _fadeInTime);
            _tween.Start();

            _musicMenu.Play(0.0f);

            MenuMusicStopped = false;
        }

        public void StartMusicGame()
        {
            if (!GameMusicStopped)
            {
                return;
            }

            _musicGame.VolumeDb = -80;

            _tween.InterpolateProperty(_musicGame, "volume_db", -80, _gameMusicDefaultVolume, _fadeInTime);
            _tween.Start();

            _musicGame.Play(0.0f);

            GameMusicStopped = false;
        }

        public void StopMusicMenu()
        {
            _tween.InterpolateProperty(_musicMenu, "volume_db", _musicMenu.VolumeDb, -80, _fadeOutTime);
            _tween.Start();     

            MenuMusicStopped = true;       
        }

        public void StopMusicGame()
        {
            _tween.InterpolateProperty(_musicGame, "volume_db", _musicGame.VolumeDb, -80, _fadeOutTime);
            _tween.Start();           

            GameMusicStopped = true; 
        }
    }
}