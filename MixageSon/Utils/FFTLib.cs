using NAudio.Dsp;

namespace MixageSon.Utils
{
    public static class FFTLib
    {
        public static void PerformFFT(float[] inBuffer, double[] outBuffer)
        {
            Complex[] fftBuffer = new Complex[inBuffer.Length];
            for (int i = 0; i < fftBuffer.Length; i++)
            {
                fftBuffer[i] = new() { X = inBuffer[i], Y = 0 };
            }

            FastFourierTransform.FFT(true, (int)Math.Log(inBuffer.Length, 2), fftBuffer);

            double[] magnitudes = new double[inBuffer.Length];
            for (int i = 0; i < fftBuffer.Length; i++)
            {
                double magnitude = Math.Sqrt(fftBuffer[i].X * fftBuffer[i].X + fftBuffer[i].Y * fftBuffer[i].Y);
                outBuffer[i] = magnitude;
            }
        }
    }
}
