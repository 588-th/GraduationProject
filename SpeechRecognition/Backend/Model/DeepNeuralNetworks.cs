using System.Diagnostics;
using Tensorflow;
using Tensorflow.Keras.Engine;
using Tensorflow.NumPy;
using static Tensorflow.Binding;
using static Tensorflow.KerasApi;

namespace Logic.Model
{
    /// <summary>
    /// Represents a deep neural network model tailored for sound characteristic analysis.
    /// </summary>
    public class DeepNeuralNetworks(int numStates, int vectorSize)
    {
        #region Fields

        /// <summary>
        /// Gets or sets the number of states (classes) for the neural network.
        /// </summary>
        public int NumStates { get; set; } = numStates;

        /// <summary>
        /// Gets or sets the size of the input vector.
        /// </summary>
        public int VectorSize { get; set; } = vectorSize;

        /// <summary>
        /// The sequential model used for the deep neural network.
        /// </summary>
        private Sequential _model;

        /// <summary>
        /// The cancellation token source for stopping the training.
        /// </summary>
        private CancellationTokenSource _cancellationTokenSource;

        #endregion

        #region Methods

        /// <summary>
        /// Builds the deep neural network model with a specific architecture.
        /// </summary>
        public void Build()
        {
            int width = 1;
            int height = VectorSize;
            int numClasses = NumStates;
            float scale = 1.0f / 100;

            _model = keras.Sequential();
            _model.add(keras.layers.Rescaling(scale, input_shape: new Shape(width, height)));
            _model.add(tf.keras.layers.Dense(units: 128, activation: tf.keras.activations.Relu));
            _model.add(tf.keras.layers.Dense(units: 128, activation: tf.keras.activations.Relu));
            _model.add(tf.keras.layers.Dense(units: 128, activation: tf.keras.activations.Relu));
            _model.add(tf.keras.layers.Dense(units: numClasses, activation: tf.keras.activations.Softmax));
            _model.summary();

            Debug.WriteLine("[DeepNeuralNetworks] Build DNN Complete");
            Debug.WriteLine("Num GPUs Available: ", len(tf.config.list_physical_devices("GPU")));
        }

        /// <summary>
        /// Trains the neural network using the provided sound characteristics and states.
        /// </summary>
        /// <param name="soundCharacteristics">A list of sound characteristic vectors.</param>
        /// <param name="states">A list of corresponding state labels.</param>
        /// <param name="epochs">The number of training epochs.</param>
        public void Train(List<double[]> soundCharacteristics, List<int> states, int epochs)
        {
            _cancellationTokenSource = new CancellationTokenSource();

            _model.compile(optimizer: tf.keras.optimizers.Adam(),
                           loss: tf.keras.losses.SparseCategoricalCrossentropy(),
                           metrics: new[] { tf.keras.metrics.SparseCategoricalAccuracy() });

            double[,] arrmfccVectors = new double[soundCharacteristics.Count, 12];
            for (int i = 0; i < arrmfccVectors.GetLength(0); i++)
            {
                for (int j = 0; j < arrmfccVectors.GetLength(1); j++)
                {
                    arrmfccVectors[i, j] = soundCharacteristics[i][j];
                }
            }

            var input = np.array(arrmfccVectors);
            var target = np.array(states.ToArray());

            // Train the model with cancellation support
            Task.Run(() =>
            {
                try
                {
                    for (int epoch = 0; epoch < epochs; epoch++)
                    {
                        if (_cancellationTokenSource.Token.IsCancellationRequested)
                        {
                            Debug.WriteLine("[DeepNeuralNetworks] Training cancelled");
                            return;
                        }

                        _model.fit(x: input, y: target, epochs: 1);
                    }
                }
                catch (OperationCanceledException)
                {
                    Debug.WriteLine("[DeepNeuralNetworks] Training stopped due to cancellation");
                }
                Console.WriteLine("[DeepNeuralNetworks] Train DNN Complete");
            }, _cancellationTokenSource.Token);
        }

        /// <summary>
        /// Requests to stop the ongoing training process.
        /// </summary>
        public void StopTraining()
        {
            _cancellationTokenSource?.Cancel();
        }

        /// <summary>
        /// Predicts the states based on the provided sound characteristics.
        /// </summary>
        /// <param name="soundCharacteristics">A list of sound characteristic vectors.</param>
        /// <returns>A 2D array of probabilities for each state.</returns>
        public double[][] Predict(List<double[]> soundCharacteristics)
        {
            double[][] emptyMatrix = [];
            if (soundCharacteristics.Count == 0)
                return emptyMatrix;

            double[,] mfccVectorsMatrix = new double[soundCharacteristics.Count, 12];
            for (int i = 0; i < mfccVectorsMatrix.GetLength(0); i++)
            {
                for (int j = 0; j < mfccVectorsMatrix.GetLength(1); j++)
                {
                    mfccVectorsMatrix[i, j] = soundCharacteristics[i][j];
                }
            }

            NDArray input = np.array(mfccVectorsMatrix);
            Tensor predictions = _model.predict(new[] { input });
            NDArray predictionsArray = predictions.numpy();

            long length = predictionsArray.shape[0];
            long width = predictionsArray.shape[1];

            double[][] probabilityMatrix = new double[length][];
            for (int i = 0; i < length; i++)
            {
                probabilityMatrix[i] = new double[width];
                for (int j = 0; j < width; j++)
                {
                    probabilityMatrix[i][j] = predictionsArray[i, j];
                }
            }

            return probabilityMatrix;
        }

        /// <summary>
        /// Saves the trained model to the specified path.
        /// </summary>
        /// <param name="path">The file path to save the model.</param>
        public void SaveModel(string path)
        {
            _model.save(path);
        }

        /// <summary>
        /// Loads a trained model from the specified path.
        /// </summary>
        /// <param name="path">The file path to load the model from.</param>
        public void LoadModel(string path)
        {
            IModel loadedModel = keras.models.load_model(path);
            _model = (Sequential)loadedModel;
        }

        #endregion
    }
}
