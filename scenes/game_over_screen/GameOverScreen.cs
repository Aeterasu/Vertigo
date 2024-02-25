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
        private LineEdit _lineEdit;

        public GameResults GameResults { get; set; } = new GameResults(0);

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
            _lineEdit = GetNode<LineEdit>("Control/Leaderboard/LineEdit");
        }

        public override void _PhysicsProcess(float delta)
        {
            base._PhysicsProcess(delta);
        }

        private void OnFocusChanged(Control control)
        {
            _menuCategory.DisableInput = control == _lineEdit;
        }
    }
}