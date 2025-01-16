using MixageSon.Sound;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace MixageSon.Rendering
{
    public class FFTCanvas
    {
        private const float FPS = 160f;
        private readonly Canvas _canvas;
        private readonly CustomAggregator _source;
        private readonly int _fftSize;
        private double[] _buffer;
        private readonly Rectangle[] _rectangles;

        public FFTCanvas(CustomAggregator source, Canvas canvas, int fftSize)
        {
            _source = source;
            _canvas = canvas;

            _fftSize = fftSize;
            _buffer = new double[_fftSize];
            _rectangles = new Rectangle[fftSize];


            for (int i = 0; i < _fftSize; i++)
            {
                double mt = _canvas.Height / 4;
                double ml = _canvas.Width / _fftSize * i;

                _rectangles[i] = new()
                {
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Top,
                    Height = 0,
                    Width = 0,
                    Fill = new RadialGradientBrush(System.Windows.Media.Colors.White, System.Windows.Media.Colors.Transparent)
                };

                _canvas.Children.Add(_rectangles[i]);
            }

            _canvas.UpdateLayout();

            _source.FftDone += Tick;

            DispatcherTimer timer = new()
            {
                Interval = TimeSpan.FromSeconds(1 / FPS)
            };

            timer.Tick += VisualTick;
            timer.Start();
        }

        private void VisualTick(object? sender, EventArgs e)
        {
            for (int i = 0; i < 128; i++)
            {
                double mag = _buffer[i];

                _rectangles[i].Width = _canvas.ActualWidth / 128;
                _rectangles[i].Height = Lerp(_rectangles[i].Height, _canvas.ActualHeight * mag * 20, 0.1);

                Thickness margin = _rectangles[i].Margin;
                margin.Left = _canvas.ActualWidth / 128 * i;
                margin.Top = (_canvas.ActualHeight - _rectangles[i].Height) / 2;
                _rectangles[i].Margin = margin;
            }

            _canvas.UpdateLayout();
        }

        protected void Tick(object? sender, FftDoneEventArgs e)
        {
            _buffer = e.ComputeMagnitudes();
        }

        private static double Lerp(double a, double b, double t)
        {
            return a + (b - a) * t;
        }
    }
}
