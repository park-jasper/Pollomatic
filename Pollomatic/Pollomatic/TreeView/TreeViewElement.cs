using System.Windows.Input;
using Xamarin.Forms;

namespace Pollomatic.TreeView
{
    public class TreeViewElement : StackLayout
    {
        public const uint AnimationDelay = 150;
        public bool Expanded { get; set; }
        public string Text { get; set; }
        private readonly Image _toggle;
        private readonly Label _text;
        public TreeViewElement(ICommand expandCollapseCommand, ICommand tappedCommand)
        {
            Orientation = StackOrientation.Horizontal;
            _toggle = new Image()
            {
                Source = ImageSource.FromFile("Assets/ArrowRight.png"),
                GestureRecognizers =
                {
                    new TapGestureRecognizer()
                    {
                        Command = expandCollapseCommand
                    }
                }
            };
            _text = new Label()
            {
                GestureRecognizers =
                {
                    new TapGestureRecognizer()
                    {
                        Command = tappedCommand
                    },
                    new TapGestureRecognizer()
                    {
                        Command = new Command(ShowSelection)
                    }
                }
            };
            Children.Add(_toggle);
            Children.Add(_text);

            PropertyChanged += TreeViewElement_PropertyChanged;
        }

        private void ShowSelection()
        {
            _text.BackgroundColor = Color.Gray;
        }

        public void Unselect()
        {
            _text.BackgroundColor = Color.Transparent;
        }

        private void TreeViewElement_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(Expanded):
                    AnimateRotation(_toggle, Expanded);
                    break;
                case nameof(Text):
                    _text.Text = Text;
                    break;
            }
        }

        private static void AnimateRotation(Image image, bool expanded)
        {
            image.RotateTo(expanded ? 90 : 0, AnimationDelay);
        }
    }
}