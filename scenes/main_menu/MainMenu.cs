using Godot;
using System;

namespace Vertigo
{
    public class MainMenu : Control
    {
        private MenuCategory _menuCategory;

        private Tween _tween = new Tween();

        public static MainMenu Instance { get; set; }

        public override void _Ready()
        {
            base._Ready();

            _menuCategory = GetNode<MenuCategory>("MenuLayer/MenuCategory");

            AddChild(_tween);

            _tween.InterpolateProperty(GetNode<Control>("MenuLayer/MenuCategory"), "modulate", new Color(1, 1, 1, 0), new Color(1, 1, 1, 1), 0.8f);
            _tween.Start();
        }
    }

}