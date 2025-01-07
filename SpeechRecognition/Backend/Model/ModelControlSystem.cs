using Logic.Algorithms;
using Logic.Audio;
using System.Diagnostics;

namespace Logic.Model
{
    /// <summary>
    /// Provides control methods for training, retraining, and recognizing speech using the SRModel.
    /// </summary>
    public static class ModelControlSystem
    {
        /// <summary>
        /// Event triggered when a training cycle is finished.
        /// </summary>
        public static event EventHandler? CycleFinished;

        /// <summary>
        /// Event triggered when the calculation of word accuracy is finished.
        /// </summary>
        public static event EventHandler? CalculateAccuracyOfWordFinished;

        /// <summary>
        /// The speech recognition model being controlled.
        /// </summary>
        public static SRModel Model { get; set; }

        public static bool StopTraining { get; set; }

        /// <summary>
        /// Trains the model, building the acoustic model if it is not already trained.
        /// </summary>
        public static void Train()
        {
            Model.IsBusy = true;

            if (!Model.IsTrained)
            {
                Model.AcousticModel.Build();
                TrainStates();
            }

            for (int i = 0; i < Model.NumCycles; i++)
            {
                if (StopTraining)
                {
                    StopTraining = false;
                    return;
                }

                TrainWords();

                if (StopTraining)
                {
                    StopTraining = false;
                    Model.IsBusy = false;
                    return;
                }

                Model.NumFinishedCycles += 1;
                OnCicleFinished(null, EventArgs.Empty);
            }
            Model.IsTrained = true;
            Model.IsBusy = false;
        }

        public static void StopTrain()
        {
            StopTraining = true;
            Model.AcousticModel.StopTraining();
        }

        /// <summary>
        /// Retrains the model by resetting training progress and training again.
        /// </summary>
        public static void Retrain()
        {
            Model.NumFinishedCycles = 0;
            Model.IsTrained = false;
            Train();
        }

        /// <summary>
        /// Recognizes speech using the current audio input.
        /// </summary>
        /// <returns>A tuple containing the recognized word and the probability matrix.</returns>
        /// <exception cref="Exception">Thrown if the model is not trained.</exception>
        public static (string, double[][]) Recognize()
        {
            if (!Model.IsTrained)
            {
                throw new Exception("Model is not trained");
            }

            List<double[]> soundCharactVectors = Model.SoundsCharacteristics.Extract(AudioRepository.GetAudioFileFromName(AppSettings.AudioCurrent, AudioRepository.RecognizingAudio));
            double[][] probabilityMatrix = Model.AcousticModel.Predict(soundCharactVectors);
            List<int> recognizedStates = StateProcessing.GetWordStates(probabilityMatrix);
            string word = WordbookMatcher.ConvertingStatesToLetters(Model.Statebook, recognizedStates);
            return (WordbookMatcher.FindMostSimilarWord(Model.Wordbook, word), probabilityMatrix);
        }

        /// <summary>
        /// Recognizes speech using the provided audio information.
        /// </summary>
        /// <param name="audioInformation">The audio information to recognize.</param>
        /// <returns>A tuple containing the recognized word and the probability matrix.</returns>
        /// <exception cref="Exception">Thrown if the model is not trained.</exception>
        public static (string, double[][]) Recognize(AudioInformation audioInformation)
        {
            if (!Model.IsTrained)
                Train();

            List<double[]> soundCharactVectors = Model.SoundsCharacteristics.Extract(audioInformation);
            double[][] probabilityMatrix = Model.AcousticModel.Predict(soundCharactVectors);
            List<int> recognizedStates = StateProcessing.GetWordStates(probabilityMatrix);
            if (recognizedStates.Count == 0)
            {
                return ("\"Тишина\"", probabilityMatrix);
            }
            string word = WordbookMatcher.ConvertingStatesToLetters(Model.Statebook, recognizedStates);
            return (WordbookMatcher.FindMostSimilarWord(Model.Wordbook, word), probabilityMatrix);
        }

        /// <summary>
        /// Trains the acoustic model using state-level training data.
        /// </summary>
        private static void TrainStates()
        {
            List<double[]> soundCharactVectors = new List<double[]>();
            List<int> states = new List<int>();

            foreach (KeyValuePair<string, List<AudioInformation>> trainingPair in AudioRepository.TrainingStates)
            {
                string key = Model.Statebook.FirstOrDefault(x => x.Value == trainingPair.Key).Value;
                if (key == null)
                    continue;

                Debug.WriteLine("[Model] " + trainingPair.Key);
                foreach (AudioInformation audioInformation in trainingPair.Value)
                {
                    if (StopTraining)
                    {
                        return;
                    }

                    List<double[]> soundsCharacteristics = Model.SoundsCharacteristics.Extract(audioInformation);
                    soundCharactVectors.AddRange(soundsCharacteristics);
                    states.AddRange(Enumerable.Repeat(Model.Statebook.First(kv => kv.Value == trainingPair.Key).Key, soundsCharacteristics.Count));
                }
            }

            Debug.WriteLine("[Model] Training From States Start");
            Model.AcousticModel.Train(soundCharactVectors, states, Model.NumStatesTrainingEpoch);
            Debug.WriteLine("[Model] Train From TrainStates Complete");
        }

        /// <summary>
        /// Trains the acoustic model using word-level training data.
        /// </summary>
        private static void TrainWords()
        {
            List<double[]> soundCharactVectors = new List<double[]>();
            List<int> states = new List<int>();

            foreach (KeyValuePair<string, List<AudioInformation>> trainingPair in AudioRepository.TrainingWords)
            {
                foreach (AudioInformation audioInformation in trainingPair.Value)
                {
                    if (StopTraining)
                    {
                        return;
                    }

                    string[] keys = trainingPair.Key.Split("_");
                    List<int> targetStates = new List<int> { 0 };
                    targetStates.AddRange(keys.Select(k => Model.Statebook.First(x => x.Value == k).Key));
                    targetStates.Add(0);

                    List<double[]> soundsCharacteristics = Model.SoundsCharacteristics.Extract(audioInformation);
                    double[][] probMatrix = Model.AcousticModel.Predict(soundsCharacteristics);

                    Debug.WriteLine("[Model] " + trainingPair.Key);
                    List<int> transcription = TranscriptionAlignment.GetStates(probMatrix, targetStates);
                    if (transcription != null)
                    {
                        Debug.WriteLine("[Model] Transcription Success");
                        soundCharactVectors.AddRange(soundsCharacteristics);
                        states.AddRange(transcription);
                    }
                    Debug.WriteLine("-----------------------");
                }
            }

            Debug.WriteLine("[Model] Training From Words Start");
            Model.AcousticModel.Train(soundCharactVectors, states, Model.NumWordsTrainingEpoch);
            Debug.WriteLine("[Model] Train From Words Complete");
        }

        /// <summary>
        /// Calculates the accuracy of the model for each word in the wordbook.
        /// </summary>
        /// <returns>A dictionary with words as keys and their accuracy percentages as values.</returns>
        /// <exception cref="Exception">Thrown if the model is not trained.</exception>
        public static Dictionary<string, double> CalculateAccuracy()
        {
            if (!Model.IsTrained)
            {
                throw new Exception("Model is not trained");
            }

            Dictionary<string, double> accuracy = new Dictionary<string, double>();

            foreach (KeyValuePair<string, List<AudioInformation>> trainingPair in AudioRepository.TrainingWords)
            {
                string key = trainingPair.Key.Replace("_", "");
                if (!Model.Wordbook.Contains(key))
                    continue;

                int totalPredictions = 0;
                int correctPredictions = 0;

                foreach (AudioInformation audioInformation in trainingPair.Value)
                {
                    List<double[]> soundCharactVectors = Model.SoundsCharacteristics.Extract(audioInformation);
                    double[][] probabilityMatrix = Model.AcousticModel.Predict(soundCharactVectors);
                    List<int> recognizedStates = StateProcessing.GetWordStates(probabilityMatrix);
                    string word = WordbookMatcher.ConvertingStatesToLetters(Model.Statebook, recognizedStates);
                    string searchedWord = WordbookMatcher.FindMostSimilarWord(Model.Wordbook, word);

                    totalPredictions++;
                    if (searchedWord == key)
                        correctPredictions++;
                }

                accuracy.Add(key, totalPredictions == 0 ? 0 : (double)correctPredictions / totalPredictions * 100);
                OnCalculateAccuracityOfWordFinished(null, EventArgs.Empty);
            }

            return accuracy;
        }

        /// <summary>
        /// Triggers the CicleFinished event.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private static void OnCicleFinished(object sender, EventArgs e)
        {
            CycleFinished?.Invoke(sender, e);
        }

        /// <summary>
        /// Triggers the CalculateAccuracityOfWordFinished event.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private static void OnCalculateAccuracityOfWordFinished(object sender, EventArgs e)
        {
            CalculateAccuracyOfWordFinished?.Invoke(sender, e);
        }
    }
}
