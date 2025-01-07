using NAudio.Wave;

namespace Logic.Audio
{
    public class AudioBuilder : IBuilderAudioInformation
    {
        private AudioInformation _audioInformation = new();

        public AudioBuilder()
        {
            Reset();
        }

        public void Reset()
        {
            _audioInformation = new AudioInformation();
        }

        public void ReadAudio(string audioFilePath)
        {
            using AudioFileReader reader = new(audioFilePath);
            float[] audioData = new float[reader.Length / sizeof(float)];
            reader.Read(audioData, 0, audioData.Length);

            _audioInformation.Path = audioFilePath;
            _audioInformation.Name = Path.GetFileName(audioFilePath);
            _audioInformation.Channels = reader.WaveFormat.Channels;
            _audioInformation.SampleRate = reader.WaveFormat.SampleRate;
            _audioInformation.Content = audioData.Select(sample => (double)sample).ToArray();
        }

        public void TrimSilence(double thresholdDB)
        {
            var content = _audioInformation.Content;

            int startIndex = 0;
            int endIndex = content.Length - 1;

            while (startIndex < endIndex && CalculateDB(content[startIndex]) < thresholdDB)
            {
                startIndex++;
            }

            while (endIndex > startIndex && CalculateDB(content[endIndex]) < thresholdDB)
            {
                endIndex--;
            }

            if (startIndex > 0 || endIndex < content.Length - 1)
            {
                int newLength = endIndex - startIndex + 1;
                double[] trimmedContent = new double[newLength];
                Array.Copy(content, startIndex, trimmedContent, 0, newLength);
                content = trimmedContent;
            }

            _audioInformation.Content = content;
        }

        public AudioInformation GetAudioInformation()
        {
            AudioInformation audioInformation = _audioInformation;
            Reset();
            return audioInformation;
        }

        private static double CalculateDB(double amplitude)
        {
            return 20 * Math.Log10(Math.Abs(amplitude));
        }
    }
}
