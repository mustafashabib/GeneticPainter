using System;
using System.Collections.Generic;
using System.Text;

namespace GeneticPainterLib
{
    internal static class DrawingHelper
    {
        internal static System.Drawing.Bitmap ResizeImage(System.Drawing.Bitmap original, int width, int height)
        {

            int sourceWidth = original.Width;
            int sourceHeight = original.Height;

            float percent = 0;
            float percentW = 0;
            float percentH = 0;
            percentW = ((float)width/ (float)sourceWidth);
            percentH = ((float)height/ (float)sourceHeight);

            if (percentH < percentW)
                percent = percentH;
            else
                percent = percentW;

            int destWidth = (int)(sourceWidth * percent);
            int destHeight = (int)(sourceHeight * percent);

            System.Drawing.Bitmap b = new System.Drawing.Bitmap(destWidth, destHeight);
            System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(b);
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            g.DrawImage(original, 0, 0, destWidth, destHeight);
            g.Dispose();
            return b;
        }

        internal static System.Drawing.Color BlendColors(System.Drawing.Color[] colors)
        {
            //rgba blending formula:
            //(1-as)rs + asrd = rf
            //(1-as)gs + asgd = gf
            //(1-as)bs + asbd = bf
            //(1-as)as + asad = af
            if (colors == null || colors.Length == 0)
            {
                throw new ArgumentNullException("colors", "Cannot be null or empty.");
            }

            System.Drawing.Color destination = colors[0];
            for (int i = 1; i < colors.Length; i++)
            {
                System.Drawing.Color source = colors[i];
                int one_minus_source_a = 255 - source.A;
                int rf = (one_minus_source_a * source.R) + (source.A * destination.R);
                int gf = (one_minus_source_a * source.G) + (source.A * destination.G);
                int bf = (one_minus_source_a * source.B) + (source.A * destination.B);
                int af = (one_minus_source_a * source.A) + (source.A * destination.A);
                destination = System.Drawing.Color.FromArgb(af/255, rf/255, gf/255, bf/255);
            }
            return destination;


        }
    }
}
