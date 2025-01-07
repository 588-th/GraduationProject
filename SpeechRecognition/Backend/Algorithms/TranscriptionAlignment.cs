using System.Diagnostics;
using System.Text;

namespace Logic.Algorithms
{
    /// <summary>
    /// Provides methods for aligning transcriptions.
    /// </summary>
    public static class TranscriptionAlignment
    {
        /// <summary>
        /// Gets the aligned states based on the state probabilities and target states.
        /// </summary>
        /// <param name="stateProbabilities">The probabilities of each state at each time step.</param>
        /// <param name="targetStates">The target states.</param>
        /// <returns>The aligned states.</returns>
        /// <exception cref="ArgumentNullException">Thrown when stateProbabilities or targetStates is null.</exception>
        public static List<int>? GetStates(double[][] stateProbabilities, List<int> targetStates)
        {
            if (stateProbabilities == null)
            {
                throw new ArgumentNullException(nameof(stateProbabilities), "State probabilities cannot be null or empty.");
            }

            if (targetStates == null)
            {
                throw new ArgumentNullException(nameof(targetStates), "Target states cannot be null or empty.");
            }

            List<int> recognizedStates = Greezly(stateProbabilities);

            ReplaceSingleStates(recognizedStates);
            (List<int> fittedStates, bool succes) = FitRecognizedStates(recognizedStates, targetStates);

            DebugOutputStates(recognizedStates);
            DebugOutputStates(fittedStates);

            if (!succes)
            {
                return null;
            }

            return fittedStates;
        }

        /// <summary>
        /// Extracts recognized states from the given state probabilities using the Greezly algorithm.
        /// </summary>
        /// <param name="stateProbabilities">The probabilities of each state at each time step.</param>
        /// <returns>A list of recognized states.</returns>
        public static List<int> Greezly(double[][] stateProbabilities)
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
        /// <param name="probabilities">The probabilities of each state at a specific time step.</param>
        /// <returns>The index of the state with the maximum probability.</returns>
        public static int FindMaxProbabilityState(double[] probabilities)
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
        /// Fits the recognized states to the target states.
        /// </summary>
        public static (List<int>, bool) FitRecognizedStates(List<int> recognizedStates, List<int> targetStates)
        {
            List<int> fittedStates = [];
            List<List<int>> segments = SeparateSegments(recognizedStates);

            int targetIndex = 0;
            int segmentIndex = 0;
            while (segmentIndex < segments.Count)
            {
                // If the segment matches the expected one, record it.
                if (segments[segmentIndex][0] == targetStates[targetIndex])
                {
                    fittedStates.AddRange(segments[segmentIndex]);
                    segmentIndex++;
                    continue;
                }

                // Look for the expected segment in the next 4 frames of the following segments.
                int segmentCount = segments[segmentIndex].Count;
                int j = segmentIndex + 1;
                bool replace = false;
                while (segmentCount <= 4 && j < segments.Count - 1) // -1 to prevent accessing the last (zeroth) segment.
                {
                    if (segments[j][0] == targetStates[targetIndex])
                    {
                        replace = true;
                        break;
                    }

                    segmentCount += segments[j].Count;
                    j++;
                }

                // If the expected segment is found within the next segments within 4 frames, record all these segments under one tag.
                if (replace)
                {
                    int k = segmentIndex;
                    while (k <= j)
                    {
                        fittedStates.AddRange(Enumerable.Repeat(targetStates[targetIndex], segments[k].Count));
                        k++;
                    }
                    segmentIndex = j + 1;
                    continue;
                }

                // If the segment is the first one, replace with the first expected one (zero).
                if (targetIndex == 0 && fittedStates.Count == 0)
                {
                    fittedStates.AddRange(Enumerable.Repeat(targetStates[targetIndex], segments[segmentIndex].Count));
                    segmentIndex++;
                    continue;
                }

                // If the segment is the last one, replace with the last expected one (zero).
                if (targetIndex == targetStates.Count - 1)
                {
                    fittedStates.AddRange(Enumerable.Repeat(targetStates[targetIndex], segments[segmentIndex].Count));
                    segmentIndex++;
                    continue;
                }

                // If the segment is zero, move to the next target.
                if (targetIndex == 0)
                {
                    targetIndex++;
                    continue;
                }

                List<List<int>> fittedStatesSegments = SeparateSegments(fittedStates);

                // If the segment with the target is already recorded and is large enough, move to the next target.
                if (targetStates[targetIndex] == fittedStatesSegments[fittedStatesSegments.Count - 1][0] && fittedStatesSegments[fittedStatesSegments.Count - 1].Count >= 4)
                {
                    targetIndex++;
                    continue;
                }

                // If the segment is not the last one.
                if (segmentIndex < segments.Count - 1)
                {
                    // Create/append the expected segment using the current segment.
                    fittedStates.AddRange(Enumerable.Repeat(targetStates[targetIndex], segments[segmentIndex].Count));
                    segmentIndex++;
                    continue;
                }

                targetIndex++;
            }

            segments = SeparateSegments(fittedStates);
            if (segments.Count != targetStates.Count)
            {
                return (fittedStates, false);
            }

            int numDigidOrigin = CountNotNulDigits(recognizedStates);
            int numDigidFitted = CountNotNulDigits(fittedStates);

            if (numDigidFitted < numDigidOrigin - 10)
            {
                return (fittedStates, false);
            }

            return (fittedStates, true);
        }

        public static int CountNotNulDigits(List<int> numbers)
        {
            int count = 0;
            foreach (int number in numbers)
            {
                if (number != 0)
                {
                    count++;
                }
            }
            return count;
        }

        /// <summary>
        /// Separates the recognized states into segments.
        /// </summary>
        /// <param name="recognizedStates">The list of recognized states.</param>
        /// <returns>A list of segments.</returns>
        public static List<List<int>> SeparateSegments(List<int> recognizedStates)
        {
            List<List<int>> segments = [];
            List<int> currentSegment = [recognizedStates[0]];

            for (int i = 1; i < recognizedStates.Count; i++)
            {
                if (recognizedStates[i] != recognizedStates[i - 1])
                {
                    segments.Add(currentSegment);
                    currentSegment = [recognizedStates[i]];
                }
                else
                {
                    currentSegment.Add(recognizedStates[i]);
                }
            }

            segments.Add(currentSegment);

            return segments;
        }

        /// <summary>
        /// Outputs the states for debugging purposes.
        /// </summary>
        /// <param name="states">The list of states.</param>
        public static void DebugOutputStates(List<int> states)
        {
            StringBuilder statesLineBuilder = new();
            foreach (int state in states)
            {
                statesLineBuilder.Append(state).Append(' ');
            }
            string statesLine = statesLineBuilder.ToString().Trim();
            Debug.WriteLine(statesLine);
        }
    }
}
