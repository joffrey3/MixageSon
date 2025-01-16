using NAudio.Wave;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace MixageSon.Rendering
{
    public class Visualizer
    {
        private readonly Canvas _canvas;
        Collection<IRenderer> Renderers { get; } = [];

        protected const int FPS = 60;

        public Visualizer(Canvas canvas)
        {
            _canvas = canvas;

            DispatcherTimer timer = new()
            {
                Interval = TimeSpan.FromSeconds(1.0 / FPS)
            };

            timer.Tick += Update;
            timer.Start();
        }

        protected void Update(object? sender, EventArgs arguments)
        {
            _canvas.Children.Clear();
            _canvas.Children.Add(new Rectangle()
            {
                Width = _canvas.ActualWidth,
                Height = _canvas.ActualHeight,
                Fill = Brushes.Black,
                Stroke = Brushes.Black,
            });

            foreach (IRenderer renderer in Renderers)
            {
                renderer.Render(_canvas);
            }
        }
    }
}
