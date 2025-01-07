using Interface.Windows;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using Logic;
using Logic.Audio;
using Logic.Model;
using NAudio.Wave;
using SkiaSharp;
using System.Drawing;
using System.Timers;
using System.Windows.Controls;
using Tensorflow.Keras.Engine;

namespace Interface.Pages
{
    public partial class ModelTrain : Page
    {
        #region Fields
        public event EventHandler? ModelTrainingStart;
        public event EventHandler? ModelTrainingStep;
        public event EventHandler? ModelTrainingEnd;

        public event EventHandler? CalculateAccuracyStart;
        public event EventHandler? CalculateAccuracyStep;
        public event EventHandler? CalculateAccuracyEnd;

        public SolidColorPaint LegendTextPaint { get; set; } = new SolidColorPaint { Color = new SKColor(255, 255, 255) };
        public static Axis[] YAxes { get; set; } = { new Axis { } };

        private Thread _modelThread;
        private readonly AutoResetEvent _trainingEvent = new(false);
        private readonly AutoResetEvent _recognitionEvent = new(false);
        private readonly AutoResetEvent _retrainEvent = new(false);
        private readonly AutoResetEvent _calculateAccuracyEvent = new(false);

        private static readonly System.Timers.Timer _timer = new(1000);
        private static TimeSpan _currentTime;

        public int NumFinishedCycles { get; private set; }
        public int NumCycles { get; private set; }

        public int NumFinishedCalculatedWords { get; private set; }
        public int NumCalculatedWords { get; private set; }

        private int _minWeight;

        private WaveOutEvent? _waveOut;
        #endregion

        #region Constructor
        public ModelTrain()
        {
            InitializeComponent();
            SetupEventHandlers();
            StartModelThread();
            DataContext = this;
        }
        #endregion

        #region Methods

        private void SetupEventHandlers()
        {
            AppWindows.modelList.ModelSelected += (_, __) => CheckModelTraining();
            AppWindows.modelSettings.ModelSettingsUpdated += (_, __) => CheckModelTraining();
            ModelControlSystem.CycleFinished += (_, __) => UpdateIteration();
            ModelControlSystem.CalculateAccuracyOfWordFinished += (_, __) => UpdateCalculatedWords();

            FileExplorerRecognize.FileSelected += (_, __) => UpdateAudioFileInformation();
            FileExplorerRecognize.DoubleClick += (_, __) => _recognitionEvent.Set();
            ButtonRecognize.Click += (_, __) => _recognitionEvent.Set();
            ButtonRetrain.Click += (_, __) => _retrainEvent.Set();
            ButtonTrain.Click += (_, __) => _trainingEvent.Set();
            ButtonCalculateAccuracy.Click += (_, __) => _calculateAccuracyEvent.Set();
            ButtonClearRecognizedWords.Click += (_, __) => ClearRecognizedWords();
            ButtonClearAccuracy.Click += (_, __) => ClearAccuracyWords();
            ButtonPlayStopAudio.Click += (_, __) => PlayStopAudio();
            ButtonSaveModel.Click += (_, __) => SaveModel();
            ButtonSetMinWeight.Click += (_, __) => SetMinWeight();
            ButtonStopTrain.Click += (_, __) => StopTrain();

            _timer.Elapsed += TimerElapsed;
        }

        private void StartModelThread()
        {
            _modelThread = new Thread(ModelLoop) { IsBackground = true };
            _modelThread.Start();
        }

        private void ModelLoop()
        {
            while (true)
            {
                WaitHandle[] waitHandles = { _trainingEvent, _recognitionEvent, _retrainEvent, _calculateAccuracyEvent };
                int signaledHandle = WaitHandle.WaitAny(waitHandles);

                switch (signaledHandle)
                {
                    case 0:
                        Train();
                        break;
                    case 1:
                        Recognize();
                        break;
                    case 2:
                        Retrain();
                        break;
                    case 3:
                        OutputAccuracyWords();
                        break;
                }
            }
        }

        private void CheckModelTraining()
        {
            UpdateTimer();
            NumCycles = AppSettings.ModelCurrent.NumCycles;
            NumCalculatedWords = AppSettings.ModelCurrent.NumWords;
            TextBlockCycles.Text = $"Cycles: {AppSettings.ModelCurrent.NumFinishedCycles}";

            if (AppSettings.ModelCurrent.IsTrained)
            {
                ModelFileSystem.LoadAcousticModel();
                SetStatusModelNotBusy();
            }
            else
            {
                SetStatusModelBusy();
                ButtonTrain.IsEnabled = true;
            }
        }

        private void UpdateAudioFileInformation()
        {
            string path = FileExplorerRecognize.CurrentFile;

            var audioBuilder = new AudioBuilder();
            audioBuilder.ReadAudio(path);
            var audioInformation = audioBuilder.GetAudioInformation();

            AppSettings.AudioCurrent = audioInformation.Name;

            TextBlockAudioFileInformation.Text = $"Name: {audioInformation.Name}\nChannels: {audioInformation.Channels}\nSampleRate: {audioInformation.SampleRate}";

            ButtonRecognize.IsEnabled = true;
            ButtonPlayStopAudio.IsEnabled = true;
        }

        private void Recognize()
        {
            if (AppSettings.AudioCurrent == null || AppSettings.AudioCurrent == "")
            { return; }

            var (word, probabilityMatrix) = ModelControlSystem.Recognize();

            Dispatcher.Invoke(() =>
            {
                OutputRecognizedWord(word);
                OutputProbabilityMatrix(probabilityMatrix);
            });
        }

        private void Train()
        {
            Dispatcher.Invoke(() =>
            {
                SetStatusModelBusy();
                ModelTrainingStart?.Invoke(this, EventArgs.Empty);

                _timer.Elapsed += (_, __) => Dispatcher.Invoke(() => TextBlockTrainTime.Text = $"Train time: {_currentTime:hh\\:mm\\:ss}");
                _timer.Start();
                ButtonStopTrain.IsEnabled = true;
            });

            ModelControlSystem.Train();

            Dispatcher.Invoke(() =>
            {
                ModelTrainingEnd?.Invoke(this, EventArgs.Empty);
                SetStatusModelNotBusy();
                _timer.Stop();
            });
        }

        private void StopTrain()
        {
            ModelControlSystem.StopTrain();

            Dispatcher.Invoke(() =>
            {
                ModelTrainingEnd?.Invoke(this, EventArgs.Empty);
                SetStatusModelNotBusy();
                _timer.Stop();
                ButtonStopTrain.IsEnabled = false;
            });

        }

        private void Retrain()
        {
            ModelControlSystem.Model.NumFinishedCycles = 0;
            NumFinishedCycles = 0;
            TextBlockCycles.Text = "0";

            Dispatcher.Invoke(() =>
            {
                SetStatusModelBusy();
                ModelTrainingStart?.Invoke(this, EventArgs.Empty);

                _currentTime = TimeSpan.Zero;
                _timer.Elapsed += (_, __) => Dispatcher.Invoke(() => TextBlockTrainTime.Text = $"Train time: {_currentTime:hh\\:mm\\:ss}");
                _timer.Start();
                ButtonStopTrain.IsEnabled = true;
            });

            ModelControlSystem.Retrain();

            Dispatcher.Invoke(() =>
            {
                ModelTrainingEnd?.Invoke(this, EventArgs.Empty);
                SetStatusModelNotBusy();
                _timer.Stop();
            });
        }

        private void SetStatusModelBusy()
        {
            SetModelButtonsEnabled(false);
        }

        private void SetStatusModelNotBusy()
        {
            SetModelButtonsEnabled(true);
        }

        private void SetModelButtonsEnabled(bool isEnabled)
        {
            ButtonRecognize.IsEnabled = isEnabled;
            ButtonTrain.IsEnabled = isEnabled;
            ButtonRetrain.IsEnabled = isEnabled;
            ButtonCalculateAccuracy.IsEnabled = isEnabled;
            ButtonSaveModel.IsEnabled = isEnabled;
        }

        private void SetMinWeight() => _minWeight = int.Parse(TextBoxMinWeight.Text);

        private void UpdateTimer()
        {
            var trainingTimeParts = AppSettings.ModelCurrent.TrainingTime.Split(':');

            if (trainingTimeParts.Length == 3 &&
                int.TryParse(trainingTimeParts[0], out int hours) &&
                int.TryParse(trainingTimeParts[1], out int minutes) &&
                int.TryParse(trainingTimeParts[2], out int seconds))
            {
                _currentTime = new TimeSpan(hours, minutes, seconds);
                TextBlockTrainTime.Text = $"Train time: {_currentTime:hh\\:mm\\:ss}";
            }
        }

        private void UpdateIteration()
        {
            Dispatcher.Invoke((Delegate)(() =>
            {
                NumFinishedCycles++;
                ModelTrainingStep?.Invoke(this, EventArgs.Empty);
                TextBlockCycles.Text = $"Cycles: {NumFinishedCycles}";
            }));
        }

        private void UpdateCalculatedWords()
        {
            NumFinishedCalculatedWords++;
            NumCalculatedWords = AppSettings.ModelCurrent.NumWords;
            CalculateAccuracyStep?.Invoke(this, EventArgs.Empty);
        }

        private void OutputProbabilityMatrix(double[][] probabilityMatrix)
        {
            probabilityMatrix = FlipMatrix(probabilityMatrix);

            int frameCount = probabilityMatrix[0].Length;
            int stateCount = probabilityMatrix.Length;

            var heatMapColors = new[]
            {
                new SKColor(36, 151, 243).AsLvcColor(),
                new SKColor(233, 30, 99).AsLvcColor(),
            };

            var values = new List<WeightedPoint>();
            var states = new HashSet<int>();

            int stateNumber = 0;
            bool containsMinWeight;

            for (int state = 0; state < stateCount; state++)
            {
                containsMinWeight = false;

                for (int frame = 0; frame < frameCount; frame++)
                {
                    double probability = probabilityMatrix[state][frame];
                    int weightedValue = (int)(probability * 1000);

                    if (weightedValue >= _minWeight)
                    {
                        containsMinWeight = true;
                        states.Add(state);
                    }
                }

                if (containsMinWeight)
                {
                    for (int frame = 0; frame < frameCount; frame++)
                    {
                        double probability = probabilityMatrix[state][frame];
                        int weightedValue = (int)(probability * 1000);
                        values.Add(new WeightedPoint(frame, stateNumber, weightedValue));
                    }
                    stateNumber++;
                }
            }

            string[] letters = states.Select(state => AppSettings.ModelCurrent.Statebook[state]).ToArray();

            var heatSeries = new HeatSeries<WeightedPoint>
            {
                Name = "",
                HeatMap = heatMapColors,
                Values = values
            };

            YAxes[0].Labels = letters;
            ChartRecognizedStates.Series = new[] { heatSeries };
        }

        private void OutputRecognizedWord(string word) => ListViewRecognizedWord.Items.Add(word);

        private void OutputAccuracyWords()
        {
            NumFinishedCalculatedWords = 0;

            Dispatcher.Invoke((Delegate)(() =>
            {
                CalculateAccuracyStart?.Invoke(this, EventArgs.Empty);
                SetStatusModelBusy();
            }));

            var accuracy = ModelControlSystem.CalculateAccuracy();

            Dispatcher.Invoke(() =>
            {
                foreach (var item in accuracy)
                {
                    ListViewAccuracyWord.Items.Add(item.Key);
                    ListViewAccuracy.Items.Add(item.Value);
                }

                SetStatusModelNotBusy();
                CalculateAccuracyEnd?.Invoke(this, EventArgs.Empty);
            });
        }

        private void ClearRecognizedWords() => ListViewRecognizedWord.Items.Clear();

        private void ClearAccuracyWords()
        {
            ListViewAccuracy.Items.Clear();
            ListViewAccuracyWord.Items.Clear();
        }

        private static void SaveModel()
        {
            AppSettings.ModelCurrent.TrainingTime = $"{_currentTime:hh\\:mm\\:ss}";
            ModelFileSystem.SaveModel();
            ModelFileSystem.SaveAcousticModel();
        }

        private void PlayStopAudio()
        {
            var audioInformation = AudioRepository.GetAudioFileFromName(AppSettings.AudioCurrent, AudioRepository.RecognizingAudio);

            if (_waveOut == null)
            {
                _waveOut = new WaveOutEvent();
                _waveOut.Init(new AudioFileReader(audioInformation.Path));
                _waveOut.Play();
                ButtonPlayStopAudio.ControlContent = "Stop";

                _waveOut.PlaybackStopped += (_, __) =>
                {
                    _waveOut?.Stop();
                    _waveOut?.Dispose();
                    _waveOut = null;
                    ButtonPlayStopAudio.ControlContent = "Play";
                };
            }
            else if (_waveOut.PlaybackState == PlaybackState.Playing)
            {
                _waveOut.Stop();
            }
        }

        private static double[][] FlipMatrix(double[][] matrix)
        {
            int rows = matrix.Length;
            int cols = matrix[0].Length;

            var flippedMatrix = new double[cols][];

            for (int i = 0; i < cols; i++)
            {
                flippedMatrix[i] = new double[rows];
            }

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    flippedMatrix[j][i] = matrix[i][j];
                }
            }

            return flippedMatrix;
        }

        private static void TimerElapsed(object sender, ElapsedEventArgs e) => _currentTime = _currentTime.Add(TimeSpan.FromSeconds(1));

        #endregion
    }
}