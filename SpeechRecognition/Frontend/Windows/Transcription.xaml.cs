using Logic;
using Logic.Algorithms;
using Logic.Audio;
using Logic.Model;
using Microsoft.Win32;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace Interface.Windows
{
    /// <summary>
    /// Логика взаимодействия для Transcription.xaml
    /// </summary>
    public partial class Transcription : Window
    {
        private static string _fileName;

        public Transcription()
        {
            InitializeComponent();
            SetupEventHandlers();
        }

        private void SetupEventHandlers()
        {
            Closing += (sender, e) => e.Cancel = true;
            Closing += (_, __) => HideStatus();
            Closing += (_, __) => Hide();

            ButtonTranscribe.Click += (_, __) => Transcribe();

            ButtonSave.Click += (_, __) => SaveTranscription();
            ButtonSave.Click += (_, __) => ShowStatusSaved();
        }

        private void Transcribe()
        {
            string path = string.Empty;

            var openFileDialog = new OpenFileDialog
            {
                Filter = "Audio files (*.wav)|*.wav",
                Title = "Выберите аудиофайл WAV"
            };

            bool? result = openFileDialog.ShowDialog();

            if (result == true)
            {
                path = openFileDialog.FileName;
            }

            if (!File.Exists(path))
                return;

            AudioBuilder builder = new();
            builder.ReadAudio(path);
            AudioInformation audioInformation = builder.GetAudioInformation();

            _fileName = audioInformation.Name;

            List<double[]> soundCharactVectors = ModelControlSystem.Model.SoundsCharacteristics.Extract(audioInformation);
            double[][] probabilityMatrix = ModelControlSystem.Model.AcousticModel.Predict(soundCharactVectors);
            List<List<int>> recognizedStates = StateProcessing.GetSentenceStates(probabilityMatrix);
            string word;
            StringBuilder bld = new();
            foreach (var item in recognizedStates)
            {
                word = WordbookMatcher.ConvertingStatesToLetters(ModelControlSystem.Model.Statebook, item);
                bld.Append(WordbookMatcher.FindMostSimilarWord(ModelControlSystem.Model.Wordbook, word));
                bld.Append(" ");
            }
            string recognizedText = bld.ToString();
            var trimRecognixeText = recognizedText.Trim(' ');
            TextBoxTranscription.Text = trimRecognixeText;
        }

        private void SaveTranscription()
        {
            var saveFileDialog = new SaveFileDialog
            {
                Filter = "Text files (*.txt)|*.txt",
                Title = "Выберите директорию и укажите название текстового файла",
                FileName = $"{_fileName.Split('.')[0]}Transcription.txt"
            };

            bool? result = saveFileDialog.ShowDialog();

            if (result == true)
            {
                string filePath = saveFileDialog.FileName;
                string textToSave = TextBoxTranscription.Text;

                if (!string.IsNullOrEmpty(filePath))
                {
                    File.WriteAllText(filePath, textToSave);
                }
            }
        }

        private void ShowStatusSaved()
        {
            GridStatus.Background = (SolidColorBrush)System.Windows.Application.Current.Resources["Green"];
        }

        private void HideStatus()
        {
            GridStatus.Background = Brushes.Transparent;
        }
    }
}
