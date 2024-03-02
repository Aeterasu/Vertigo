using Godot;
using System;

namespace Vertigo
{
    public class GameResultsManager : Node
    {
        public const string SCORE_SAVEFILE_PATH = "user://score_data.save";

        private GameResults _currentGameResults = new GameResults(0);
        private GameResults _highscoreResults = new GameResults(0);

        public GameResults CurrentGameResults
        {
            get => _currentGameResults;
            set
            {
                _currentGameResults = value;

                if (value.Time > HighscoreResults.Time)
                {
                    HighscoreResults = value;
                    _currentGameResults.NewHighscore = true;
                }
            }
        }
        
        public GameResults HighscoreResults
        {
            get => _highscoreResults;
            set
            {
                _highscoreResults = value;
                SaveHighscore();
            }
        }

        public static GameResultsManager Instance { get; private set; }

        public override void _Ready()
        {
            base._Ready();

            Instance = this;

            LoadHighscore();
        }

        public void LoadHighscore()
        {
            var saveGame = new File();
            if (!saveGame.FileExists(SCORE_SAVEFILE_PATH))
            {
                return;
            }

            Error err = saveGame.Open(SCORE_SAVEFILE_PATH, File.ModeFlags.Read);

            if (err != Error.Ok)
            {
                return;
            }

            HighscoreResults = new GameResults(saveGame.GetDouble());
            saveGame.Close();
        }

        public void SaveHighscore()
        {
            var saveGame = new File();
            Error err = saveGame.Open(SCORE_SAVEFILE_PATH, File.ModeFlags.Write);

            if (err != Error.Ok)
            {
                return;
            }

            saveGame.StoreDouble(HighscoreResults.Time);
            saveGame.Close();
        }
    }
}