using Godot;
using System;
using System.Collections.Generic;

namespace Vertigo
{
    public class Intro : Node2D
    {
        [Export] private readonly MainState _targetState = MainState.MAIN_MENU;
        [Export] private readonly float _textRate = 0.8f;
        private float _ticks = 0.0f;

        private int _currentText = 0;

        private List<IntroText> _introTexts = new List<IntroText>();

        public override void _Ready()
        {
            base._Ready();

            GetNode<AnimationPlayer>("AnimationPlayer").Play("Intro");
            GetNode<AnimationPlayer>("AnimationPlayer").Connect("animation_finished", this, "OnAnimationFinished");

            foreach (Node node in GetChildren())
            {
                if (node is IntroText)
                {
                    _introTexts.Add(node as IntroText);
                }
            }
        }

        public override void _PhysicsProcess(float delta)
        {
            base._PhysicsProcess(delta);

            //_ticks += delta;

            if (_ticks >= _textRate)
            {
                _ticks = 0.0f;
                
                if (_currentText < _introTexts.Count)
                {
                    _introTexts[_currentText].Animate();

                    _currentText += 1;
                }
            }
        }

        public override void _Input(InputEvent @event)
        {
            base._Input(@event);

            if (@event is InputEventKey || @event is InputEventJoypadButton || @event is InputEventMouseButton)
            {
                if (@event.IsPressed())
                {
                    GetNode<AnimationPlayer>("AnimationPlayer").Play("Skip");
                }
            }
        }

        private void OnAnimationFinished(string animName)
        {
            Main.Instance.State = _targetState;
        }
    }
}