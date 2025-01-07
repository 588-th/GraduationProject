using Interface.Pages;
using Interface.Windows;

namespace Interface
{
    public static class AppWindows
    {
        public static readonly Guide guide = new();
        public static readonly ModelList modelList = new();
        public static readonly ModelSettings modelSettings = new();
        public static readonly ModelDictionaries modelDictionaries = new();
        public static readonly AudioRepositories audioRepositories = new();
        public static readonly AudioProcessing audioProcessing = new();
        public static readonly ModelTrain modelTrain = new();
        public static readonly Transcription transcription = new();
        public static readonly Realtime realtime = new();
    }
}
