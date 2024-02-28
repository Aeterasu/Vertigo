using Godot;
using System;

namespace Vertigo
{
    public class MainMenu : Control
    {
        private MenuCategory _menuCategory;

        private Cliff _cliff;

        private Label _pressAnyLabel;

        private Tween _tween = new Tween();

        public static bool IsMenuActive { get; private set; } = false;
        public static MainMenu Instance { get; set; }

        public override void _Ready()
        {
            base._Ready();

            _menuCategory = GetNode<MenuCategory>("MenuLayer/MenuCategory");
            _menuCategory.DisableInput = true;
            _menuCategory.Visible = false;

            _cliff = GetNode<Cliff>("Cliff");

            _pressAnyLabel = GetNode<Label>("MenuLayer/PressAnyKeyLabel");
            _pressAnyLabel.Text = Tr("MENU_PRESS_ANY_KEY");

            AddChild(_tween);

            if (IsMenuActive)
            {
                QuickActivate();
            }
        }

        public override void _PhysicsProcess(float delta)
        {
            base._PhysicsProcess(delta);

            if (_pressAnyLabel.IsValid())
            {
                if (_pressAnyLabel.Modulate.a <= 0.05f)
                {
                    _pressAnyLabel.QueueFree();
                }
            }
        }

        public override void _Input(InputEvent @event)
        {
            base._Input(@event);

            if (@event is InputEventKey || @event is InputEventJoypadButton || @event is InputEventMouseButton)
            {
                if (!IsMenuActive)
                {
                    Activate();
                    GetNode<AudioStreamPlayer>("StartAudio").Play();
                }
            }
        }

        public void Activate()
        {
            IsMenuActive = true;
            
            _menuCategory.DisableInput = false;
            _menuCategory.Visible = true;

            _cliff.Animate();

            _tween.InterpolateProperty(GetNode<Control>("MenuLayer/MenuCategory"), "modulate", new Color(1, 1, 1, 0), new Color(1, 1, 1, 1), 0.5f);
            _tween.InterpolateProperty(_pressAnyLabel, "modulate", new Color(1, 1, 1, 1), new Color(1, 1, 1, 0), 0.25f);
            _tween.Start();
        }

        public void QuickActivate()
        {
            IsMenuActive = true;
            
            _menuCategory.DisableInput = false;
            _menuCategory.Visible = true;

            _cliff.QuickAnimate();

            _tween.InterpolateProperty(GetNode<Control>("MenuLayer/MenuCategory"), "modulate", new Color(1, 1, 1, 0), new Color(1, 1, 1, 1), 0.5f);
            _tween.Start();            

            _pressAnyLabel.Visible = false;
        }
    }
}