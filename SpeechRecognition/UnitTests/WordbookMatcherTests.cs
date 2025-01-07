using Logic.Algorithms;

namespace UnitTests
{
    public class WordbookMatcherTests
    {
        [Fact]
        public void ConvertingStatesToLetters_ValidInput_ReturnsCorrectString()
        {
            var statebook = new Dictionary<int, string>
            {
                { 0, "a" },
                { 1, "b" },
                { 2, "c" }
            };
            var recognizedStates = new List<int> { 0, 1, 2 };

            string result = WordbookMatcher.ConvertingStatesToLetters(statebook, recognizedStates);

            Assert.Equal("abc", result);
        }

        [Fact]
        public void ConvertingStatesToLetters_EmptyRecognizedStates_ReturnsEmptyString()
        {
            var statebook = new Dictionary<int, string>
            {
                { 0, "a" },
                { 1, "b" },
                { 2, "c" }
            };
            var recognizedStates = new List<int>();

            string result = WordbookMatcher.ConvertingStatesToLetters(statebook, recognizedStates);

            Assert.Equal(string.Empty, result);
        }

        [Fact]
        public void FindMostSimilarWord_NullWordbook_ThrowsArgumentNullException()
        {
            List<string> wordbook = null;
            string word = "test";

            Assert.Throws<ArgumentNullException>(() => WordbookMatcher.FindMostSimilarWord(wordbook, word));
        }

        [Fact]
        public void FindMostSimilarWord_EmptyWordbook_ThrowsArgumentNullException()
        {
            var wordbook = new List<string>();
            string word = "test";

            Assert.Throws<ArgumentNullException>(() => WordbookMatcher.FindMostSimilarWord(wordbook, word));
        }

        [Fact]
        public void FindMostSimilarWord_NullWord_ThrowsArgumentNullException()
        {
            var wordbook = new List<string> { "test" };
            string word = null;

            Assert.Throws<ArgumentNullException>(() => WordbookMatcher.FindMostSimilarWord(wordbook, word));
        }

        [Fact]
        public void FindMostSimilarWord_ValidInput_ReturnsMostSimilarWord()
        {
            var wordbook = new List<string> { "tost", "beast", "test" };
            string word = "rest";

            string result = WordbookMatcher.FindMostSimilarWord(wordbook, word);

            Assert.Equal("test", result);
        }

        [Fact]
        public void CalculateSimilarity_ValidInput_ReturnsCorrectSimilarity()
        {
            string word1 = "test";
            string word2 = "best";

            double result = WordbookMatcher.CalculateSimilarity(word1, word2);

            Assert.Equal(0.75, result, 2);
        }

        [Fact]
        public void CalculateDistance_ValidInput_ReturnsCorrectDistance()
        {
            string word1 = "kitten";
            string word2 = "sitting";

            int result = WordbookMatcher.CalculateDistance(word1, word2);

            Assert.Equal(3, result);
        }

        [Fact]
        public void CalculateDistance_NullOrEmptyWords_ThrowsArgumentException()
        {
            string word1 = "kitten";
            string word2 = null;

            Assert.Throws<ArgumentException>(() => WordbookMatcher.CalculateDistance(word1, word2));
        }
    }
}
