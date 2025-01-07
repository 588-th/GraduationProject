using Logic.Algorithms;

namespace UnitTests
{
    public class TranscriptionAlignmentTests
    {
        [Fact]
        public void GetStates_NullStateProbabilities_ThrowsArgumentNullException()
        {
            double[][] stateProbabilities = null;
            List<int> targetStates = new List<int> { 1, 2, 3 };

            Assert.Throws<ArgumentNullException>(() => TranscriptionAlignment.GetStates(stateProbabilities, targetStates));
        }

        [Fact]
        public void GetStates_NullTargetStates_ThrowsArgumentNullException()
        {
            double[][] stateProbabilities = new double[][]
            {
                new double[] { 0.1, 0.9 },
                new double[] { 0.8, 0.2 },
                new double[] { 0.3, 0.7 }
            };
            List<int> targetStates = null;

            Assert.Throws<ArgumentNullException>(() => TranscriptionAlignment.GetStates(stateProbabilities, targetStates));
        }

        [Fact]
        public void GetStates_ValidInput_ReturnsAlignedStates()
        {
            double[][] stateProbabilities =
            [
                [0.1, 0.9],
                [0.1, 0.9],
                [0.1, 0.9],
                [0.45, 0.55],
                [0.6, 0.4],
                [0.7, 0.3],
                [0.2, 0.8],
                [0.86, 0.14],
                [0.8, 0.2],
                [0.3, 0.7],
                [0.1, 0.9],
                [0.2, 0.8],
                [0.1, 0.9],
            ];
            List<int> targetStates = new List<int> { 1, 0, 1 };

            List<int>? result = TranscriptionAlignment.GetStates(stateProbabilities, targetStates);

            Assert.NotNull(result);
            Assert.Equal(new List<int> { 1, 1, 1, 1, 0, 0, 0, 0, 0, 1, 1, 1, 1 }, result);
        }

        [Fact]
        public void FindMaxProbabilityState_ReturnsCorrectIndex()
        {
            double[] probabilities = new double[] { 0.1, 0.9, 0.5 };
            int expectedIndex = 1;

            int result = TranscriptionAlignment.FindMaxProbabilityState(probabilities);

            Assert.Equal(expectedIndex, result);
        }

        [Fact]
        public void ReplaceSingleStates_ReplacesCorrectly()
        {
            List<int> recognizedStates = new List<int> { 1, 2, 1, 3, 3, 3, 4, 4, 1, 1, 1 };
            List<int> expectedStates = new List<int> { 1, 1, 1, 3, 3, 3, 4, 4, 1, 1, 1 };

            TranscriptionAlignment.ReplaceSingleStates(recognizedStates);

            Assert.Equal(expectedStates, recognizedStates);
        }

        [Fact]
        public void FitRecognizedStates_FitsCorrectly()
        {
            List<int> recognizedStates = new List<int> { 1, 1, 1, 1, 2, 2, 2, 2, 2, 3, 3, 3, 3,3, 3, 1, 1, 4, 4, 4, 5 };
            List<int> targetStates = new List<int> { 1, 2, 3, 4 };

            (List<int> fittedStates, bool success) = TranscriptionAlignment.FitRecognizedStates(recognizedStates, targetStates);

            Assert.True(success);
            Assert.Equal(new List<int> { 1, 1, 1, 1, 2, 2, 2, 2, 2, 3, 3, 3, 3,3, 3, 4, 4, 4, 4, 4, 4 }, fittedStates);
        }

        [Fact]
        public void SeparateSegments_SeparatesCorrectly()
        {
            List<int> recognizedStates = new List<int> { 1, 1, 2, 2, 2, 3, 3, 4, 4, 4, 4, 5 };
            List<List<int>> expectedSegments = new List<List<int>>
            {
                new List<int> { 1, 1 },
                new List<int> { 2, 2, 2 },
                new List<int> { 3, 3 },
                new List<int> { 4, 4, 4, 4 },
                new List<int> { 5 }
            };

            List<List<int>> result = TranscriptionAlignment.SeparateSegments(recognizedStates);

            Assert.Equal(expectedSegments.Count, result.Count);
            for (int i = 0; i < expectedSegments.Count; i++)
            {
                Assert.Equal(expectedSegments[i], result[i]);
            }
        }
    }
}
