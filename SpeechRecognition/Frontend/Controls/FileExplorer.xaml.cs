using Logic;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Interface.Controls
{
    public partial class FileExplorer : UserControl
    {
        public event EventHandler<string> DoubleClick;
        public event EventHandler<string> FileSelected;
        public string CurrentFile;

        public static readonly DependencyProperty ControlTagProperty =
    DependencyProperty.Register("ControlTagProperty", typeof(string), typeof(FileExplorer), new PropertyMetadata("Recognize"));

        public string ControlTag
        {
            get { return (string)GetValue(ControlTagProperty); }
            set { SetValue(ControlTagProperty, value); }
        }

        public static readonly DependencyProperty ControlBackgroundProperty =
            DependencyProperty.Register("ControlBackground", typeof(Brush), typeof(FileExplorer), new PropertyMetadata(null));

        public Brush ControlBackground
        {
            get { return (Brush)GetValue(ControlBackgroundProperty); }
            set { SetValue(ControlBackgroundProperty, value); }
        }

        public static readonly DependencyProperty ControlForegroundProperty =
    DependencyProperty.Register("ControlForeground", typeof(Brush), typeof(FileExplorer), new PropertyMetadata(null));

        public Brush ControlForeground
        {
            get { return (Brush)GetValue(ControlForegroundProperty); }
            set { SetValue(ControlForegroundProperty, value); }
        }

        public static readonly DependencyProperty ControlBorderThicknessProperty =
    DependencyProperty.Register("ControlBorderThickness", typeof(string), typeof(FileExplorer), new PropertyMetadata("0"));

        public string ControlBorderThickness
        {
            get { return (string)GetValue(ControlBorderThicknessProperty); }
            set { SetValue(ControlBorderThicknessProperty, value); }
        }

        public static readonly DependencyProperty ControlBorderBrushProperty =
    DependencyProperty.Register("ControlBorderBrush", typeof(Brush), typeof(FileExplorer), new PropertyMetadata(null));

        public Brush ControlBorderBrush
        {
            get { return (Brush)GetValue(ControlBorderBrushProperty); }
            set { SetValue(ControlBorderBrushProperty, value); }
        }

        public static readonly DependencyProperty CornerRadiusProperty =
    DependencyProperty.Register("ControlCornerRadius", typeof(string), typeof(FileExplorer), new PropertyMetadata("10"));

        public string ControlCornerRadius
        {
            get { return (string)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }

        RoutedEventHandler routedHandler;

        public FileExplorer()
        {
            InitializeComponent();
            DataContext = this;

            routedHandler = (_, __) => DisplayFilesAndDirectories();
            Loaded += new RoutedEventHandler(routedHandler);
            FileTreeView.MouseDoubleClick += (_, __) => FileTreeView_MouseDoubleClick();
            FileTreeView.SelectedItemChanged += (_, __) => FileTreeView_SelectedItemChanged();
        }

        private void DisplayFilesAndDirectories()
        {
            string directoryPath;

            switch (ControlTag)
            {
                case "States":
                    directoryPath = AppSettings.AudioTrainingStatesFolderPath;
                    break;
                case "Words":
                    directoryPath = AppSettings.AudioTrainingWordsFolderPath;
                    break;
                case "Recognize":
                    directoryPath = AppSettings.AudioRecognizeFolderPath;
                    break;
                default:
                    return;
            }

            try
            {
                FileTreeView.Items.Clear();
                var rootNode = new TreeViewItem { Header = new DirectoryInfo(directoryPath).Name };
                FileTreeView.Items.Add(rootNode);
                PopulateTreeView(directoryPath, rootNode);
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"Error occurred while accessing the directory: {ex.Message}");
            }

            Loaded -= new RoutedEventHandler(routedHandler);
        }

        private void PopulateTreeView(string directoryPath, TreeViewItem parentNode)
        {
            try
            {
                foreach (string subDir in Directory.GetDirectories(directoryPath))
                {
                    var subDirNode = new TreeViewItem { Header = new DirectoryInfo(subDir).Name };
                    parentNode.Items.Add(subDirNode);
                    PopulateTreeView(subDir, subDirNode);
                }

                foreach (string file in Directory.GetFiles(directoryPath, "*.wav"))
                {
                    var fileNode = new TreeViewItem { Header = new FileInfo(file).Name, Tag = file };
                    parentNode.Items.Add(fileNode);
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void FileTreeView_MouseDoubleClick()
        {
            if (FileTreeView.SelectedItem is TreeViewItem selectedItem)
            {
                var selectedItemTag = selectedItem.Tag as string;
                if (File.Exists(selectedItemTag))
                {
                    DoubleClick?.Invoke(this, selectedItemTag);
                }
            }
        }

        private void FileTreeView_SelectedItemChanged()
        {
            if (FileTreeView.SelectedItem is TreeViewItem selectedItem)
            {
                var selectedItemTag = selectedItem.Tag as string;
                if (File.Exists(selectedItemTag))
                {
                    CurrentFile = selectedItemTag;
                    FileSelected?.Invoke(this, selectedItemTag);
                }
            }
        }

        private void ShowErrorMessage(string message)
        {
            MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}