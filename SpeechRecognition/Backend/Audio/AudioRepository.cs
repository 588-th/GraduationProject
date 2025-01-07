namespace Logic.Audio
{
    /// <summary>
    /// Provides methods for initializing and managing audio repositories.
    /// </summary>
    public static class AudioRepository
    {
        #region Fields
        /// <summary>
        /// Gets the dictionary containing training audio information for states.
        /// </summary>
        public static Dictionary<string, List<AudioInformation>> TrainingStates { get; private set; } = [];

        /// <summary>
        /// Gets the dictionary containing training audio information for words.
        /// </summary>
        public static Dictionary<string, List<AudioInformation>> TrainingWords { get; private set; } = [];

        /// <summary>
        /// Gets the dictionary containing audio information for recognizing audio.
        /// </summary>
        public static Dictionary<string, List<AudioInformation>> RecognizingAudio { get; private set; } = [];
        #endregion

        #region Methods
        /// <summary>
        /// Initializes the audio repository by loading training states, training words, and recognizing audio.
        /// </summary>
        public static void InitRepository()
        {
            TrainingStates = InitStatesAudio(AppSettings.AudioTrainingStatesFolderPath);
            TrainingWords = InitWordsAudio(AppSettings.AudioTrainingWordsFolderPath);
            RecognizingAudio = InitRecognizingAudio(AppSettings.AudioRecognizeFolderPath);
        }

        /// <summary>
        /// Initializes the audio repository for training states from the specified folder path.
        /// </summary>
        /// <param name="folderPath">The folder path containing state audio files.</param>
        /// <returns>A dictionary containing audio information for training states.</returns>
        public static Dictionary<string, List<AudioInformation>> InitStatesAudio(string folderPath)
        {
            Dictionary<string, List<AudioInformation>> audioDictionary = new();
            string[] subDirectories = Directory.GetDirectories(folderPath);

            foreach (string subDirectory in subDirectories)
            {
                string[] subDirAudioFiles = Directory.GetFiles(subDirectory, "*.wav").OrderBy(file => file).ToArray();

                foreach (string file in subDirAudioFiles)
                {
                    string directoryName = GetDirectoryName(subDirectory);

                    var builder = new AudioBuilder();
                    builder.ReadAudio(file);
                    if (directoryName != "-")
                    {
                        builder.TrimSilence(AppSettings.TrimSilentsThreshold);
                    }
                    AudioInformation audioInformation = builder.GetAudioInformation();

                    if (audioDictionary.ContainsKey(directoryName))
                    {
                        audioDictionary[directoryName].Add(audioInformation);
                    }
                    else
                    {
                        audioDictionary[directoryName] = new List<AudioInformation> { audioInformation };
                    }
                }
            }

            return audioDictionary;
        }

        /// <summary>
        /// Initializes the audio repository for training words from the specified folder path.
        /// </summary>
        /// <param name="folderPath">The folder path containing word audio files.</param>
        /// <returns>A dictionary containing audio information for training words.</returns>
        public static Dictionary<string, List<AudioInformation>> InitWordsAudio(string folderPath)
        {
            Dictionary<string, List<AudioInformation>> audioDictionary = new();
            string[] subDirectories = Directory.GetDirectories(folderPath);

            foreach (string subDirectory in subDirectories)
            {
                string[] subDirAudioFiles = Directory.GetFiles(subDirectory, "*.wav").OrderBy(file => file).ToArray();

                foreach (string file in subDirAudioFiles)
                {
                    var builder = new AudioBuilder();
                    builder.ReadAudio(file);
                    AudioInformation audioInformation = builder.GetAudioInformation();

                    string directoryName = GetDirectoryName(subDirectory);

                    if (audioDictionary.ContainsKey(directoryName))
                    {
                        audioDictionary[directoryName].Add(audioInformation);
                    }
                    else
                    {
                        audioDictionary[directoryName] = new List<AudioInformation> { audioInformation };
                    }
                }
            }

            return audioDictionary;
        }

        /// <summary>
        /// Initializes the audio repository for recognizing audio from the specified folder path.
        /// </summary>
        /// <param name="folderPath">The folder path containing recognizing audio files.</param>
        /// <returns>A dictionary containing audio information for recognizing audio.</returns>
        public static Dictionary<string, List<AudioInformation>> InitRecognizingAudio(string folderPath)
        {
            Dictionary<string, List<AudioInformation>> audioDictionary = new();
            string[] subDirectories = Directory.GetDirectories(folderPath);

            foreach (string subDirectory in subDirectories)
            {
                string[] subSubDirectories = Directory.GetDirectories(subDirectory);

                foreach (string subSubDirectory in subSubDirectories)
                {
                    string[] subDirAudioFiles = Directory.GetFiles(subSubDirectory, "*.wav").OrderBy(file => file).ToArray();

                    foreach (string file in subDirAudioFiles)
                    {
                        var builder = new AudioBuilder();
                        builder.ReadAudio(file);
                        AudioInformation audioInformation = builder.GetAudioInformation();

                        string directoryName = GetDirectoryName(subSubDirectory);

                        if (audioDictionary.ContainsKey(directoryName))
                        {
                            audioDictionary[directoryName].Add(audioInformation);
                        }
                        else
                        {
                            audioDictionary[directoryName] = new List<AudioInformation> { audioInformation };
                        }
                    }
                }
            }

            return audioDictionary;
        }

        /// <summary>
        /// Gets the name of the directory from the specified path.
        /// </summary>
        /// <param name="path">The path to get the directory name from.</param>
        /// <returns>The name of the directory.</returns>
        private static string GetDirectoryName(string path)
        {
            var parts = path.Split('\\');
            return parts[^1];
        }

        /// <summary>
        /// Gets the audio file from the specified file name and audio dictionary.
        /// </summary>
        /// <param name="fileName">The name of the audio file.</param>
        /// <param name="audioDictionary">The dictionary containing audio information.</param>
        /// <returns>The audio information corresponding to the specified file name.</returns>
        public static AudioInformation GetAudioFileFromName(string fileName, Dictionary<string, List<AudioInformation>> audioDictionary)
        {
            AudioInformation audioInformation = new();
            foreach (var item in audioDictionary)
            {
                audioInformation = item.Value.Find(f => f.Name == fileName);
                if (audioInformation != null)
                    break;
            }

            return audioInformation;
        }
        #endregion
    }
}
