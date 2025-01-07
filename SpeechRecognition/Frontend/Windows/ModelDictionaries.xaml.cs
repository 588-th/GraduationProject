using Logic;
using Logic.Model;
using System.IO;
using System.Windows;
using System.Windows.Media;
using Tensorflow.Keras.Engine;

namespace Interface.Windows
{
    /// <summary>
    /// Логика взаимодействия для ModelDictionaries.xaml
    /// </summary>
    public partial class ModelDictionaries : Window
    {
        #region Fields
        public event EventHandler ModelDictionaryUpdated;
        #endregion

        #region Constructors
        public ModelDictionaries()
        {
            InitializeComponent();
            SetupEventHandlers();
        }
        #endregion

        #region Methods
        private void SetupEventHandlers()
        {
            AppWindows.modelList.ModelSelected += (_, __) => LoadDictionaries();

            Closing += (sender, e) => e.Cancel = true;
            Closing += (_, __) => HideStatus();
            Closing += (_, __) => Hide();

            ButtonApply.Click += (_, __) =>
            {
                if (!AppSettings.ModelCurrent.IsBusy)
                {
                    if (ValidDictionaries())
                    {
                        SaveDictionaries();
                        ModelFileSystem.SaveModel();
                        ShowStatusSaved();
                    }
                }
            };

            ButtonImportStates.Click += (_, __) => ImportStatesDictionary();
            ButtonImportStates.Click += (_, __) => ShowStatusUnSaved();

            ButtonImportWords.Click += (_, __) => ImportWordsDictionary();
            ButtonImportWords.Click += (_, __) => ShowStatusUnSaved();

            ButtonExportStates.Click += (_, __) => ExportStatesDictionary();
            ButtonExportWords.Click += (_, __) => ExportWordsDictionary();

            ButtonSortStates.Click += (_, __) => SortStatesDictionary();
            ButtonSortWords.Click += (_, __) => SortWordsDictionary();
        }

        private bool ValidDictionaries()
        {
            bool valid = true;

            string wr = TextBoxWordDictionary.Text.Replace("\r", "");
            string[] words = wr.Split('\n');

            string st = TextBoxStateDictionary.Text.Replace("\r", "");
            string[] states = st.Split('\n');

            if (words.Length == 0)
                valid = false;

            if (states.Length <=1)
                valid = false;

            if (valid == false)
            {
                Message message = new($"Dictionaries cannot be empty", Message.WindowType.Ok);
                message.ShowDialog();
            }

            return valid;
        }

        private void LoadDictionaries()
        {
            LoadStateDictionary();
            LoadWordDictionary();
        }

        private void SaveDictionaries()
        {
            SaveWordDictionary();
            SaveStateDictionary();
            OnModelDictionaryUpdated(this, EventArgs.Empty);
        }

        private void LoadStateDictionary()
        {
            SRModel model = AppSettings.ModelCurrent;

            if (model == null)
                return;

            TextBoxStateDictionary.Text = "";

            for (int i = 1; i < model.Statebook.Count; i++)
            {
                TextBoxStateDictionary.Text += $"{model.Statebook[i]}\n";
            }
        }

        private void LoadWordDictionary()
        {
            SRModel model = AppSettings.ModelCurrent;

            if (model == null)
                return;

            TextBoxWordDictionary.Text = "";

            foreach (string word in model.Wordbook)
            {
                TextBoxWordDictionary.Text += $"{word}\n";
            }
        }

        private void ImportStatesDictionary()
        {
            System.Windows.Forms.OpenFileDialog openFileDialog = new()
            {
                Filter = "Текстовые файлы (*.txt)|*.txt|Все файлы (*.*)|*.*"
            };

            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string filePath = openFileDialog.FileName;

                string[] lines = File.ReadAllLines(filePath);

                TextBoxStateDictionary.Clear();

                foreach (string line in lines)
                {
                    TextBoxStateDictionary.Text += line + Environment.NewLine;
                }
            }
        }

        private void ImportWordsDictionary()
        {
            System.Windows.Forms.OpenFileDialog openFileDialog = new()
            {
                Filter = "Текстовые файлы (*.txt)|*.txt|Все файлы (*.*)|*.*"
            };

            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string filePath = openFileDialog.FileName;

                string[] lines = File.ReadAllLines(filePath);

                TextBoxWordDictionary.Clear();

                foreach (string line in lines)
                {
                    TextBoxWordDictionary.Text += line + Environment.NewLine;
                }
            }
        }

        private void ExportStatesDictionary()
        {
            ExportDictionary(TextBoxStateDictionary);
        }

        private void ExportWordsDictionary()
        {
            ExportDictionary(TextBoxWordDictionary);
        }

        private void SortStatesDictionary()
        {
            SortDictionary(TextBoxStateDictionary);
        }

        private void SortWordsDictionary()
        {
            SortDictionary(TextBoxWordDictionary);
        }

        private void ExportDictionary(System.Windows.Controls.TextBox textBox)
        {
            Microsoft.Win32.SaveFileDialog saveFileDialog = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "Text file (*.txt)|*.txt",
                Title = "Save Dictionary",
                FileName = $"{AppSettings.ModelCurrent.Name}Dictionary.txt"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                File.WriteAllText(saveFileDialog.FileName, textBox.Text);
            }
        }

        private void SortDictionary(System.Windows.Controls.TextBox textBox)
        {
            List<string> lines = textBox.Text.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None).ToList();
            lines.Sort(StringComparer.CurrentCulture);
            lines.Remove("");
            textBox.Text = string.Join(Environment.NewLine, lines);
        }

        private void SaveStateDictionary()
        {
            SRModel model = AppSettings.ModelCurrent;

            model.Statebook.Clear();

            string st = TextBoxStateDictionary.Text.Replace("\r", "");
            string[] states = st.Split('\n');

            model.Statebook.Add(0, "-");
            for (int i = 1; i < states.Length + 1; i++)
            {
                if (states[i - 1] == "")
                    continue;

                model.Statebook.Add(i, states[i - 1]);
            }

            model.NumStates = model.Statebook.Count;
            model.AcousticModel.NumStates = model.Statebook.Count;

            AppSettings.ModelCurrent = model;
        }

        private void SaveWordDictionary()
        {
            SRModel model = AppSettings.ModelCurrent;

            model.Wordbook.Clear();

            string wr = TextBoxWordDictionary.Text.Replace("\r", "");
            string[] words = wr.Split('\n');

            foreach (string word in words)
            {
                if (word == "")
                    continue;

                model.Wordbook.Add(word);
            }

            model.NumWords = model.Wordbook.Count;

            AppSettings.ModelCurrent = model;
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

        private void OnModelDictionaryUpdated(object sender, EventArgs e)
        {
            ModelDictionaryUpdated?.Invoke(sender, e);
        }
        #endregion
    }
}
