using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Vertigo
{
    public class Leaderboard : Control
    {
        [Export] private readonly int _visibleHighscores = 10;
        [Export] private bool _generateNewPositions = false;

        private Node _scoreGetter;
        private Node _scoreSender;

        public bool _finishedRefresh = false;

        private readonly List<LeaderboardPosition> _leaderboardPositions = new List<LeaderboardPosition>();
        private LeaderboardPosition _initialLeaderboardPosition;
        private LeaderboardPosition _currentLeaderboardPosition;

        private Control _highscoresParent;

        private Godot.Collections.Array _scores;

        private Node2D _loadingArc;

        public float TotalYOffset { get; private set; } = 0f;

        public bool IsRefreshFinished { get => _finishedRefresh; }

        public override void _Ready()
        {
            base._Ready();

            _loadingArc = GetNode<Node2D>("LoadingArc");

            _highscoresParent = GetNode<Control>("TopTenPositions");

            _scoreGetter = GetNode<Node>("ScoreGetter");
            _scoreSender = GetNode<Node>("ScoreSender");
            _scoreGetter.Call("refresh");

            _currentLeaderboardPosition = GetNode<LeaderboardPosition>("LeaderboardCurrentPosition");

            foreach (Node node in _highscoresParent.GetChildren())
            {
                if (node is LeaderboardPosition)
                {
                    _leaderboardPositions.Add(node as LeaderboardPosition);
                }
            }

            _initialLeaderboardPosition = _leaderboardPositions[0];

            GetNode("/root/SilentWolf/Scores").Connect("sw_score_posted", this, "Refresh");

            Label label = GetNodeOrNull<Label>("Label");

            if (label.IsValid())
            {
                label.Text = Tr("LEADERBOARD_TEXT");
            }

            LineEdit lineEdit = GetNodeOrNull<LineEdit>("LineEdit");

            if (lineEdit.IsValid())
            {
                lineEdit.PlaceholderText = Tr("LEADERBOARD_PLACEHOLDER");
            }

            Button button = GetNodeOrNull<Button>("ScoreSubmitButton");

            if (button.IsValid())
            {
                button.Text = Tr("LEADERBOARD_SUBMIT");
            }
        }

        public override void _PhysicsProcess(float delta)
        {
            base._PhysicsProcess(delta);

            if ((bool)_scoreGetter.Get("finished_refresh") && !_finishedRefresh)
            {
                _finishedRefresh = true;
                _scores = GetNode<Node>("/root/SilentWolf/Scores").Get("scores") as Godot.Collections.Array;

                int i = 0;
                int j = 0;

                foreach (Godot.Collections.Dictionary score in _scores)
                {
                    if (i <= _visibleHighscores - 1 && i < _leaderboardPositions.Count)
                    {
                        if (_generateNewPositions)
                        {
                            LeaderboardPosition leaderboardPosition = _initialLeaderboardPosition.Duplicate() as LeaderboardPosition;
                            _leaderboardPositions.Add(leaderboardPosition);
                            leaderboardPosition.RectPosition = new Vector2(12f, 24f) + Vector2.Down * 18 * (i + 1);
                            TotalYOffset = _scores.Count > 10 ? 18 * (i - 10) : 0f;
                            _highscoresParent.AddChild(leaderboardPosition);
                        }

                        _leaderboardPositions[i].Position = i + 1;
                        _leaderboardPositions[i].PlayerName = (string)score["player_name"];
                        _leaderboardPositions[i].Score = (float)score["score"];

                        if (Equals(score["score_id"], _scoreSender.Get("score_id")))
                        {
                            _leaderboardPositions[i].Highlight();
                        }

                        i++;
                    }
                    else
                    {
                        if (Equals(score["score_id"], _scoreSender.Get("score_id")))
                        {
                            _currentLeaderboardPosition.Position = j + 1;
                            _currentLeaderboardPosition.PlayerName = (string)score["player_name"];
                            _currentLeaderboardPosition.Score = (float)score["score"];
                            _currentLeaderboardPosition.Highlight();
                        }
                    }

                    j++;
                }
            }

            _loadingArc.Visible = !IsRefreshFinished;
        }

        public void Refresh(object data)
        {
            _scoreGetter.Call("refresh");
            _finishedRefresh = false;
        }

        public void Submit(string playerName, double score)
        {
            _scoreSender.Call("send_score", playerName, score);
        }
    }
}