using System.Windows;

namespace Interface.Windows
{
    /// <summary>
    /// Логика взаимодействия для Message.xaml
    /// </summary>
    public partial class Message : Window
    {
        public enum WindowType
        {
            Ok,
            YesNo
        }

        public Message(string message, WindowType windowType)
        {
            InitializeComponent();
            TextBlockMessage.Text = message;
            SetupEventHandlers();

            if (windowType == WindowType.Ok)
            {
                ButtonYes.Visibility = Visibility.Collapsed;
                ButtonNo.Visibility = Visibility.Collapsed;
            }
        }

        private void SetupEventHandlers()
        {
            ButtonYes.Click += (_, __) =>
            {
                DialogResult = true;
                Close();
            };
            ButtonNo.Click += (_, __) =>
            {
                DialogResult = false;
                Close();
            };
        }
    }
}
