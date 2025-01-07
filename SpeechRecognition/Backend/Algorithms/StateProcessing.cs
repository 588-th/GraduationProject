namespace Logic.Algorithms
{
    /// <summary>
    /// Provides methods for processing state probabilities and extracting recognized states.
    /// </summary>
    public static class StateProcessing
    {
        /// <summary>
        /// Processes the given state probabilities and returns a list of recognized states.
        /// </summary>
        /// <param name="stateProbabilities">The probabilities of each state at each time step.</param>
        /// <returns>A list of recognized states.</returns>
        /// <exception cref="ArgumentNullException">Thrown when stateProbabilities is null or empty.</exception>
        public static List<int> GetWordStates(double[][] stateProbabilities)
        {
            if (stateProbabilities == null)
            {
                throw new ArgumentNullException(nameof(stateProbabilities), "State probabilities cannot be null or empty.");
            }

            List<int> recognizedStates = Greezly(stateProbabilities);
            ReplaceSingleStates(recognizedStates);
            RemoveBlankStates(recognizedStates);
            ReplaceSingleStates(recognizedStates);
            RemoveSingleStates(recognizedStates);
            CombineStates(recognizedStates);
            return recognizedStates;
        }

        public static List<List<int>> GetSentenceStates(double[][] stateProbabilities)
        {
            if (stateProbabilities == null)
            {
                throw new ArgumentNullException(nameof(stateProbabilities), "State probabilities cannot be null or empty.");
            }

            List<int> recognizedStates = Greezly(stateProbabilities);
            ReplaceSingleStates(recognizedStates);
            TrimSilence(recognizedStates);
            List<List<int>> unblankstates = SeparateUnblankStates(recognizedStates);

            for (int i = 0; i < unblankstates.Count; i++)
            {
                ReplaceSingleStates(unblankstates[i]);
                RemoveBlankStates(recognizedStates);
                ReplaceSingleStates(recognizedStates);
                RemoveSingleStates(unblankstates[i]);
                CombineStates(unblankstates[i]);
            }

            return unblankstates;
        }

        private static List<int> TrimSilence(List<int> recognizedStates)
        {
            for (int i = 0; i < recognizedStates.Count; i++)
            {
                if (recognizedStates[i] != 0)
                {
                    break;
                }
                else
                {
                    recognizedStates.RemoveAt(i);
                    i--;
                }
            }

            for (int i = recognizedStates.Count - 1; i >= 0; i--)
            {
                if (recognizedStates[i] != 0)
                {
                    break;
                }
                else
                {
                    recognizedStates.RemoveAt(i);
                }
            }

            return recognizedStates;
        }

        private static List<List<int>> SeparateUnblankStates(List<int> recognizedStates)
        {
            List<List<int>> unblankStates = new List<List<int>>();
            int countBlank = 0;
            int start = 0;

            for (int i = 0; i < recognizedStates.Count; i++)
            {
                if (recognizedStates[i] == 0)
                {
                    countBlank++;
                }

                if (recognizedStates[i] != 0)
                {
                    if (countBlank >= 10)
                    {
                        unblankStates.Add(recognizedStates.GetRange(start, i - countBlank - start));
                        start = i;
                    }

                    countBlank = 0;
                }

                if (i == recognizedStates.Count - 1 && i - start > 3)
                {
                    unblankStates.Add(recognizedStates.GetRange(start, i - start));
                }
            }

            return unblankStates;
        }

        /// <summary>
        /// Extracts recognized states from the given state probabilities using the Greezly algorithm.
        /// </summary>
        /// <param name="stateProbabilities">The probabilities of each state at each time step.</param>
        /// <returns>A list of recognized states.</returns>
        private static List<int> Greezly(double[][] stateProbabilities)
        {
            int numRows = stateProbabilities.Length;
            int state;
            List<int> statePath = [];

            for (int i = 0; i < numRows; i++)
            {
                state = FindMaxProbabilityState(stateProbabilities[i]);
                statePath.Add(state);
            }

            return statePath;
        }

        /// <summary>
        /// Finds the state with the maximum probability for each time step using the Greezly algorithm.
        /// </summary>
        /// <param name="stateProbabilities">The probabilities of each state at a specific time step.</param>
        /// <returns>The index of the state with the maximum probability.</returns>
        private static int FindMaxProbabilityState(double[] probabilities)
        {
            double maxProbability = double.MinValue;
            int maxIndex = -1;

            for (int i = 0; i < probabilities.Length; i++)
            {
                if (probabilities[i] > maxProbability)
                {
                    maxProbability = probabilities[i];
                    maxIndex = i;
                }
            }

            return maxIndex;
        }

        /// <summary>
        /// Replaces single states that are surrounded by the same state with the surrounding state.
        /// </summary>
        /// <param name="recognizedStates">The list of recognized states.</param>
        public static void ReplaceSingleStates(List<int> recognizedStates)
        {
            for (int i = 1; i < recognizedStates.Count - 1; i++)
            {
                if (recognizedStates[i] != recognizedStates[i + 1] && recognizedStates[i - 1] == recognizedStates[i + 1])
                {
                    recognizedStates[i] = recognizedStates[i + 1];
                }
            }
        }

        /// <summary>
        /// Removes single states that are not preceded or followed by the same state.
        /// </summary>
        /// <param name="recognizedStates">The list of recognized states.</param>
        public static void RemoveSingleStates(List<int> recognizedStates)
        {
            for (int i = 1; i < recognizedStates.Count - 1; i++)
            {
                if (recognizedStates[i] != recognizedStates[i + 1] && recognizedStates[i] != recognizedStates[i - 1])
                {
                    recognizedStates.RemoveAt(i);
                }
            }
        }

        /// <summary>
        /// Removes blank states (state with index 0) from the list of recognized states.
        /// </summary>
        /// <param name="recognizedStates">The list of recognized states.</param>
        public static void RemoveBlankStates(List<int> recognizedStates)
        {
            recognizedStates.RemoveAll(state => state == 0);
        }

        /// <summary>
        /// Combines adjacent duplicate states into one state.
        /// </summary>
        /// <param name="recognizedStates">The list of recognized states.</param>
        public static void CombineStates(List<int> recognizedStates)
        {
            for (int i = recognizedStates.Count - 1; i > 0; i--)
            {
                if (recognizedStates[i] == recognizedStates[i - 1])
                {
                    recognizedStates.RemoveAt(i);
                }
            }
        }
    }
}
