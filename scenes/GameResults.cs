using Godot;
using System;

namespace Vertigo
{
    public class GameResults
    {
        public double Time { get; private set; } = 0f;
        public bool NewHighscore { get; set; } = false;

        public GameResults(double time)
        {
            Time = time;
        }
    }

}