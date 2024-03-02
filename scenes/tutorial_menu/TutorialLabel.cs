using Godot;
using System;

namespace Vertigo
{
    public class TutorialLabel : Label
    {
        [Export] public string StringKeyKeyboard { get; private set; } = "TUTORIAL_DEFAULT_KEYBOARD";
        [Export] public string StringKeyGamepad { get; private set; } = "TUTORIAL_DEFAULT_GAMEPAD";

        public override void _Ready()
        {
            base._Ready();

            Autowrap = true;
        }

        public override void _PhysicsProcess(float delta)
        {
            base._PhysicsProcess(delta);

            if (InputDeviceChecker.InputDevice == InputDevice.CONTROLLER)
            {
                Text = Tr(StringKeyGamepad);
            }
            else
            {
                Text = Tr(StringKeyKeyboard);
            }
        }
    }
}