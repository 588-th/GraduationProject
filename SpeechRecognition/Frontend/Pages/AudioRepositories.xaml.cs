using Logic;
using Logic.Audio;
using Logic.Model;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media;

namespace Interface.Pages
{
    /// <summary>
    /// Логика взаимодействия для AudioRepositories.xaml
    /// </summary>
    public partial class AudioRepositories : Page
    {
        #region Fields
        public event EventHandler? AudioFolderPathChanged;
        public event EventHandler? RepositoryInited;
        #endregion

        #region Constructor
        public AudioRepositories()
        {
            InitializeComponent();
            SetupEventHandlers();
        }
        #endregion

        #region Methods
        private void SetupEventHandlers()
        {
            Loaded += (_, __) => LoadPaths();
            Loaded += (_, __) => HideStatus();

            ButtonApply.Click += (_, __) =>
            {
                if (AppSettings.ModelCurrent == null)
                {
                    SavePaths();
                    OnAudioFolderPathChanged(this, EventArgs.Empty);
                    ShowStatusSaved();
                }
                else if (!AppSettings.ModelCurrent.IsBusy)
                {
                    SavePaths();
                    OnAudioFolderPathChanged(this, EventArgs.Empty);
                    ShowStatusSaved();
                }
            };

            ButtonInitRepositores.Click += (_, __) =>
            {
                if (AppSettings.ModelCurrent == null)
                {
                    InitRepositories();
                    OnRepositoryInited(this, EventArgs.Empty);
                }
                else if (!AppSettings.ModelCurrent.IsBusy)
                {
                    InitRepositories();
                    OnRepositoryInited(this, EventArgs.Empty);
                }
            };

            ButtonStatesFolderPath.Click += (_, __) => SelectStatesFolderPath();
            ButtonWordsFolderPath.Click += (_, __) => SelectWordsFolderPath();
            ButtonRecognizeFolderPath.Click += (_, __) => SelectRecognizeFolderPath();

            TextBoxAudioTrainingStatesFolderPath.TextChanged += (_, __) => ShowStatusUnSaved();
            TextBoxAudioTrainingWordsFolderPath.TextChanged += (_, __) => ShowStatusUnSaved();
            TextBoxAudioRecognizeFolderPath.TextChanged += (_, __) => ShowStatusUnSaved();
        }

        private static void InitRepositories()
        {
            AudioRepository.InitRepository();
        }

        private void LoadPaths()
        {
            TextBoxAudioTrainingStatesFolderPath.Text = AppSettings.AudioTrainingStatesFolderPath;
            TextBoxAudioTrainingWordsFolderPath.Text = AppSettings.AudioTrainingWordsFolderPath;
            TextBoxAudioRecognizeFolderPath.Text = AppSettings.AudioRecognizeFolderPath;
        }

        private void SavePaths()
        {
            AppSettings.AudioTrainingStatesFolderPath = TextBoxAudioTrainingStatesFolderPath.Text;
            AppSettings.AudioTrainingWordsFolderPath = TextBoxAudioTrainingWordsFolderPath.Text;
            AppSettings.AudioRecognizeFolderPath = TextBoxAudioRecognizeFolderPath.Text;
        }

        private void SelectStatesFolderPath()
        {
            using var folderBrowserDialog = new FolderBrowserDialog();
            DialogResult result = folderBrowserDialog.ShowDialog();

            if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(folderBrowserDialog.SelectedPath))
            {
                TextBoxAudioTrainingStatesFolderPath.Text = folderBrowserDialog.SelectedPath;
            }
        }

        private void SelectWordsFolderPath()
        {
            using var folderBrowserDialog = new FolderBrowserDialog();
            DialogResult result = folderBrowserDialog.ShowDialog();

            if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(folderBrowserDialog.SelectedPath))
            {
                TextBoxAudioTrainingWordsFolderPath.Text = folderBrowserDialog.SelectedPath;
            }
        }

        private void SelectRecognizeFolderPath()
        {
            using var folderBrowserDialog = new FolderBrowserDialog();
            DialogResult result = folderBrowserDialog.ShowDialog();

            if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(folderBrowserDialog.SelectedPath))
            {
                TextBoxAudioRecognizeFolderPath.Text = folderBrowserDialog.SelectedPath;
            }
        }

        private void ShowStatusSaved()
        {
            GridStatus.Background = (SolidColorBrush)System.Windows.Application.Current.Resources["Green"];
        }

        private void ShowStatusUnSaved()
        {
            GridStatus.Background = (SolidColorBrush)System.Windows.Application.Current.Resources["Blue"];
        }

        private void HideStatus()
        {
            GridStatus.Background = Brushes.Transparent;
        }

        private void OnAudioFolderPathChanged(object sender, EventArgs e)
        {
            AudioFolderPathChanged?.Invoke(sender, e);
        }

        private void OnRepositoryInited(object sender, EventArgs e)
        {
            RepositoryInited?.Invoke(sender, e);
        } 
        #endregion
    }
}
