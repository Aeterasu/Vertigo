using Godot;
using System;

namespace Vertigo
{
    public class GameOverScreen : Control
    {
        private AnimationPlayer _animationPlayer;

        private Label _currentScoreLabel;
        private Label _highscoreLabel;
        
        private MenuCategory _menuCategory;
        private Button _scoreSubmitButton;
        private LineEdit _lineEdit;

        private Leaderboard _leaderboard;
        private Timer _refreshTimer = new Timer();

        public bool ScoreSubmitted { get; private set; } = false;

        public override void _Ready()
        {
            base._Ready();

            GetViewport().Connect("gui_focus_changed", this, "OnFocusChanged");

            _animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
            _animationPlayer.Play("Init");

            _currentScoreLabel = GetNode<Label>("Control/CurrentScore/Label");
            _highscoreLabel = GetNode<Label>("Control/Highscore/Label");

            if (GameResultsManager.Instance.IsValid())
            {
                if (GameResultsManager.Instance.CurrentGameResults != null)
                {
                    _currentScoreLabel.Text = Tr("RESULTS_SCORE") + " " + StringFormat.SecondsToStringFormatted(GameResultsManager.Instance.CurrentGameResults.Time);
                }

                if (GameResultsManager.Instance.HighscoreResults != null)
                {
                    _highscoreLabel.Text = Tr("RESULTS_HIGHSCORE") + " " + StringFormat.SecondsToStringFormatted(GameResultsManager.Instance.HighscoreResults.Time);
                }
            }

            _menuCategory = GetNode<MenuCategory>("Control/MenuCategory");
            _scoreSubmitButton = GetNode<Button>("Control/Leaderboard/ScoreSubmitButton");
            _lineEdit = GetNode<LineEdit>("Control/Leaderboard/LineEdit");

            _scoreSubmitButton.Connect("pressed", this, "OnScoreSubmitButtonPressed");

            _leaderboard = GetNode<Leaderboard>("Control/Leaderboard");
        }

        public override void _PhysicsProcess(float delta)
        {
            base._PhysicsProcess(delta);

            _scoreSubmitButton.Disabled = _lineEdit.Text == "" || ScoreSubmitted;
            _lineEdit.Editable = !ScoreSubmitted;
        }

        private void OnFocusChanged(Control control)
        {
            _menuCategory.DisableInput = control == _lineEdit;
        }

        private void OnScoreSubmitButtonPressed()
        {
            if (GameResultsManager.Instance == null)
            {
                return;
            }

            if (GameResultsManager.Instance.CurrentGameResults == null)
            {
                return;
            }

            GameResults currentResults = GameResultsManager.Instance.CurrentGameResults;

            GetNode<Node>("/root/SilentWolf/Scores").Call("persist_score", _lineEdit.Text, Math.Round(currentResults.Time, 2));

            ScoreSubmitted = true;
            _lineEdit.Text = "";
        }
    }
}