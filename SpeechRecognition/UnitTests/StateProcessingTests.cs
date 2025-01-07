using Logic.Algorithms;
namespace UnitTests
{
    public class StateProcessingTests
    {
        [Fact]
        public void GetWordStates_NullInput_ThrowsArgumentNullException()
        {
            double[][] stateProbabilities = null;
            Assert.Throws<ArgumentNullException>(() => StateProcessing.GetWordStates(stateProbabilities));
        }

        [Fact]
        public void GetWordStates_ValidInput_ReturnsExpectedStates()
        {
            double[][] stateProbabilities = new double[][]
            {
                new double[] { 0.1, 0.9 },
                new double[] { 0.8, 0.2 },
                new double[] { 0.7, 0.3 },
                new double[] { 0.3, 0.7 },
                new double[] { 0.4, 0.6 }
            };

            List<int> expectedStates = new List<int> { 1 };
            List<int> result = StateProcessing.GetWordStates(stateProbabilities);

            Assert.Equal(expectedStates, result);
        }

        [Fact]
        public void GetSentenceStates_NullInput_ThrowsArgumentNullException()
        {
            double[][] stateProbabilities = null;
            Assert.Throws<ArgumentNullException>(() => StateProcessing.GetSentenceStates(stateProbabilities));
        }

        [Fact]
        public void GetSentenceStates_ValidInput_ReturnsExpectedStates()
        {
            double[][] stateProbabilities = new double[][]
            {
                new double[] { 0.1, 0.9 },
                new double[] { 0.8, 0.2 },
                new double[] { 0.7, 0.3 },
                new double[] { 0.3, 0.7 },
                new double[] { 0.4, 0.6 },
                new double[] { 0.9, 0.1 },
                new double[] { 0.6, 0.4 },
                new double[] { 0.0, 1.0 }
            };

            List<List<int>> expectedStates = new List<List<int>> {
                new List<int> { 1, 0, 1, 0}
            };

            List<List<int>> result = StateProcessing.GetSentenceStates(stateProbabilities);

            Assert.Equal(expectedStates, result);
        }

        [Fact]
        public void ReplaceSingleStates_ReplacesCorrectly()
        {
            List<int> recognizedStates = new List<int> { 1, 2, 1, 3, 3, 3, 4, 4, 1, 1, 1 };
            List<int> expectedStates = new List<int> { 1, 1, 1, 3, 3, 3, 4, 4, 1, 1, 1 };

            StateProcessing.ReplaceSingleStates(recognizedStates);

            Assert.Equal(expectedStates, recognizedStates);
        }

        [Fact]
        public void RemoveSingleStates_RemovesCorrectly()
        {
            List<int> recognizedStates = new List<int> { 1, 2, 3, 3, 2, 4, 5, 5, 6 };
            List<int> expectedStates = new List<int> { 1, 3, 3, 4, 5, 5, 6 };

            StateProcessing.RemoveSingleStates(recognizedStates);

            Assert.Equal(expectedStates, recognizedStates);
        }

        [Fact]
        public void RemoveBlankStates_RemovesCorrectly()
        {
            List<int> recognizedStates = new List<int> { 0, 1, 0, 2, 0, 3, 4, 0 };
            List<int> expectedStates = new List<int> { 1, 2, 3, 4 };

            StateProcessing.RemoveBlankStates(recognizedStates);

            Assert.Equal(expectedStates, recognizedStates);
        }

        [Fact]
        public void CombineStates_CombinesCorrectly()
        {
            List<int> recognizedStates = new List<int> { 1, 1, 2, 2, 2, 3, 3, 4, 4, 4, 4, 5 };
            List<int> expectedStates = new List<int> { 1, 2, 3, 4, 5 };

            StateProcessing.CombineStates(recognizedStates);

            Assert.Equal(expectedStates, recognizedStates);
        }
    }
}