using Godot;
using System;

namespace Vertigo
{
    public class Upgrade : Node
    {
        [Export(PropertyHint.Range, "1,32,1")] public int TargetStack { get; private set; } = 1;
        public int Stack { get; private set; } = 0;

        public override void _Ready()
        {
            base._Ready();

            Stack += 1;

            if (TargetStack > 1)
            {
                for (int i = TargetStack; i > 1; i--)
                {
                    AddStack();
                }
            }
        }

        public virtual void AddStack()
        {
            Stack += 1;
        }
    }
}