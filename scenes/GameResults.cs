using Godot;
using System;

namespace Vertigo
{
    public class GameResults
    {
        public float Time { get; private set; } = 0f;

        public GameResults(float time)
        {
            Time = time;
        }
    }

}