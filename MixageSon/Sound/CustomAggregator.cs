using FftSharp;
using FftSharp.Windows;
using NAudio.Wave;
using System.Numerics;

namespace MixageSon.Sound
{
    public class CustomAggregator(ISampleProvider source, int fftSize) : ISampleProvider
    {
        private int fftPos = 0;
        private readonly double[] _buffer = new double[fftSize];

        public WaveFormat WaveFormat => source.WaveFormat;

        public event EventHandler<FftDoneEventArgs>? FftDone;

        private void Add(float n)
        {
            _buffer[fftPos] = n;
            fftPos++;

            if (fftPos >= fftSize)
            {
                Window w = new Hanning();
                w.ApplyInPlace(_buffer);
                Complex[] complexes = FFT.Forward(_buffer);
                FftDone?.Invoke(this, new() { Results = complexes });
                fftPos = 0;
            }
        }

        public int Read(float[] buffer, int offset, int count)
        {
            int read = source.Read(buffer, offset, count);
            for (int i = 0; i < count; i++)
            {
                Add(buffer[offset + i]);
            }

            return read;
        }
    }

    public class FftDoneEventArgs : EventArgs {
        public required Complex[] Results { get; set; }

        public double[] ComputeMagnitudes()
        {
            return FFT.Magnitude(Results);
        }
    }
}
