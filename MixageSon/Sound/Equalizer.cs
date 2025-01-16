using NAudio.Dsp;
using NAudio.Wave;

namespace MixageSon.Sound
{
    public class Equalizer(ISampleProvider source) : ISampleProvider
    {
        public WaveFormat WaveFormat => source.WaveFormat;

        public static float Bass { get; set; } = 1f;

        public static float Midrange { get; set; } = 1f;

        public static float Trebble { get; set; } = 1f;

        // Bass band: 20-250 Hz
        private readonly BiQuadFilter _bassFilter = BiQuadFilter.LowPassFilter(sampleRate: source.WaveFormat.SampleRate, cutoffFrequency: 250, q: 0.707f);

        // Midrange band: 250-4000 Hz
        private readonly BiQuadFilter _midrangeFilter = BiQuadFilter.BandPassFilterConstantPeakGain(sampleRate: source.WaveFormat.SampleRate, centreFrequency: 1000, q: 1.0f);

        // Trebble band: 4000-20000 Hz
        private readonly BiQuadFilter _trebbleFilter = BiQuadFilter.HighPassFilter(sampleRate: source.WaveFormat.SampleRate, cutoffFrequency: 4000, q: 0.707f);

        public int Read(float[] buffer, int offset, int count)
        {
            int samplesRead = source.Read(buffer, offset, count);

            for (int i = 0; i < samplesRead; i++)
            {
                float sample = buffer[offset + i];

                float bass = _bassFilter.Transform(sample) * ConvertToLinear(Bass);
                float midrange = _midrangeFilter.Transform(sample) * ConvertToLinear(Midrange);
                float trebble = _trebbleFilter.Transform(sample) * ConvertToLinear(Trebble);

                buffer[offset + i] = bass + midrange + trebble;
            }

            return samplesRead;
        }

        private static float ConvertToLinear(float gainDb)
        {
            return MathF.Pow(10, gainDb / 20);
        }
    }
}
