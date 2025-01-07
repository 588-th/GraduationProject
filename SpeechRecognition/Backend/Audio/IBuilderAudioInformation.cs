namespace Logic.Audio
{
    public interface IBuilderAudioInformation
    {
        void ReadAudio(string audioFilePath);
        void TrimSilence(double thresholdDB);
    }
}
