using System;
using System.Collections.Generic;
using System.Text;

namespace GeneticPainterLib
{
    [Serializable]
    internal class Chromosome
    {
        internal static int MAXIMUM_POINTS = 3;
        internal static int MAXIMUM_X = 800;
        internal static int MAXIMUM_Y = 600;

        internal System.Drawing.Color Color;

        internal System.Drawing.Point[] Points
        { 
            get { return _points; } 
        }
        
        private System.Drawing.Point[] _points;

        internal Chromosome()
        {
            _points = new System.Drawing.Point[MAXIMUM_POINTS];
            System.Drawing.Point random_center = GetRandomPoint();
            for (int i = 0; i < MAXIMUM_POINTS; i++)
            {
                _points[i].X = random_center.X + RandomNumber.GetRandom(-50,51);//Convert.ToInt32(Math.Floor(((RandomNumber.GetRandom(0, 100) - 50) / 100.0)));
                _points[i].Y = random_center.Y + RandomNumber.GetRandom(-50, 51);//Convert.ToInt32(Math.Floor(((RandomNumber.GetRandom(0, 100) - 50) / 100.0)));
            }

            Color = System.Drawing.Color.FromArgb(RandomNumber.GetRandom(0,256), RandomNumber.GetRandom(0, 256), RandomNumber.GetRandom(0, 256), RandomNumber.GetRandom(0, 256));
        }

        internal Chromosome(System.Drawing.Point[] points)
        {
            if (points.Length == MAXIMUM_POINTS)
            {
                _points = points;
            }
            else throw new ArgumentException("Points must be of equal length.", "points");
            Color = System.Drawing.Color.FromArgb(RandomNumber.GetRandom(51,250), RandomNumber.GetRandom(0, 256), RandomNumber.GetRandom(0, 256), RandomNumber.GetRandom(0, 256));
        }

        internal Chromosome(System.Drawing.Color color)
        {
            _points = new System.Drawing.Point[MAXIMUM_POINTS];
            
            for (int i = 0; i < MAXIMUM_POINTS; i++)
            {
                _points[i] = GetRandomPoint();
            }
            Color = color;
        }

        internal Chromosome(System.Drawing.Color color, System.Drawing.Point[] points)
        {
            _points = points;
            Color = color;
        }


        internal bool ContainsPoint(int x, int y)
        {
            int i, j = 0;
            bool c = false;
            for (i = 0, j = MAXIMUM_POINTS - 1; i < MAXIMUM_POINTS; j = i++)
            {
                if (((_points[i].Y > y) != (_points[j].Y > y)) &&
                    (x < (_points[j].X - _points[i].X) * (y - _points[i].Y) / (_points[j].Y - _points[i].Y) + _points[i].X))
                {
                    c = !c;
                }
            }
            return c;
        }

        internal static System.Drawing.Point GetRandomPoint()
        {
            return new System.Drawing.Point(RandomNumber.GetRandom(0, MAXIMUM_X), RandomNumber.GetRandom(0, MAXIMUM_Y));
        }
    }
}
