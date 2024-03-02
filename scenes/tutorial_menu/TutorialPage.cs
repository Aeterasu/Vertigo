using Godot;
using System;

namespace Vertigo
{
    public class TutorialPage : Control
    {
        public int PageNumber { get; set; } = 0;

        public override void _Ready()
        {
            base._Ready();
        }
    }
}