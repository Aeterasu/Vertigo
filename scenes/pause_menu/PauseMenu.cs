using Godot;
using System;

namespace Vertigo
{
    public class PauseMenu : Control
    {
        [Export] private float _lerpWeight = 6f;

        private MenuCategory _menuCategory;

        public bool IsDisplayed { get; set; } = false;

        public override void _Ready()
        {
            base._Ready();

            _menuCategory = GetNode<MenuCategory>("MenuCategory");

            Modulate = new Color(1, 1, 1, 0);
        }

        public override void _Process(float delta)
        {
            base._Process(delta);

            if (IsDisplayed)
            {
                Modulate = Modulate.LinearInterpolate(new Color(1, 1, 1, 1), _lerpWeight * delta);
            }
            else
            {
                Modulate = Modulate.LinearInterpolate(new Color(1, 1, 1, 0), _lerpWeight * delta);
            }

            _menuCategory.DisableInput = !IsDisplayed;
        }
    }
}