using Godot;
using System;

namespace Vertigo
{
    public class TutorialRichTextLabel : RichTextLabel
    {
        [Export] public string StringKeyKeyboard { get; private set; } = "TUTORIAL_DEFAULT_KEYBOARD";
        [Export] public string StringKeyGamepad { get; private set; } = "TUTORIAL_DEFAULT_GAMEPAD";

        public override void _Ready()
        {
            base._Ready();
        }

        public override void _PhysicsProcess(float delta)
        {
            base._PhysicsProcess(delta);

            if (InputDeviceChecker.InputDevice == InputDevice.CONTROLLER)
            {
                BbcodeText = "[center]" + Tr(StringKeyGamepad);
            }
            else
            {
                BbcodeText = "[center]" + Tr(StringKeyKeyboard);
            }
        }
    }
}