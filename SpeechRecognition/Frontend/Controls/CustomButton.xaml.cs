using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Interface.Controls
{
    public partial class CustomButton : UserControl
    {
        public static readonly DependencyProperty ControlContentProperty =
            DependencyProperty.Register("ControlContent", typeof(string), typeof(CustomButton), new PropertyMetadata("Button"));

        public string ControlContent
        {
            get { return (string)GetValue(ControlContentProperty); }
            set { SetValue(ControlContentProperty, value); }
        }

        public static readonly DependencyProperty ControlBackgroundProperty =
            DependencyProperty.Register("ControlBackground", typeof(Brush), typeof(CustomButton), new PropertyMetadata(null));

        public Brush ControlBackground
        {
            get { return (Brush)GetValue(ControlBackgroundProperty); }
            set { SetValue(ControlBackgroundProperty, value); }
        }

        public static readonly DependencyProperty ControlBorderThicknessProperty =
    DependencyProperty.Register("ControlBorderThickness", typeof(string), typeof(CustomButton), new PropertyMetadata("0"));

        public string ControlBorderThickness
        {
            get { return (string)GetValue(ControlBorderThicknessProperty); }
            set { SetValue(ControlBorderThicknessProperty, value); }
        }

        public static readonly DependencyProperty ControlBorderBrushProperty =
    DependencyProperty.Register("ControlBorderBrush", typeof(Brush), typeof(CustomButton), new PropertyMetadata(null));

        public Brush ControlBorderBrush
        {
            get { return (Brush)GetValue(ControlBorderBrushProperty); }
            set { SetValue(ControlBorderBrushProperty, value); }
        }

        public static readonly DependencyProperty ControlForegroundProperty =
            DependencyProperty.Register("ControlForeground", typeof(Brush), typeof(CustomButton), new PropertyMetadata(null));

        public Brush ControlForeground
        {
            get { return (Brush)GetValue(ControlForegroundProperty); }
            set { SetValue(ControlForegroundProperty, value); }
        }

        public static readonly DependencyProperty ControlFontSizeProperty =
            DependencyProperty.Register("ControlFontSize", typeof(string), typeof(CustomButton), new PropertyMetadata("5"));

        public string ControlFontSize
        {
            get { return (string)GetValue(ControlFontSizeProperty); }
            set { SetValue(ControlFontSizeProperty, value); }
        }

        public static readonly DependencyProperty ControlFontWeightProperty =
            DependencyProperty.Register("ControlFontWeight", typeof(string), typeof(CustomButton), new PropertyMetadata("Regular"));

        public string ControlFontWeight
        {
            get { return (string)GetValue(ControlFontWeightProperty); }
            set { SetValue(ControlFontWeightProperty, value); }
        }

        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register("ControlCornerRadius", typeof(string), typeof(CustomButton), new PropertyMetadata("0"));

        public string ControlCornerRadius
        {
            get { return (string)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }

        public static readonly DependencyProperty ControlMouseOverBackgroundProperty =
            DependencyProperty.Register("ControlMouseOverBackground", typeof(Brush), typeof(CustomButton), new PropertyMetadata(null));

        public Brush ControlMouseOverBackground
        {
            get { return (Brush)GetValue(ControlMouseOverBackgroundProperty); }
            set { SetValue(ControlMouseOverBackgroundProperty, value); }
        }

        public static readonly DependencyProperty ControlMouseOverBorderBrushProperty =
            DependencyProperty.Register("ControlMouseOverBorderBrush", typeof(Brush), typeof(CustomButton), new PropertyMetadata(null));

        public Brush ControlMouseOverBorderBrush
        {
            get { return (Brush)GetValue(ControlMouseOverBorderBrushProperty); }
            set { SetValue(ControlMouseOverBorderBrushProperty, value); }
        }

        public static readonly DependencyProperty MousePressedMarginProperty =
            DependencyProperty.Register("ControlMousePressedMargin", typeof(string), typeof(CustomButton), new PropertyMetadata("2"));

        public string ControlMousePressedMargin
        {
            get { return (string)GetValue(MousePressedMarginProperty); }
            set { SetValue(MousePressedMarginProperty, value); }
        }

        public event EventHandler Click;

        public CustomButton()
        {
            InitializeComponent();
            DataContext = this;
            CButton.Click += CustomButton_Click;
        }

        private void CustomButton_Click(object sender, RoutedEventArgs e)
        {
            Click?.Invoke(this, EventArgs.Empty);
        }
    }
}