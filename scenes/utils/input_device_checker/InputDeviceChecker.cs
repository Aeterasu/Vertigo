using Godot;
using System;

namespace Vertigo
{
    public enum InputDevice
    {
        KEYBOARD_MOUSE,
        CONTROLLER,
    }

    public class InputDeviceChecker : Node
    {
        private static InputDevice _inputDevice = InputDevice.KEYBOARD_MOUSE;

        public static event Action<InputDevice> OnInputDeviceUpdate;

        public static InputDevice InputDevice
        {
            get => _inputDevice;
            private set
            {
                if (value != _inputDevice)
                {
                    OnInputDeviceUpdate?.Invoke(value);
                    _inputDevice = value;
                }
            }
        }

        public string JoyName { get; private set; } = "";

        public override void _Ready()
        {
            base._Ready();
        }

        public override void _Input(InputEvent @event)
        {
            base._Input(@event);

            if (@event is InputEventKey || @event is InputEventMouseButton)
            {
                InputDevice = InputDevice.KEYBOARD_MOUSE;
                JoyName = "";
            }
            else if (@event is InputEventJoypadButton || @event is InputEventJoypadMotion)
            {
                InputDevice = InputDevice.CONTROLLER;
                JoyName = Input.GetJoyName(0);
            }
        }
    }
}
