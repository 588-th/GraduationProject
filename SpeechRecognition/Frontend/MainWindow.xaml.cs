using Interface.Pages;
using Logic;
using Newtonsoft.Json;
using System.IO;
using System.Windows;

namespace Interface
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool repositoriesInited;

        #region Constructors
        public MainWindow()
        {
            LoadAppSettings();
            InitializeComponent();
            SetupEventHandlers();
            FrameContainer.Navigate(AppWindows.guide);
        }
        #endregion

        #region Methods
        private void SetupEventHandlers()
        {
            Closing += (_, __) =>
            {
                Closing += (_, __) => Application.Current.Shutdown();
            };

            AppWindows.modelList.ModelSelected += (_, __) => SelectedModel();
            AppWindows.modelList.ModelUnselected += (_, __) => UnselectedModel();
            AppWindows.modelList.ModelsFolderPathChanged += (_, __) => SaveAppSettings();
            AppWindows.modelSettings.ModelNameChanged += (_, __) => ModelNameChanged();
            AppWindows.audioRepositories.AudioFolderPathChanged += (_, __) => SaveAppSettings();
            AppWindows.audioRepositories.RepositoryInited += (_, __) => RepositoryInited();
            AppWindows.audioProcessing.TrimSilentsThresholdChanged += (_, __) => SaveAppSettings();
            AppWindows.modelTrain.ModelTrainingStart += (_, __) => ModelTraining();
            AppWindows.modelTrain.ModelTrainingEnd += (_, __) => ModelTrained();


            AppWindows.modelTrain.ModelTrainingStart += (_, __) => ShowProgressBar();
            AppWindows.modelTrain.ModelTrainingStart += (_, __) => UpdateProgressBarText($"Word training cicles: {AppWindows.modelTrain.NumFinishedCycles} / {AppWindows.modelTrain.NumCycles}");
            AppWindows.modelTrain.ModelTrainingStart += (_, __) => UpdateProgressBar(AppWindows.modelTrain.NumFinishedCycles, AppWindows.modelTrain.NumCycles);
            AppWindows.modelTrain.ModelTrainingStep += (_, __) => UpdateProgressBarText($"Word training cicles: {AppWindows.modelTrain.NumFinishedCycles} / {AppWindows.modelTrain.NumCycles}");
            AppWindows.modelTrain.ModelTrainingStep += (_, __) => UpdateProgressBar(AppWindows.modelTrain.NumFinishedCycles, AppWindows.modelTrain.NumCycles);
            AppWindows.modelTrain.ModelTrainingEnd += (_, __) => UpdateProgressBarText($"Word training cicles: {AppWindows.modelTrain.NumFinishedCycles} / {AppWindows.modelTrain.NumCycles}");
            AppWindows.modelTrain.ModelTrainingEnd += (_, __) => UpdateProgressBar(AppWindows.modelTrain.NumFinishedCycles, AppWindows.modelTrain.NumCycles);
            AppWindows.modelTrain.ModelTrainingEnd += (_, __) => HideProgressBar();

            AppWindows.modelTrain.CalculateAccuracyStart += (_, __) => ShowProgressBar();
            AppWindows.modelTrain.CalculateAccuracyStart += (_, __) => UpdateProgressBarText($"Calculate accuracity words: {AppWindows.modelTrain.NumFinishedCalculatedWords} / {AppWindows.modelTrain.NumCalculatedWords}");
            AppWindows.modelTrain.CalculateAccuracyStart += (_, __) => UpdateProgressBar(AppWindows.modelTrain.NumFinishedCalculatedWords, AppWindows.modelTrain.NumCalculatedWords);
            AppWindows.modelTrain.CalculateAccuracyStep += (_, __) => UpdateProgressBarText($"Calculate accuracity words: {AppWindows.modelTrain.NumFinishedCalculatedWords} / {AppWindows.modelTrain.NumCalculatedWords}");
            AppWindows.modelTrain.CalculateAccuracyStep += (_, __) => UpdateProgressBar(AppWindows.modelTrain.NumFinishedCalculatedWords, AppWindows.modelTrain.NumCalculatedWords);
            AppWindows.modelTrain.CalculateAccuracyEnd += (_, __) => UpdateProgressBarText($"Calculate accuracity words: {AppWindows.modelTrain.NumFinishedCalculatedWords} / {AppWindows.modelTrain.NumCalculatedWords}");
            AppWindows.modelTrain.CalculateAccuracyEnd += (_, __) => UpdateProgressBar(AppWindows.modelTrain.NumFinishedCalculatedWords, AppWindows.modelTrain.NumCalculatedWords);
            AppWindows.modelTrain.CalculateAccuracyEnd += (_, __) => HideProgressBar();

            ButtonSwitchTheme.Click += (_, __) => AppTheme.SwitchTheme();

            ButtonLogo.MouseUp += Navigate;
            ButtonModelList.Click += Navigate;
            ButtonModelSettings.Click += Navigate;
            ButtonModelAudioRepositories.Click += Navigate;
            ButtonModelDictionaries.Click += Navigate;
            ButtonTrainModel.Click += Navigate;
            ButtonAudioProcessing.Click += Navigate;
            ButtonTranscription.Click += Navigate;
            ButtonRealtime.Click += Navigate;
        }

        private void LoadAppSettings()
        {
            string appSettingsPath = Path.Combine(AppSettings.AppSettingsFloderPath, "AppSettings.json");

            if (File.Exists(appSettingsPath))
            {
                string jsonSettings = File.ReadAllText(appSettingsPath);

                var appSettingsObject = JsonConvert.DeserializeAnonymousType(jsonSettings, new
                {
                    ModelsFolderPath = "",
                    AudioTrainingStatesFolderPath = "",
                    AudioTrainingWordsFolderPath = "",
                    AudioRecognizeFolderPath = "",
                    TrimSilentsThreshold = "",
                });

                if (appSettingsObject == null)
                {
                    SaveAppSettings();

                    appSettingsObject = JsonConvert.DeserializeAnonymousType(jsonSettings, new
                    {
                        ModelsFolderPath = "",
                        AudioTrainingStatesFolderPath = "",
                        AudioTrainingWordsFolderPath = "",
                        AudioRecognizeFolderPath = "",
                        TrimSilentsThreshold = "",
                    });
                }

                AppSettings.ModelsFolderPath = appSettingsObject.ModelsFolderPath;
                AppSettings.AudioTrainingStatesFolderPath = appSettingsObject.AudioTrainingStatesFolderPath;
                AppSettings.AudioTrainingWordsFolderPath = appSettingsObject.AudioTrainingWordsFolderPath;
                AppSettings.AudioRecognizeFolderPath = appSettingsObject.AudioRecognizeFolderPath;
                AppSettings.TrimSilentsThreshold = int.Parse(appSettingsObject.TrimSilentsThreshold);
            }
            else
            {
                SaveAppSettings();
            }
        }

        private void SaveAppSettings()
        {
            string appSettingsPath = Path.Combine(AppSettings.AppSettingsFloderPath, "AppSettings.json");

            using StreamWriter streamWriter = new(appSettingsPath);
            using JsonWriter jw = new JsonTextWriter(streamWriter);
            JsonSerializer serializer = new();

            var appSettingsObject = new
            {
                AppSettings.ModelsFolderPath,
                AppSettings.AudioTrainingStatesFolderPath,
                AppSettings.AudioTrainingWordsFolderPath,
                AppSettings.AudioRecognizeFolderPath,
                AppSettings.TrimSilentsThreshold
            };

            serializer.Serialize(jw, appSettingsObject);
        }

        private void ModelNameChanged()
        {
            TextBlockModel.Text = AppSettings.ModelCurrent.Name;
        }

        private void SelectedModel()
        {
            if (AppSettings.ModelCurrent == null)
            {
                UnselectedModel();
                return;
            }

            TextBlockModel.Text = AppSettings.ModelCurrent.Name.ToString();
            TextBlockTrained.Text = AppSettings.ModelCurrent.IsTrained ? "Done" : "No";
            ButtonModelSettings.IsEnabled = true;
            ButtonModelAudioRepositories.IsEnabled = true;
            ButtonModelDictionaries.IsEnabled = true;

            if (AppSettings.ModelCurrent.IsTrained)
            {
                ButtonRealtime.IsEnabled = true;
                ButtonTranscription.IsEnabled = true;
            }

            if (repositoriesInited)
            {
                ButtonTrainModel.IsEnabled = true;
            }

            Realtime.UpdateAxis();
        }

        private void UnselectedModel()
        {
            TextBlockModel.Text = "None";
            TextBlockTrained.Text = "No";
            ButtonModelSettings.IsEnabled = false;
            ButtonModelAudioRepositories.IsEnabled = false;
            ButtonModelDictionaries.IsEnabled = false;
            ButtonTrainModel.IsEnabled = false;
            ButtonAudioProcessing.IsEnabled = false;
            ButtonRealtime.IsEnabled = false;
        }

        private void ModelTraining()
        {
            TextBlockTrained.Text = "In Progress";
        }

        private void ModelTrained()
        {
            TextBlockTrained.Text = "Done";

            ButtonRealtime.IsEnabled = true;
            ButtonTranscription.IsEnabled = true;
        }

        private void RepositoryInited()
        {
            if (AppSettings.ModelCurrent != null)
            {
                ButtonTrainModel.IsEnabled = true;
            }
            repositoriesInited = true;
            ButtonAudioProcessing.IsEnabled = true;
        }

        private void UpdateProgressBarText(string text)
        {
            Dispatcher.Invoke(() =>
            {
                TextBlockProgressBar.Text = text;
            });
        }

        private void UpdateProgressBar(int currentPoint, int amount)
        {
            Dispatcher.Invoke(() =>
            {
                if (amount == 0)
                {
                    ProgressBar.Value = 0;
                    return;
                }

                double progressPercentage = (double)currentPoint / amount * 100;

                ProgressBar.Value = progressPercentage;
            });
        }

        private void HideProgressBar()
        {
            ProgressBar.Visibility = Visibility.Collapsed;
            TextBlockProgressBar.Visibility = Visibility.Collapsed;
        }

        private void ShowProgressBar()
        {
            ProgressBar.Visibility = Visibility.Visible;
            TextBlockProgressBar.Visibility = Visibility.Visible;
        }

        private void Navigate(object sender, EventArgs e)
        {
            if (sender is Controls.CustomButton button)
            {
                string pageName = button.Tag.ToString();
                switch (pageName)
                {
                    case "Guide":
                        FrameContainer.Navigate(AppWindows.guide);
                        break;
                    case "ModelList":
                        AppWindows.modelList.Show();
                        AppWindows.modelList.Activate();
                        break;
                    case "ModelSettings":
                        AppWindows.modelSettings.Show();
                        AppWindows.modelSettings.Activate();
                        break;
                    case "ModelDictionaries":
                        AppWindows.modelDictionaries.Show();
                        AppWindows.modelDictionaries.Activate();
                        break;
                    case "AudioRepositories":
                        FrameContainer.Navigate(AppWindows.audioRepositories);
                        break;
                    case "ModelTrain":
                        FrameContainer.Navigate(AppWindows.modelTrain);
                        break;
                    case "AudioProcessing":
                        FrameContainer.Navigate(AppWindows.audioProcessing);
                        break;
                    case "Realtime":
                        FrameContainer.Navigate(AppWindows.realtime);
                        break;
                    case "Transcription":
                        AppWindows.transcription.Show();
                        AppWindows.transcription.Activate();
                        break;
                }
            }
        }
        #endregion
    }
}
