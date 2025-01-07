using Logic.Audio;
using MathNet.Numerics.IntegralTransforms;
using System.Numerics;

namespace Logic.Model
{
    /// <summary>
    /// Computes the Mel-Frequency Cepstral Coefficients (MFCC) for a given audio signal.
    /// </summary>
    public class MelFrequencyCepstralCoefficients
    {
        #region Fields
        private const int MILLISECONDS_IN_SECOND = 1000;

        /// <summary>
        /// Gets or sets the frame duration in milliseconds.
        /// </summary>
        public int FrameDurationMs { get; set; }

        /// <summary>
        /// Gets or sets the frame overlap duration in milliseconds.
        /// </summary>
        public int FrameOverlapDurationMs { get; set; }

        /// <summary>
        /// Gets or sets the number of Mel filters.
        /// </summary>
        public int NumFilters { get; set; }

        /// <summary>
        /// Gets or sets the number of cepstral coefficients.
        /// </summary>
        public int NumCepstralCoefficients { get; set; }
        #endregion

        #region Constructor
        public MelFrequencyCepstralCoefficients(int frameDurationMs, int frameOverlapDurationMs, int numFilters, int numCepstralCoefficients)
        {
            FrameDurationMs = frameDurationMs;
            FrameOverlapDurationMs = frameOverlapDurationMs;
            NumFilters = numFilters;
            NumCepstralCoefficients = numCepstralCoefficients;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Extracts MFCC features from the provided audio information.
        /// </summary>
        /// <param name="audioInformation">The audio information to extract features from.</param>
        /// <returns>A list of MFCC feature vectors.</returns>
        public List<double[]> Extract(AudioInformation audioInformation)
        {
            int sampleRate = audioInformation.SampleRate;
            int frameSize = sampleRate * FrameDurationMs / MILLISECONDS_IN_SECOND;
            int frameOverlap = sampleRate * FrameOverlapDurationMs / MILLISECONDS_IN_SECOND;
            double[] audioContent = audioInformation.Content;

            int numFrames = (audioContent.Length - frameSize) / (frameSize - frameOverlap) + 1;

            List<double[]> frames = SplitAudioSignal(audioContent, frameSize, frameOverlap, numFrames);
            List<double[]> notNullFrames = RemoveNullFrames(frames);
            List<double[]> windowedFrames = ApplyHammingWindows(notNullFrames, frameSize);
            List<Complex[]> fftFrames = CalculateFFT(windowedFrames);
            List<double[]> magnitudeFrames = CalculateMagnitudeSpectrum(fftFrames);
            List<double[]> melFilteredFrames = ApplyMelFilters(magnitudeFrames, NumFilters, frameSize, sampleRate);
            List<double[]> logEnergyFrames = ApplyLogarithmToEnergy(melFilteredFrames);
            List<double[]> cepstralFrames = ApplyDCT(logEnergyFrames, NumCepstralCoefficients);

            ValidateCepstralFrames(cepstralFrames);

            return cepstralFrames;
        }

        private static List<double[]> SplitAudioSignal(double[] audioContent, int frameSize, int frameOverlap, int numFrames)
        {
            List<double[]> frames = new(numFrames);

            for (int i = 0; i < numFrames; i++)
            {
                int frameStart = i * (frameSize - frameOverlap);
                int frameEnd = Math.Min(frameStart + frameSize, audioContent.Length);

                int frameLength = frameEnd - frameStart;
                double[] frame = new double[frameLength];
                Buffer.BlockCopy(audioContent, frameStart * sizeof(double), frame, 0, frameLength * sizeof(double));

                frames.Add(frame);
            }

            return frames;
        }

        private static List<double[]> RemoveNullFrames(List<double[]> frames)
        {
            return frames.Where(frame => !double.IsNaN(frame.Sum()) && frame.Sum() != 0).ToList();
        }

        private static List<double[]> ApplyHammingWindows(List<double[]> frames, int frameSize)
        {
            double HammingAlpha = 0.53836;
            double HammingBeta = 0.46164;

            List<double[]> windowedFrames = new(frames.Count);

            foreach (double[] frame in frames)
            {
                double[] windowedFrame = new double[frameSize];

                for (int i = 0; i < frameSize; i++)
                {
                    double hammingValue = HammingAlpha - HammingBeta * Math.Cos(2 * Math.PI * i / (frameSize - 1));
                    windowedFrame[i] = frame[i] * hammingValue;
                }

                windowedFrames.Add(windowedFrame);
            }

            return windowedFrames;
        }

        private static List<Complex[]> CalculateFFT(List<double[]> frames)
        {
            List<Complex[]> fftFrames = new(frames.Count);

            foreach (double[] frame in frames)
            {
                Complex[] complexFrame = Array.ConvertAll(frame, value => new Complex(value, 0));
                Fourier.Forward(complexFrame, FourierOptions.Default);
                fftFrames.Add(complexFrame);
            }

            return fftFrames;
        }

        private static List<double[]> CalculateMagnitudeSpectrum(List<Complex[]> frames)
        {
            List<double[]> magnitudeFrames = new(frames.Count);

            foreach (Complex[] frame in frames)
            {
                double[] magnitudeFrame = new double[frame.Length];

                for (int i = 0; i < frame.Length; i++)
                {
                    magnitudeFrame[i] = frame[i].Magnitude;
                }

                magnitudeFrames.Add(magnitudeFrame);
            }

            return magnitudeFrames;
        }

        private static List<double[]> ApplyMelFilters(List<double[]> frames, int numFilters, int frameSize, int sampleRate)
        {
            List<double[]> melFilteredFrames = new(frames.Count);
            int numFilterPlus = numFilters + 2;
            double minMel = HertzToMel(300);
            double maxMel = HertzToMel(8000);
            double deltaMel = (maxMel - minMel) / (numFilterPlus - 1);
            double[] hzPoints = Enumerable.Range(0, numFilterPlus)
                .Select(i => MelToHertz(minMel + i * deltaMel))
                .ToArray();
            int[] f = hzPoints.Select(hz => (int)Math.Floor((frameSize + 1) * hz / sampleRate)).ToArray();

            double[][] melFilters = new double[numFilters][];
            for (int i = 0; i < numFilters; i++)
            {
                double[] melFilter = new double[frameSize / 2 + 1];
                int m = i + 1;

                for (int j = 0; j < melFilter.Length; j++)
                {
                    if (j < f[m - 1])
                        melFilter[j] = 0;
                    else if (j >= f[m - 1] && j <= f[m + 1])
                        melFilter[j] = (double)(j - f[m - 1]) / (f[m] - f[m - 1]);
                    else if (j >= f[m] && j <= f[m + 1])
                        melFilter[j] = (double)(f[m + 1] - j) / (f[m + 1] - f[m]);
                    else
                        melFilter[j] = 0;
                }

                melFilters[i] = melFilter;
            }

            foreach (double[] magnitudeFrame in frames)
            {
                double[] melFilteredFrame = new double[numFilters];

                for (int j = 0; j < numFilters; j++)
                {
                    double[] melFilter = melFilters[j];
                    double filterEnergy = melFilter.Select((value, index) => value * magnitudeFrame[index]).Sum();
                    melFilteredFrame[j] = filterEnergy;
                }

                melFilteredFrames.Add(melFilteredFrame);
            }

            return melFilteredFrames;
        }

        private static double HertzToMel(double hertz)
        {
            return 2595 * Math.Log10(1 + hertz / 700);
        }

        private static double MelToHertz(double mel)
        {
            return 700 * (Math.Pow(10, mel / 2595) - 1);
        }

        private static List<double[]> ApplyLogarithmToEnergy(List<double[]> frames)
        {
            List<double[]> logEnergyFrames = new(frames.Count);

            foreach (double[] melFilteredFrame in frames)
            {
                double[] logEnergyFrame = new double[melFilteredFrame.Length];

                for (int i = 0; i < melFilteredFrame.Length; i++)
                {
                    logEnergyFrame[i] = Math.Log(melFilteredFrame[i]);
                }

                logEnergyFrames.Add(logEnergyFrame);
            }

            return logEnergyFrames;
        }

        private static List<double[]> ApplyDCT(List<double[]> frames, int numCepstralCoefficients)
        {
            List<double[]> dctCoefficients = [];

            foreach (double[] frame in frames)
            {
                double[] coefficients = new double[numCepstralCoefficients];
                int frameLength = frame.Length;

                for (int cepstralIndex = 0; cepstralIndex < numCepstralCoefficients; cepstralIndex++)
                {
                    double sum = 0.0;
                    double coefficient = Math.Sqrt(2.0 / frameLength);

                    for (int sampleIndex = 0; sampleIndex < frameLength; sampleIndex++)
                    {
                        double angle = Math.PI * cepstralIndex * (2 * sampleIndex + 1) / (2.0 * frameLength);
                        sum += frame[sampleIndex] * Math.Cos(angle);
                    }

                    if (cepstralIndex == 0)
                    {
                        coefficient /= Math.Sqrt(2.0);
                    }

                    coefficients[cepstralIndex] = coefficient * sum;
                }

                dctCoefficients.Add(coefficients);
            }

            return dctCoefficients;
        }

        public static List<double[]> CalculateFirstDerivative(List<double[]> frames)
        {
            List<double[]> derivativeFrames = [];

            foreach (double[] frame in frames)
            {
                double[] derivativeFrame = new double[frame.Length];

                for (int i = 1; i < frame.Length - 1; i++)
                {
                    derivativeFrame[i] = (frame[i + 1] - frame[i - 1]) / 2.0;
                }

                derivativeFrame[0] = frame[1] - frame[0];
                derivativeFrame[frame.Length - 1] = frame[frame.Length - 1] - frame[frame.Length - 2];

                derivativeFrames.Add(derivativeFrame);
            }

            return derivativeFrames;
        }

        public static List<double[]> CalculateSecondDerivative(List<double[]> frames)
        {
            List<double[]> derivativeFrames = [];

            foreach (double[] frame in frames)
            {
                double[] derivativeFrame = new double[frame.Length];

                for (int i = 1; i < frame.Length - 1; i++)
                {
                    derivativeFrame[i] = frame[i + 1] - 2 * frame[i] + frame[i - 1];
                }

                derivativeFrame[0] = frame[1] - 2 * frame[0] + frame[0];
                derivativeFrame[frame.Length - 1] = frame[frame.Length - 1] - 2 * frame[frame.Length - 2] + frame[frame.Length - 3];

                derivativeFrames.Add(derivativeFrame);
            }

            return derivativeFrames;
        }

        private static void ValidateCepstralFrames(List<double[]> cepstralFrames)
        {
            foreach (var array in cepstralFrames)
            {
                foreach (var item in array)
                {
                    if (double.IsInfinity(item) || double.IsNaN(item))
                    {
                        throw new Exception("Infinity or NAN");
                    }
                }
            }
        }
        #endregion
    }
}