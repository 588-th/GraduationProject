using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using Logic;
using Logic.Audio;
using Logic.Model;
using NAudio.Wave;
using SkiaSharp;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace Interface.Pages
{
    public partial class Realtime
    {
        #region Fields
        public SolidColorPaint LegendTextPaint { get; set; } = new SolidColorPaint { Color = new SKColor(255, 255, 255) };
        public static Axis[] YAxes { get; set; } = [new Axis { }];

        private WaveInEvent? _waveIn;
        private WaveFileWriter? _writer;
        private readonly string _outputFile = "recordedAudio.wav";
        private readonly float _energyThreshold = 8000f;
        private int _countEmptyFrames = 0;
        private int _countVoiceFrames = 0;
        private bool _isRecording = false;
        private bool _isVoiceDetected = false;
        #endregion

        #region Constructor
        public Realtime()
        {
            InitializeComponent();
            SetupEventHandlers();
            DataContext = this;
        }
        #endregion

        #region Methods
        public static void UpdateAxis()
        {
            if (YAxes.Length > 0 && AppSettings.ModelCurrent.Statebook != null)
            {
                YAxes[0].Labels = AppSettings.ModelCurrent.Statebook.Values.ToArray();
            }
        }

        private void SetupEventHandlers()
        {
            ButtonStartRecord.Click += (_, __) => StartRecord();
            ButtonStopRecord.Click += (_, __) => StopRecord();
            ButtonClearRecognizedWords.Click += (_, __) => ClearRecognizedWords();
        }

        private void StartRecord()
        {
            _waveIn = new WaveInEvent
            {
                DeviceNumber = 0,
                WaveFormat = new WaveFormat(44100, 1)
            };
            _waveIn.DataAvailable += WaveIn_DataAvailable;

            _writer = new WaveFileWriter(_outputFile, _waveIn.WaveFormat);

            Dispatcher.Invoke(() =>
            {
                ButtonStartRecord.IsEnabled = false;
                ButtonStopRecord.IsEnabled = true;
            });

            _waveIn.StartRecording();
            _isRecording = true;
        }

        private void StopRecord()
        {
            _waveIn.StopRecording();
            _writer.Close();
            _isRecording = false;

            Dispatcher.Invoke(() =>
            {
                ButtonStartRecord.IsEnabled = true;
                ButtonStopRecord.IsEnabled = false;
            });
        }

        private void Recognize()
        {
            AudioBuilder builder = new AudioBuilder();
            builder.ReadAudio("recordedAudio.wav");
            AudioInformation audioInformation = builder.GetAudioInformation();
            if (audioInformation.Content.Length < 2000)
                return;
            (string word, double[][] probabilityMatrix) = ModelControlSystem.Recognize(audioInformation);

            OutputWord(word);
            OutputProbabilityMatrix(probabilityMatrix);
        }

        private void OutputProbabilityMatrix(double[][] probabilityMatrix)
        {
            int rows = probabilityMatrix.GetLength(0);
            int cols = probabilityMatrix[0].Length;

            var heatMapColors = new[]
            {
                new SKColor(36,151,243).AsLvcColor(),
                new SKColor(233,30,99).AsLvcColor(),
            };

            var values = new ObservableCollection<WeightedPoint>();

            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < cols; col++)
                {
                    double probability = probabilityMatrix[row][col];
                    int weightedValue = (int)(probability * 1000);

                    values.Add(new WeightedPoint(row, col, weightedValue));
                }
            }

            var heatSeries = new HeatSeries<WeightedPoint>
            {
                Name = "",
                HeatMap = heatMapColors,
                Values = values
            };

            var chartSeries = new ISeries[] { heatSeries };

            Dispatcher.Invoke(() =>
            {
                ChartRecognizedStates.Series = chartSeries;
            });
        }

        private void OutputWord(string word)
        {
            Dispatcher.Invoke(() =>
            {
                ListViewRecognizedWord.Items.Add(word);
            });
        }

        private void ClearRecognizedWords()
        {
            ListViewRecognizedWord.Items.Clear();
        }

        private void WaveIn_DataAvailable(object sender, WaveInEventArgs e)
        {
            if (!_isRecording)
                return;

            float energy = CalculateEnergy(e.Buffer, e.BytesRecorded);
            if (energy > _energyThreshold)
            {
                _countEmptyFrames = 0;
                _countVoiceFrames++;
                _isVoiceDetected = true;
                Debug.WriteLine(energy);
                _writer.Write(e.Buffer, 0, e.BytesRecorded);
            }
            else
            {
                _countEmptyFrames++;
                if (_isVoiceDetected && _countVoiceFrames >= 4 && _countEmptyFrames >= 3)
                {
                    StopRecord();
                    Recognize();
                    _countVoiceFrames = 0;
                    _isVoiceDetected = false;
                    StartRecord();
                }
            }
        }

        private static float CalculateEnergy(byte[] buffer, int length)
        {
            float sum = 0;
            for (int i = 0; i < length; i += 2)
            {
                short sample = BitConverter.ToInt16(buffer, i);
                sum += sample * sample;
            }
            return sum / (length / 2);
        } 
        #endregion
    }
}
