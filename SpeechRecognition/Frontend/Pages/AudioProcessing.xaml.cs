using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView.WPF;
using Logic;
using Logic.Audio;
using NAudio.Wave;
using SkiaSharp;
using System.Windows.Controls;

namespace Interface.Pages
{
    /// <summary>
    /// Логика взаимодействия для AudioProcessing.xaml
    /// </summary>
    public partial class AudioProcessing : Page
    {
        #region Fields
        public event EventHandler? TrimSilentsThresholdChanged;

        private WaveOutEvent? _waveOut;
        #endregion

        #region Constructor
        public AudioProcessing()
        {
            InitializeComponent();
            SetupEventHandlers();
            LoadAudioProcessingData();
            DataContext = this;
        }
        #endregion

        #region Methods
        private void SetupEventHandlers()
        {
            ButtonApply.Click += (_, __) => SaveAudioProcessingData();

            FileExplorerRecognize.FileSelected += (_, __) => UpdateAudioFileInformation();
            FileExplorerRecognize.DoubleClick += async (_, __) => await OutputAudio(true);
            FileExplorerRecognize.DoubleClick += async (_, __) => await OutputAudio(false);

            ButtonProcessing.Click += async (_, __) => await OutputAudio(true);
            ButtonProcessing.Click += async (_, __) => await OutputAudio(false);

            ButtonPlayStopAudio.Click += (_, __) => PlayStopAudio();
        }

        private void LoadAudioProcessingData()
        {
            TextBoxTrimSilentsThreshold.Text = AppSettings.TrimSilentsThreshold.ToString();
        }

        private void SaveAudioProcessingData()
        {
            AppSettings.TrimSilentsThreshold = int.Parse(TextBoxTrimSilentsThreshold.Text);
            OnTrimSilentsThresholdChanged(this, EventArgs.Empty);
        }

        private void UpdateAudioFileInformation()
        {
            string path = FileExplorerRecognize.CurrentFile;

            AudioBuilder audioBuilder = new();
            audioBuilder.ReadAudio(path);
            AudioInformation audioInformation = audioBuilder.GetAudioInformation();

            AppSettings.AudioCurrent = audioInformation.Name;

            TextBlockAudioFileInformation.Text = $"Name: {audioInformation.Name}\nChannels: {audioInformation.Channels}\nSampleRate: {audioInformation.SampleRate}";
        }

        private async Task OutputAudio(bool isSource)
        {
            string path = FileExplorerRecognize.CurrentFile;

            AudioBuilder audioBuilder = new();
            audioBuilder.ReadAudio(path);
            if (isSource)
                audioBuilder.TrimSilence(AppSettings.TrimSilentsThreshold);

            AudioInformation audioInformation = audioBuilder.GetAudioInformation();

            if (isSource)
                await CreateChartAsync(ChartProcessingAudio, audioInformation.Content, new SKColor(33, 150, 243));
            else
                await CreateChartAsync(ChartSourseAudio, audioInformation.Content, new SKColor(244, 67, 54));
        }

        public static async Task CreateChartAsync(CartesianChart chart, double[] data, SKColor color)
        {
            SolidColorPaint solidColorPaint = new() { Color = color, StrokeThickness = 2 };

            LineSeries<double> series = new()
            {
                Values = data,
                Fill = null,
                GeometrySize = 0,
                Stroke = solidColorPaint,
                GeometryStroke = solidColorPaint
            };

            chart.Series = new[] { series };
        }

        private void PlayStopAudio()
        {
            AudioInformation audioInformation = AudioRepository.GetAudioFileFromName(AppSettings.AudioCurrent, AudioRepository.RecognizingAudio);

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

        private void OnTrimSilentsThresholdChanged(object sender, EventArgs e)
        {
            TrimSilentsThresholdChanged?.Invoke(sender, e);
        }
        #endregion
    }
}
