using Interface.Items;
using Logic;
using Logic.Model;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;

namespace Interface.Windows
{
    /// <summary>
    /// Логика взаимодействия для ModelList.xaml
    /// </summary>
    public partial class ModelList : Window
    {
        #region Fields
        public event EventHandler ModelSelected;
        public event EventHandler ModelUnselected;
        public event EventHandler ModelsFolderPathChanged;
        #endregion

        #region Constuctors
        public ModelList()
        {
            InitializeComponent();
            Loaded += (_, __) => SetupEventHandlers();
            LoadModels();
            LoadModelsPath();
        }
        #endregion

        #region Methods
        private void SetupEventHandlers()
        {
            AppWindows.modelSettings.ModelSettingsUpdated += (_, __) => LoadModels();
            AppWindows.modelDictionaries.ModelDictionaryUpdated += (_, __) => LoadModels();
            AppWindows.modelTrain.ModelTrainingEnd += (_, __) => LoadModels();

            Closing += (sender, e) => e.Cancel = true;
            Closing += (_, __) => Hide();

            ButtonUpdateList.Click += (_, __) => LoadModels();
            ButtonCreateModel.Click += (_, __) =>
            {
                if (AppSettings.ModelCurrent == null)
                {
                    CreateModel();
                    ModelFileSystem.SaveModel();
                    LoadModels();
                }
                else if (!AppSettings.ModelCurrent.IsBusy)
                {
                    CreateModel();
                    ModelFileSystem.SaveModel();
                    LoadModels();
                }
            };
            ButtonModelsFolderPath.Click += (_, __) => SelectModelsFolderPath();
            ButtonSaveModelsFolderPath.Click += (_, __) => SaveModelsPath();
            ButtonSaveModelsFolderPath.Click += (_, __) => OnModelsFolderPathChanged(this, EventArgs.Empty);
            ButtonSaveModelsFolderPath.Click += (_, __) => ShowStatusSaved();
            TextBoxModelsFolderPath.TextChanged += (_, __) => CheckChanges();
        }

        private void LoadModels()
        {
            StackPanelModels.Children.Clear();

            var models = ModelFileSystem.GetModelList();

            foreach (SRModel model in models)
            {
                if (model.CreateDate == "")
                    continue;

                ItemModel itemModel = new(model);
                itemModel.ModelSelected += (_, __) =>
                {
                    if (AppSettings.ModelCurrent == null)
                    {
                        model.IsBusy = false;
                        AppSettings.ModelCurrent = model;
                        ModelControlSystem.Model = model;
                        OnSelectedModel(_, EventArgs.Empty);
                    }
                    else if (!AppSettings.ModelCurrent.IsBusy)
                    {
                        model.IsBusy = false;
                        AppSettings.ModelCurrent = model;
                        ModelControlSystem.Model = model;
                        OnSelectedModel(_, EventArgs.Empty);
                    }
                };

                itemModel.ModelDeleted += (_, __) =>
                {
                    Message message = new($"Are you sure you want to delete the \"{model.Name}\" model?", Message.WindowType.YesNo);
                    message.ShowDialog();
                    if (message.DialogResult == true)
                    {
                        if (model == AppSettings.ModelCurrent && model.IsBusy)
                        {

                        }
                        else if (AppSettings.ModelCurrent == AppSettings.ModelCurrent)
                        {
                            AppSettings.ModelCurrent = null;
                            ModelControlSystem.Model = null;
                            OnUnselectedModel(_, EventArgs.Empty);
                        }
                        StackPanelModels.Children.Remove(itemModel);
                        ModelFileSystem.DeleteModel(model.Name);
                    }
                };

                StackPanelModels.Children.Add(itemModel);
            }
        }

        private static void CreateModel()
        {
            MelFrequencyCepstralCoefficients MFCC = new(AppSettings.DefaultFrameDurationMs,
                AppSettings.DefaultFrameOverlapDurationMs,
                AppSettings.DefaultNumFilters,
                AppSettings.DefaultNumCepstralCoefficients);

            DeepNeuralNetworks DNN = new(0, AppSettings.DefaultNumCepstralCoefficients);

            SRModel model = new(MFCC, DNN)
            {
                Name = Guid.NewGuid().ToString(),
                CreateDate = DateTime.Today.ToShortDateString(),
                NumStatesTrainingEpoch = AppSettings.DefaultNumStatesTrainingEpoch,
                NumWordsTrainingEpoch = AppSettings.DefaultNumWordsTrainingEpoch,
                NumCycles = AppSettings.DefaultNumCicles,
                Statebook = [],
                Wordbook = [],
                NumStates = 0,
                NumWords = 0,
                TrainingTime = "00:00:00"
            };

            AppSettings.ModelCurrent = model;
            ModelControlSystem.Model = model;
        }

        private void SelectModelsFolderPath()
        {
            using var folderBrowserDialog = new FolderBrowserDialog();
            DialogResult result = folderBrowserDialog.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK && !string.IsNullOrWhiteSpace(folderBrowserDialog.SelectedPath))
            {
                TextBoxModelsFolderPath.Text = folderBrowserDialog.SelectedPath;
            }
        }

        private void LoadModelsPath()
        {
            TextBoxModelsFolderPath.Text = AppSettings.ModelsFolderPath;
        }

        private void SaveModelsPath()
        {
            AppSettings.ModelsFolderPath = TextBoxModelsFolderPath.Text;
        }

        private void CheckChanges()
        {
            if (TextBoxModelsFolderPath.Text != AppSettings.ModelsFolderPath)
                ShowStatusUnSaved();
            else
                HideStatus();
        }

        private void ShowStatusSaved()
        {
            GridStatus.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#3EC965"));
        }

        private void ShowStatusUnSaved()
        {
            GridStatus.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2497f3"));
        }

        private void HideStatus()
        {
            GridStatus.Background = Brushes.Transparent;
        }

        private void OnSelectedModel(object sender, EventArgs e)
        {
            ModelSelected?.Invoke(sender, e);
        }

        private void OnUnselectedModel(object sender, EventArgs e)
        {
            ModelUnselected?.Invoke(sender, e);
        }

        private void OnModelsFolderPathChanged(object sender, EventArgs e)
        {
            ModelsFolderPathChanged?.Invoke(sender, e);
        }
        #endregion
    }
}