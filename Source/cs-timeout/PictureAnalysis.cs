using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cs_timed_silver
{
    public static class PictureAnalysis
    {
        public static List<Color> TenMostUsedColors { get; private set; }
        public static List<double> TenMostUsedColorIncidences { get; private set; }

        public static Color MostUsedColor { get; private set; }
        public static double MostUsedColorIncidence { get; private set; }

        private static int pixelColor;

        private static Dictionary<int, double> dctColorIncidence;

        public static void GetMostUsedColor(Bitmap theBitMap)
        {
            TenMostUsedColors = new List<Color>();
            TenMostUsedColorIncidences = new List<double>();

            MostUsedColor = Color.Empty;
            MostUsedColorIncidence = 0;

            // does using Dictionary<int,int> here
            // really pay-off compared to using
            // Dictionary<Color, int> ?

            // would using a SortedDictionary be much slower, or ?

            dctColorIncidence = new Dictionary<int, double>();

            // this is what you want to speed up with unmanaged code
            for (int row = 0; row < theBitMap.Size.Width; row++)
            {
                for (int col = 0; col < theBitMap.Size.Height; col++)
                {
                    Color c = theBitMap.GetPixel(row, col);
                    pixelColor = c.ToArgb();

                    if (dctColorIncidence.Keys.Contains(pixelColor))
                    {
                        dctColorIncidence[pixelColor] += c.A / 255d;
                    }
                    else
                    {
                        dctColorIncidence.Add(pixelColor, c.A / 255d);
                    }
                }
            }

            // note that there are those who argue that a
            // .NET Generic Dictionary is never guaranteed
            // to be sorted by methods like this
            var dctSortedByValueHighToLow = dctColorIncidence.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value);

            // this should be replaced with some elegant Linq ?
            foreach (KeyValuePair<int, double> kvp in dctSortedByValueHighToLow.Take(10))
            {
                TenMostUsedColors.Add(Color.FromArgb(kvp.Key));
                TenMostUsedColorIncidences.Add(kvp.Value);
            }

            MostUsedColor = Color.FromArgb(dctSortedByValueHighToLow.First().Key);
            MostUsedColorIncidence = dctSortedByValueHighToLow.First().Value;
        }
    }
}
