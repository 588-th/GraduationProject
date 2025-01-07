using Logic.Model;

namespace Logic
{
    public static class AppSettings
    {
        #region Fields

        private static string _appSettingsFloderPath = "Materials\\AppSettings";

        // Model
        private static SRModel _modelCurrent;
        private static string _modelsFolderPath = "Materials\\Models";

        // Audio
        private static string _audioCurrent = "";
        private static string _audioTrainingStatesFolderPath = "Materials\\TrainingAudio\\Phonemes";
        private static string _audioTrainingWordsFolderPath = "Materials\\TrainingAudio\\Words";
        private static string _audioRecognizeFolderPath = "Materials\\RecongnizingAudio";

        // Mel Frequency Cepstral Coefficients
        private static int _defaultNumFilters = 26;
        private static int _defaultNumCepstralCoefficients = 12;
        private static int _defaultFrameDurationMs = 20;
        private static int _defaultFrameOverlapDurationMs = 10;

        // DNN
        private static int _defaultNumStatesTrainingEpoch = 10;
        private static int _defaultNumWordsTrainingEpoch = 10;
        private static int _defaultNumCicles = 3;

        // Audio Procesing
        private static int _trimSilentsThreshold = -50;
        #endregion

        #region Accessors
        public static string AppSettingsFloderPath
        {
            get { return _appSettingsFloderPath; }
            set { _appSettingsFloderPath = value; }
        }

        public static SRModel ModelCurrent
        {
            get { return _modelCurrent; }
            set { _modelCurrent = value; }
        }

        public static string ModelsFolderPath
        {
            get { return _modelsFolderPath; }
            set { _modelsFolderPath = value; }
        }

        // Audio
        public static string AudioCurrent
        {
            get { return _audioCurrent; }
            set { _audioCurrent = value; }
        }

        public static string AudioTrainingStatesFolderPath
        {
            get { return _audioTrainingStatesFolderPath; }
            set { if (DirectoryExist(value)) _audioTrainingStatesFolderPath = value; }
        }

        public static string AudioTrainingWordsFolderPath
        {
            get { return _audioTrainingWordsFolderPath; }
            set { if (DirectoryExist(value)) _audioTrainingWordsFolderPath = value; }
        }

        public static string AudioRecognizeFolderPath
        {
            get { return _audioRecognizeFolderPath; }
            set { if (DirectoryExist(value)) _audioRecognizeFolderPath = value; }
        }

        // Mel Frequency Cepstral Coefficients
        public static int DefaultNumFilters
        {
            get { return _defaultNumFilters; }
            set { if (AboveZero(value)) _defaultNumFilters = value; }
        }
        public static int DefaultNumCepstralCoefficients
        {
            get { return _defaultNumCepstralCoefficients; }
            set { if (AboveZero(value)) _defaultNumCepstralCoefficients = value; }
        }
        public static int DefaultFrameDurationMs
        {
            get { return _defaultFrameDurationMs; }
            set { if (AboveZero(value)) _defaultFrameDurationMs = value; }
        }
        public static int DefaultFrameOverlapDurationMs
        {
            get { return _defaultFrameOverlapDurationMs; }
            set { if (AboveZero(value)) _defaultFrameOverlapDurationMs = value; }
        }

        // DNN
        public static int DefaultNumStatesTrainingEpoch
        {
            get { return _defaultNumStatesTrainingEpoch; }
            set { if (AboveZero(value)) _defaultNumStatesTrainingEpoch = value; }
        }
        public static int DefaultNumWordsTrainingEpoch
        {
            get { return _defaultNumWordsTrainingEpoch; }
            set { if (AboveZero(value)) _defaultNumWordsTrainingEpoch = value; }
        }
        public static int DefaultNumCicles
        {
            get { return _defaultNumCicles; }
            set { if (AboveZero(value)) _defaultNumCicles = value; }
        }

        // Audio Processing
        public static int TrimSilentsThreshold
        {
            get { return _trimSilentsThreshold; }
            set { _trimSilentsThreshold = value; }
        }
        #endregion

        #region Methods
        public static bool AboveZero(int number)
        {
            if (number <= 0)
                return false;

            return true;
        }

        public static bool DirectoryExist(string path)
        {
            if (!Directory.Exists(path))
                return false;

            return true;
        }
        #endregion
    }
}
