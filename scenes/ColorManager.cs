using Godot;
using System;
using System.Collections.Generic;

namespace Vertigo
{
    public class ColorManager : Node
    {
        [Export] private int _currentColorId = 0;
        [Export] public List<GradientTexture2D> Colors { get; private set; } = new List<GradientTexture2D>();

        public int CurrentColorId
        {
            get => _currentColorId;
            set
            {
                if (value < 0)
                {
                    value = Colors.Count - 1;
                }
                else if (value > Colors.Count - 1)
                {
                    value = 0;
                }

                _currentColorId = value;
            }
        }

        public override void _Ready()
        {
            base._Ready();

            CurrentColorId = _currentColorId;
        }
    }
}
