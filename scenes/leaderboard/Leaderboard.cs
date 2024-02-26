using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Vertigo
{
    public class Leaderboard : Control
    {
        private Node _scoreGetter;

        public bool _finishedRefresh = false;

        private List<LeaderboardPosition> _leaderboardPositions = new List<LeaderboardPosition>();

        private Godot.Collections.Array _scores;

        public override void _Ready()
        {
            base._Ready();

            _scoreGetter = GetNode<Node>("ScoreGetter");
            _scoreGetter.Call("refresh");

            foreach (Node node in GetNode<Control>("TopTenPositions").GetChildren())
            {
                if (node is LeaderboardPosition)
                {
                    _leaderboardPositions.Add(node as LeaderboardPosition);
                }
            }

            GetNode("/root/SilentWolf/Scores").Connect("sw_score_posted", this, "Refresh");
        }

        public override void _PhysicsProcess(float delta)
        {
            base._PhysicsProcess(delta);

            if ((bool)_scoreGetter.Get("finished_refresh") && !_finishedRefresh)
            {
                _finishedRefresh = true;
                _scores = GetNode<Node>("/root/SilentWolf/Scores").Get("scores") as Godot.Collections.Array;

                int i = 0;

                foreach (Godot.Collections.Dictionary score in _scores)
                {
                    _leaderboardPositions[i].Position = i + 1;
                    _leaderboardPositions[i].PlayerName = (string)score["player_name"];
                    _leaderboardPositions[i].Score = (float)score["score"];

                    i++;
                }
            }
        }

        public void Refresh(object data)
        {
            _scoreGetter.Call("refresh");
            _finishedRefresh = false;
        }
    }
}