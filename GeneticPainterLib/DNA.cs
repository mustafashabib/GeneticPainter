using System;
using System.Collections.Generic;
using System.Text;

namespace GeneticPainterLib
{


    [Serializable]
    internal class DNA
    {
        

        internal static int MAXIMUM_CHROMOSOMES = 250;
        internal static int MUTATION_CHANCE = 2;
        internal static float MUTATION_AMOUNT = .1f;
        public static bool UNIFORM_CROSSOVER = true;
        private Chromosome[] _chromosomes;
        public Chromosome[] Chromosomes { get { return _chromosomes; }}

        internal DNA()
        {
            _chromosomes = new Chromosome[MAXIMUM_CHROMOSOMES];
            for (int i = 0; i < MAXIMUM_CHROMOSOMES; i++)
            {
                _chromosomes[i] = new Chromosome();
            }
        }

        internal DNA(DNA parent)
        {
            if (parent != null && parent.Chromosomes.Length != MAXIMUM_CHROMOSOMES)
                throw new ArgumentException("Parent must have the same number of Chromosomes as this DNA.");

            _chromosomes = new Chromosome[MAXIMUM_CHROMOSOMES];
            for (int i = 0; i < MAXIMUM_CHROMOSOMES; i++)
            {
                if (i % 2 == 0)
                    _chromosomes[i] = new Chromosome();
                else
                    _chromosomes[i] = parent.Chromosomes[i];
            }
        }

        internal DNA(DNA parent1, DNA parent2)
        {
            if (parent1 != null && parent1.Chromosomes.Length != MAXIMUM_CHROMOSOMES)
                throw new ArgumentException("Parent 1 must have the same number of Chromosomes as this DNA.");

            if (parent2 != null && parent2.Chromosomes.Length != MAXIMUM_CHROMOSOMES)
                throw new ArgumentException("Parent 2 must have the same number of Chromosomes as this DNA.");

            _chromosomes = new Chromosome[MAXIMUM_CHROMOSOMES];
            
            int split_on = RandomNumber.GetRandom(0, MAXIMUM_CHROMOSOMES);
            
            int which_first = RandomNumber.GetRandom(0, 2);
            DNA parent = null;

            for (int i = 0; i < MAXIMUM_CHROMOSOMES; i++)
            {
                if (UNIFORM_CROSSOVER)
                {
                   
                        if (RandomNumber.GetRandom(0, 2) < 1)
                        {
                            parent = parent1;
                        }
                        else
                        {
                            parent = parent2;
                        }
                }
                else
                {
                        if (i < split_on)
                        {
                            parent = parent1;
                        }
                        else
                        {
                            parent = parent2;
                        }
                }

                if (RandomNumber.GetRandom(0, 100) <= MUTATION_CHANCE)
                {
                    _chromosomes[i] = MutateChromosome(parent.Chromosomes[i]);
                }
                else
                {
                    _chromosomes[i] = parent.Chromosomes[i];
                }
            }
        }

        internal Chromosome MutateChromosome(Chromosome c)
        {
            System.Drawing.Point[] points = new System.Drawing.Point[Chromosome.MAXIMUM_POINTS];
            c.Points.CopyTo(points,0);
            System.Drawing.Color color = System.Drawing.Color.FromArgb(c.Color.A, c.Color.R, c.Color.G, c.Color.B);
            

            int random_point = RandomNumber.GetRandom(0, Chromosome.MAXIMUM_POINTS);
            int new_value = 0;
           
            switch (RandomNumber.GetRandom(0, 6))
            {
                case 0:
                    //new_value = color.A + (Convert.ToInt32(Math.Floor(color.A * MUTATION_AMOUNT)) * mutation_direction);
                    new_value = color.A + Convert.ToInt32((255 * (RandomNumber.GetRandom(0, 100) / 100.0 * MUTATION_AMOUNT * 2) - MUTATION_AMOUNT));
                    if (new_value > 255)
                    {
                        new_value = 255;
                    }
                    if (new_value < 0)
                    {
                        new_value = 0;
                    }
                    color = System.Drawing.Color.FromArgb(new_value, color.R, color.G, color.B);
                    break;
                case 1:
                    new_value = color.R + Convert.ToInt32(255 * ((RandomNumber.GetRandom(0, 100) / 100.0 * MUTATION_AMOUNT * 2) - MUTATION_AMOUNT));
                    if (new_value > 255)
                    {
                        new_value = 255;
                    }
                    if (new_value < 0)
                    {
                        new_value = 0;
                    }
                    color = System.Drawing.Color.FromArgb(color.A, new_value, color.G, color.B);
                    break;
                case 2:
                    new_value = color.G + Convert.ToInt32(255 *((RandomNumber.GetRandom(0, 100) / 100.0 * MUTATION_AMOUNT * 2) - MUTATION_AMOUNT));
                    if (new_value > 255)
                    {
                        new_value = 255;
                    }
                    if (new_value < 0)
                    {
                        new_value = 0;
                    }
                    color = System.Drawing.Color.FromArgb(color.A, color.R, new_value, color.B);
                    break;
                case 3:
                    new_value = color.B + Convert.ToInt32(255 * ((RandomNumber.GetRandom(0, 100) / 100.0 * MUTATION_AMOUNT * 2) - MUTATION_AMOUNT));
                    if (new_value > 255)
                    {
                        new_value = 255;
                    }
                    if (new_value < 0)
                    {
                        new_value = 0;
                    }
                    color = System.Drawing.Color.FromArgb(color.A, color.R, color.G,new_value);
                    break;
                case 4:
                    new_value = points[random_point].X + Convert.ToInt32(Chromosome.MAXIMUM_X * ((RandomNumber.GetRandom(0, 100) / 100.0 * MUTATION_AMOUNT * 2) - MUTATION_AMOUNT));
                    
                    if (new_value > Chromosome.MAXIMUM_X)
                    {
                        new_value = Chromosome.MAXIMUM_X;
                    }
                    if (new_value < 0)
                    {
                        new_value = 0;
                    }
                    points[random_point].X = new_value;
                    break;
                case 5:
                    new_value = points[random_point].Y + Convert.ToInt32(Chromosome.MAXIMUM_Y * ((RandomNumber.GetRandom(0, 100) / 100.0 * MUTATION_AMOUNT * 2) - MUTATION_AMOUNT));
                    if (new_value > Chromosome.MAXIMUM_Y)
                    {
                        new_value = Chromosome.MAXIMUM_Y;
                    }
                    if (new_value < 0)
                    {
                        new_value = 0;
                    }
                    points[random_point].Y = new_value;
                    break;
            }
            return new Chromosome(color, points);
        }

        internal System.Drawing.Bitmap GetImage(int width, int height)
        {
            System.Drawing.Bitmap output = new System.Drawing.Bitmap(width, height);
            System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(output);

            System.Drawing.SolidBrush brush;
            for (int i = 0; i < DNA.MAXIMUM_CHROMOSOMES; i++)
            {
                brush = new System.Drawing.SolidBrush(_chromosomes[i].Color);
                g.FillPolygon(brush, _chromosomes[i].Points);
            }
            g.Flush();
            g.Dispose();

            return output;
        }


    }
}
