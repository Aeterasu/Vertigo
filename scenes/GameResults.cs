using Godot;
using System;

namespace Vertigo
{
    public class GameResults
    {
        public double Time { get; private set; } = 0f;

        public GameResults(double time)
        {
            Time = time;
        }
    }

}