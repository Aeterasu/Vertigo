using Godot;
using System;
using System.Collections.Generic;

namespace Vertigo
{
    public class ColorManager : Node
    {
        [Export] private int _currentColorId = 0;
        [Export] public List<ColorScheme> Colors { get; private set; } = new List<ColorScheme>();

        public static ColorScheme GetCurrentColor { get => Main.Instance.ColorManager.Colors[Main.Instance.ColorManager.CurrentColorId]; }

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
