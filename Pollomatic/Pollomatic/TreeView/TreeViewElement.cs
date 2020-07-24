using System.Windows.Input;
using Xamarin.Forms;

namespace Pollomatic.TreeView
{
    public class TreeViewElement : StackLayout
    {
        public const uint AnimationDelay = 150;
        public bool Expanded { get; set; }
        public string Text { get; set; }
        private Image _toggle;
        private Label _text;
        public TreeViewElement(ICommand expandCollapseCommand)
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
            _text = new Label();
            Children.Add(_toggle);
            Children.Add(_text);

            PropertyChanged += TreeViewElement_PropertyChanged;
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