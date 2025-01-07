namespace Logic.Model
{
    /// <summary>
    /// Represents a speech recognition model that includes acoustic and language components.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="SRModel"/> class.
    /// </remarks>
    /// <param name="MFCC">The Mel-frequency cepstral coefficients.</param>
    /// <param name="DNN">The deep neural network used as the acoustic model.</param>
    public class SRModel(MelFrequencyCepstralCoefficients MFCC, DeepNeuralNetworks DNN)
    {
        /// <summary>
        /// Gets or sets the name of the model.
        /// </summary>
        public string Name { get; set; } = "";

        /// <summary>
        /// Gets or sets a value indicating whether the model has been trained.
        /// </summary>
        public bool IsTrained { get; set; }

        public bool IsBusy { get; set; }

        /// <summary>
        /// Gets or sets the creation date of the model.
        /// </summary>
        public string CreateDate { get; set; } = "";

        /// <summary>
        /// Gets or sets the total training time of the model.
        /// </summary>
        public string TrainingTime { get; set; } = "";

        /// <summary>
        /// Gets or sets the number of finished training cycles.
        /// </summary>
        public int NumFinishedCycles { get; set; }

        /// <summary>
        /// Gets or sets the number of epochs per state training cycle.
        /// </summary>
        public int NumStatesTrainingEpoch { get; set; }

        /// <summary>
        /// Gets or sets the number of epochs per word training cycle.
        /// </summary>
        public int NumWordsTrainingEpoch { get; set; }

        /// <summary>
        /// Gets or sets the total number of training cycles.
        /// </summary>
        public int NumCycles { get; set; }

        /// <summary>
        /// Gets or sets the Mel-frequency cepstral coefficients used for sound characteristics.
        /// </summary>
        public MelFrequencyCepstralCoefficients SoundsCharacteristics { get; set; } = MFCC;

        /// <summary>
        /// Gets or sets the deep neural network used as the acoustic model.
        /// </summary>
        public DeepNeuralNetworks AcousticModel { get; set; } = DNN;

        /// <summary>
        /// Gets or sets the dictionary mapping state indices to their corresponding names.
        /// </summary>
        public Dictionary<int, string> Statebook { get; set; } = [];

        /// <summary>
        /// Gets or sets the list of words recognized by the model.
        /// </summary>
        public List<string> Wordbook { get; set; } = [];

        /// <summary>
        /// Gets or sets the number of states in the model.
        /// </summary>
        public int NumStates { get; set; }

        /// <summary>
        /// Gets or sets the number of words in the model.
        /// </summary>
        public int NumWords { get; set; }
    }
}
