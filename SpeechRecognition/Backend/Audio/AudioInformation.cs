namespace Logic.Audio
{
    /// <summary>
    /// Represents information about audio data.
    /// </summary>
    public class AudioInformation
    {
        #region Fields
        /// <summary>
        /// Gets or sets the path of the audio file.
        /// </summary>
        public string Path { get; set; } = "";

        /// <summary>
        /// Gets or sets the name of the audio file.
        /// </summary>
        public string Name { get; set; } = "";

        /// <summary>
        /// Gets or sets the number of channels in the audio data.
        /// </summary>
        public int Channels { get; set; }

        /// <summary>
        /// Gets or sets the sample rate of the audio data.
        /// </summary>
        public int SampleRate { get; set; }

        /// <summary>
        /// Gets or sets the content of the audio data as an array of samples.
        /// </summary>
        public double[] Content { get; set; }
        #endregion
    }
}
