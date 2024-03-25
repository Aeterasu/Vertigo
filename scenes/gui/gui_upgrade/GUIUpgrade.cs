using Godot;
using System;

namespace Vertigo
{
    public class GUIUpgrade : Control
    {
        public Upgrade Upgrade { get; set; }

        public override void _Ready()
        {
            base._Ready();
        }

        public void PlayAnimation()
        {
            GetNode<AnimationPlayer>("AnimationPlayer").Play("Purchase");
        }

        public void SetStack(int stack)
        {
            Label label = GetNode<Label>("StackLabel");

            label.Text = stack.ToString();

            label.Visible = stack > 1;
        }

        public void SetUpgrade(UpgradeCard card)
        {
            GetNode<Sprite>("Sprite").Frame = card.TextureAtlasId;
        }
    }
}