using Godot;
using System;
using System.Collections.Generic;

namespace Vertigo
{
    public class TutorialMenu : Control
    {
        private MenuButton _previousPageButton;
        private MenuButton _nextPageButton;

        private string _previousPageLabelKey;
        private string _nextPageLabelKey;

        private int _currentPageNumber = 0;

        private List<TutorialPage> _pages = new List<TutorialPage>();

        public int CurrentPageNumber
        {
            get => _currentPageNumber;
            private set
            {
                _currentPageNumber = value;

                if ((_currentPageNumber < 0) || (_currentPageNumber > _pages.Count - 1))
                {
                    Main.Instance.State = MainState.MAIN_MENU;
                }
            }
        }

        public int MaxPageCount
        {
            get => _pages.Count - 1;
        }

        public override void _Ready()
        {
            base._Ready();

            _previousPageButton = GetNode<MenuButton>("MenuCategory/PreviousPage");
            _nextPageButton = GetNode<MenuButton>("MenuCategory/NextPage");

            _previousPageButton.StopTextUpdate = true;
            _nextPageButton.StopTextUpdate = true;

            foreach (Node node in GetNode<Node>("TutorialPages").GetChildren())
            {
                if (node is TutorialPage)
                {
                    _pages.Add(node as TutorialPage);
                }
            }
        }

        public override void _PhysicsProcess(float delta)
        {
            base._PhysicsProcess(delta);

            if (CurrentPageNumber <= 0)
            {
                _previousPageButton.GetLabel.Text = Tr("MENU_BUTTON_BACK_TO_MENU"); 
            }
            else
            {
                _previousPageButton.GetLabel.Text = Tr(Enum.GetName(typeof(MenuButtonType), _previousPageButton.ButtonType));
            }

            if (CurrentPageNumber >= _pages.Count - 1)
            {
                _nextPageButton.GetLabel.Text = Tr("MENU_BUTTON_BACK_TO_MENU"); 
            }
            else
            {
                _nextPageButton.GetLabel.Text = Tr(Enum.GetName(typeof(MenuButtonType), _nextPageButton.ButtonType));
            }

            foreach (TutorialPage page in _pages)
            {
                page.Visible = _pages.IndexOf(page) == CurrentPageNumber;
            }
        }

        public void NextPage()
        {
            CurrentPageNumber += 1;

            var animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");

            animationPlayer.Stop();
            animationPlayer.Play("PageChange");
        }

        public void PreviousPage()
        {
            CurrentPageNumber -= 1;

            var animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");

            animationPlayer.Stop();
            animationPlayer.Play("PageChange");
        }
    }
}