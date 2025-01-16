using NAudio.Wave;
using Newtonsoft.Json.Linq;
using System.Drawing;

namespace MixageSon.Rendering
{
    public static class SoundColors
    {
        public static Color FromHSV(float hue, float saturation, float value)
        {
            if (hue < 0 || hue > 360)
                throw new ArgumentOutOfRangeException(nameof(hue), "Hue must be in the range [0, 360].");
            if (saturation < 0 || saturation > 1)
                throw new ArgumentOutOfRangeException(nameof(saturation), "Saturation must be in the range [0, 1].");
            if (value < 0 || value > 1)
                throw new ArgumentOutOfRangeException(nameof(value), "Value must be in the range [0, 1].");

            double c = value * saturation;
            double x = c * (1 - Math.Abs((hue / 60.0) % 2 - 1));
            double m = value - c;

            double r = 0, g = 0, b = 0;

            if (hue >= 0 && hue < 60)
            {
                r = c; g = x; b = 0;
            }
            else if (hue >= 60 && hue < 120)
            {
                r = x; g = c; b = 0;
            }
            else if (hue >= 120 && hue < 180)
            {
                r = 0; g = c; b = x;
            }
            else if (hue >= 180 && hue < 240)
            {
                r = 0; g = x; b = c;
            }
            else if (hue >= 240 && hue < 300)
            {
                r = x; g = 0; b = c;
            }
            else if (hue >= 300 && hue <= 360)
            {
                r = c; g = 0; b = x;
            }

            int red = (int)((r + m) * 255);
            int green = (int)((g + m) * 255);
            int blue = (int)((b + m) * 255);

            return Color.FromArgb(red, green, blue);
        }
    }
}
