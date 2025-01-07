using Logic;
using Logic.Model;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Media;

namespace Interface.Windows
{
    /// <summary>
    /// Логика взаимодействия для ModelSettings.xaml
    /// </summary>
    public partial class ModelSettings : Window
    {
        #region Fields
        public event EventHandler ModelNameChanged;
        public event EventHandler ModelSettingsUpdated;
        private string _oldModelName;
        #endregion

        #region Constructors
        public ModelSettings()
        {
            InitializeComponent();
            SetupEventHandlers();
        }
        #endregion

        #region Methods
        private void SetupEventHandlers()
        {
            AppWindows.modelList.ModelSelected += (_, __) => LoadSettings();

            Closing += (sender, e) =>
            {
                e.Cancel = true;
                HideStatus();
                Hide();
            };

            ButtonApply.Click += (_, __) =>
            {
                if (!AppSettings.ModelCurrent.IsBusy)
                {
                    if (ValidateSettings())
                    {
                        SaveSettings();
                        ModelFileSystem.SaveModel(_oldModelName, AppSettings.ModelCurrent.Name);
                        ShowStatusSaved();
                    }
                }
            };

            TextBoxModelName.TextChanged += (_, __) => ShowStatusUnSaved();
            TextBoxNumFilters.TextChanged += (_, __) => ShowStatusUnSaved();
            TextBoxNumCepstralCoefficients.TextChanged += (_, __) => ShowStatusUnSaved();
            TextBoxFrameDurationMs.TextChanged += (_, __) => ShowStatusUnSaved();
            TextBoxFrameOverlapDurationMs.TextChanged += (_, __) => ShowStatusUnSaved();
            TextBoxNumStatesTrainingEpoch.TextChanged += (_, __) => ShowStatusUnSaved();
            TextBoxNumWordsTrainingEpoch.TextChanged += (_, __) => ShowStatusUnSaved();
            TextBoxNumCicles.TextChanged += (_, __) => ShowStatusUnSaved();
        }

        private void LoadSettings()
        {
            SRModel model = AppSettings.ModelCurrent;

            if (model == null)
                return;

            MelFrequencyCepstralCoefficients MFCC = model.SoundsCharacteristics;

            TextBoxModelName.Text = model.Name;

            TextBoxNumFilters.Text = MFCC.NumFilters.ToString();
            TextBoxNumCepstralCoefficients.Text = MFCC.NumCepstralCoefficients.ToString();
            TextBoxFrameDurationMs.Text = MFCC.FrameDurationMs.ToString();
            TextBoxFrameOverlapDurationMs.Text = MFCC.FrameOverlapDurationMs.ToString();

            TextBoxNumStatesTrainingEpoch.Text = model.NumStatesTrainingEpoch.ToString();
            TextBoxNumWordsTrainingEpoch.Text = model.NumWordsTrainingEpoch.ToString();
            TextBoxNumCicles.Text = model.NumCycles.ToString();

            HideStatus();
        }

        private void SaveSettings()
        {
            SRModel model = AppSettings.ModelCurrent;

            MelFrequencyCepstralCoefficients MFCC = model.SoundsCharacteristics;

            bool nameIsChanged = TextBoxModelName.Text != model.Name;
            _oldModelName = model.Name;

            model.Name = TextBoxModelName.Text;

            MFCC.NumFilters = int.Parse(TextBoxNumFilters.Text.Trim());
            MFCC.NumCepstralCoefficients = int.Parse(TextBoxNumCepstralCoefficients.Text.Trim());
            MFCC.FrameDurationMs = int.Parse(TextBoxFrameDurationMs.Text.Trim());
            MFCC.FrameOverlapDurationMs = int.Parse(TextBoxFrameOverlapDurationMs.Text.Trim());

            model.NumStatesTrainingEpoch = int.Parse(TextBoxNumStatesTrainingEpoch.Text.Trim());
            model.NumWordsTrainingEpoch = int.Parse(TextBoxNumWordsTrainingEpoch.Text.Trim());
            model.NumCycles = int.Parse(TextBoxNumCicles.Text.Trim());

            model.SoundsCharacteristics = MFCC;

            AppSettings.ModelCurrent = model;

            if (nameIsChanged)
            {
                OnModelNameChanged(this, EventArgs.Empty);
            }

            OnModelSettingsUpdated(this, EventArgs.Empty);
        }

        private bool ValidateSettings()
        {
            bool valid = true;
            string errorMessage = "";

            string digitPattern = @"^\d+$";

            if (TextBoxNumFilters.Text == "" ||
                !Regex.IsMatch(TextBoxNumFilters.Text.Trim(), digitPattern) ||
                !int.TryParse(TextBoxNumFilters.Text.Trim(), out int numFilters) ||
                numFilters < 1)
            {
                valid = false;
                errorMessage += "Invalid value for NumFiltets. It must be a positive integer.\n";
            }

            if (TextBoxNumCepstralCoefficients.Text == "" ||
                !Regex.IsMatch(TextBoxNumCepstralCoefficients.Text.Trim(), digitPattern) ||
                !int.TryParse(TextBoxNumCepstralCoefficients.Text.Trim(), out int numCepstralCoefficients) ||
                numCepstralCoefficients < 1)
            {
                valid = false;
                errorMessage += "Invalid value for NumCepstralCoefficients. It must be a positive integer.\n";
            }

            if (TextBoxFrameDurationMs.Text == "" ||
                !Regex.IsMatch(TextBoxFrameDurationMs.Text.Trim(), digitPattern) ||
                !int.TryParse(TextBoxFrameDurationMs.Text.Trim(), out int frameDurationMs) ||
                frameDurationMs < 1)
            {
                valid = false;
                errorMessage += "Invalid value for FrameDurationMs. It must be a positive integer.\n";
            }

            if (TextBoxFrameOverlapDurationMs.Text == "" ||
                !Regex.IsMatch(TextBoxFrameOverlapDurationMs.Text.Trim(), digitPattern) ||
                !int.TryParse(TextBoxFrameOverlapDurationMs.Text.Trim(), out int frameOverlapDurationMs) ||
                frameOverlapDurationMs < 1)
            {
                valid = false;
                errorMessage += "Invalid value for FrameOverlapDurationMs. It must be a positive integer.\n";
            }

            if (TextBoxNumStatesTrainingEpoch.Text == "" ||
                !Regex.IsMatch(TextBoxNumStatesTrainingEpoch.Text.Trim(), digitPattern) ||
                !int.TryParse(TextBoxNumStatesTrainingEpoch.Text.Trim(), out int numStatesTrainingEpoch) ||
                numStatesTrainingEpoch < 1)
            {
                valid = false;
                errorMessage += "Invalid value for NumStatesTrainingEpoch. It must be a positive integer.\n";
            }

            if (TextBoxNumWordsTrainingEpoch.Text == "" ||
                !Regex.IsMatch(TextBoxNumWordsTrainingEpoch.Text.Trim(), digitPattern) ||
                !int.TryParse(TextBoxNumWordsTrainingEpoch.Text.Trim(), out int numWordsTrainingEpoch) ||
                numWordsTrainingEpoch < 1)
            {
                valid = false;
                errorMessage += "Invalid value for NumWordsTrainingEpoch. It must be a positive integer.\n";
            }

            if (TextBoxNumCicles.Text == "" ||
                !Regex.IsMatch(TextBoxNumCicles.Text.Trim(), digitPattern) ||
                !int.TryParse(TextBoxNumCicles.Text.Trim(), out int numCicles) ||
                numCicles < 1)
            {
                valid = false;
                errorMessage += "Invalid value for NumCicles. It must be a positive integer.\n";
            }

            if (!valid)
            {
                Message message = new(errorMessage, Message.WindowType.Ok);
                message.ShowDialog();
            }

            return valid;
        }

        private void ShowStatusSaved()
        {
            GridStatus.Background = (SolidColorBrush)Application.Current.Resources["Green"];
        }

        private void ShowStatusUnSaved()
        {
            GridStatus.Background = (SolidColorBrush)Application.Current.Resources["Blue"];
        }

        private void HideStatus()
        {
            GridStatus.Background = Brushes.Transparent;
        }

        private void OnModelNameChanged(object sender, EventArgs e)
        {
            ModelNameChanged?.Invoke(sender, e);
        }

        private void OnModelSettingsUpdated(object sender, EventArgs e)
        {
            ModelSettingsUpdated?.Invoke(sender, e);
        }
        #endregion
    }
}
