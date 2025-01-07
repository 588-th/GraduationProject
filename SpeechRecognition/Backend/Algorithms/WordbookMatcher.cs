namespace Logic.Algorithms
{
    /// <summary>
    /// Provides methods for finding the most similar word from a wordbook based on Levenshtein distance.
    /// </summary>
    public static class WordbookMatcher
    {
        /// <summary>
        /// Converts a list of recognized states to their corresponding letters.
        /// </summary>
        /// <param name="recognizedStates">The list of recognized states.</param>
        /// <returns>A string representation of the recognized states.</returns>
        public static string ConvertingStatesToLetters(Dictionary<int, string> statebook, List<int> recognizedStates)
        {
            return string.Join("", recognizedStates.ConvertAll(state => statebook[state]));
        }

        /// <summary>
        /// Finds the most similar word from the wordbook to the given word.
        /// </summary>
        /// <param name="wordbook">The list of words to search through.</param>
        /// <param name="word">The word to find similarity for.</param>
        /// <returns>The most similar word from the wordbook.</returns>
        /// <exception cref="ArgumentNullException">Thrown when wordbook is null or empty, or when word is null or empty.</exception>
        public static string? FindMostSimilarWord(List<string> wordbook, string word)
        {
            if (wordbook == null || wordbook.Count == 0)
            {
                throw new ArgumentNullException(nameof(wordbook), "Wordbook cannot be null or empty.");
            }

            if (string.IsNullOrEmpty(word))
            {
                throw new ArgumentNullException(nameof(word), "Word cannot be null or empty.");
            }

            string mostSimilarWord = wordbook[0];
            double maxDistance = 0;

            foreach (string dictionaryWord in wordbook)
            {
                double distance = CalculateSimilarity(word, dictionaryWord);

                if (distance > maxDistance)
                {
                    maxDistance = distance;
                    mostSimilarWord = dictionaryWord;
                }
            }

            return mostSimilarWord;
        }

        /// <summary>
        /// Calculates the similarity between two words using Levenshtein distance.
        /// </summary>
        /// <param name="word1">The first word.</param>
        /// <param name="word2">The second word.</param>
        /// <returns>The similarity between the two words.</returns>
        /// <exception cref="ArgumentException">Thrown when either word1 or word2 is null or empty.</exception>
        public static double CalculateSimilarity(string word1, string word2)
        {
            if (string.IsNullOrEmpty(word1) || string.IsNullOrEmpty(word2))
            {
                throw new ArgumentException("Words cannot be null or empty.");
            }

            int distance = CalculateDistance(word1, word2);
            int maxLength = Math.Max(word1.Length, word2.Length);
            return 1.0 - (double)distance / maxLength;
        }

        /// <summary>
        /// Calculates the Levenshtein distance between two words.
        /// </summary>
        /// <param name="word1">The first word.</param>
        /// <param name="word2">The second word.</param>
        /// <returns>The Levenshtein distance between the two words.</returns>
        /// <exception cref="ArgumentException">Thrown when either word1 or word2 is null or empty.</exception>
        public static int CalculateDistance(string word1, string word2)
        {
            if (string.IsNullOrEmpty(word1) || string.IsNullOrEmpty(word2))
            {
                throw new ArgumentException("Words cannot be null or empty.");
            }

            int[,] distance = new int[word1.Length + 1, word2.Length + 1];

            for (int i = 0; i <= word1.Length; i++)
                distance[i, 0] = i;

            for (int j = 0; j <= word2.Length; j++)
                distance[0, j] = j;

            for (int i = 1; i <= word1.Length; i++)
            {
                for (int j = 1; j <= word2.Length; j++)
                {
                    int cost = (word1[i - 1] == word2[j - 1]) ? 0 : 1;

                    distance[i, j] = Math.Min(
                        Math.Min(distance[i - 1, j] + 1, distance[i, j - 1] + 1),
                        distance[i - 1, j - 1] + cost
                    );
                }
            }

            return distance[word1.Length, word2.Length];
        }
    }
}