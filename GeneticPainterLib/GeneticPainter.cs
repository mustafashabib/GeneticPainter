using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;


namespace GeneticPainterLib
{
    
    public class GeneticPainter
    {
        public static int WIDTH = 800;
        public static int HEIGHT = 600;
        public static int[] CLOSE_ENOUGH_SCORES = { 4000000, 3000000, 2000000, 1000000 };
        public static int MAXIMUM_ITERATIONS = 100000;
        public const  int MAXIMUM_POPULATION = 40;
        public const double PopulationPercentageToBreed =.25;
        public const bool KillParents = true;
        private System.Drawing.Bitmap _goal_image;
        private System.Drawing.Bitmap[] _goal_fitness_test = new System.Drawing.Bitmap[4];
        private System.Collections.Generic.SortedList<long,DNA> _population;
        private DNA _best_generation;
        private int _current_generation;
        private long _best_generation_score = long.MaxValue;
        private int _current_generation_tier = 0;
#if DEBUG
        private string _save_to_dir;
#endif
        private static int _breed_top_index;
        

        public long BestGenerationScore
        {
            get { return _best_generation_score; }
        }
        public int NumberOfGenerations
        {
            get { return _current_generation; }
        }
        public GeneticPainter(string goal_image_path, bool resize)
        {
#if DEBUG
            _save_to_dir = System.IO.Path.GetDirectoryName(goal_image_path);
#endif
            _breed_top_index = Convert.ToInt32(Math.Floor(MAXIMUM_POPULATION * PopulationPercentageToBreed));
           
            _goal_image = new System.Drawing.Bitmap(goal_image_path);
            for (int i = 0; i < 4; i++)
            {
                double resize_percentage = .25 + (i * .25);
                _goal_fitness_test[i] = DrawingHelper.ResizeImage(_goal_image, Convert.ToInt32(_goal_image.Width * resize_percentage), Convert.ToInt32(_goal_image.Height * resize_percentage));//thumbnail//thumbnail
            }

            if (resize)
            {
                if (_goal_image.Width != WIDTH || _goal_image.Height != HEIGHT)
                {
                    _goal_image = DrawingHelper.ResizeImage(_goal_image, WIDTH, HEIGHT);
                }
                WIDTH = _goal_image.Width;
                HEIGHT = _goal_image.Height;
            }
            else
            {
                WIDTH = _goal_image.Width;
                HEIGHT = _goal_image.Height;
            }
            Chromosome.MAXIMUM_X = WIDTH;
            Chromosome.MAXIMUM_Y = HEIGHT;
        }

        public GeneticPainter(string goal_image_path, int width, int height)
        {
#if DEBUG
            _save_to_dir = System.IO.Path.GetDirectoryName(goal_image_path);
#endif
            _breed_top_index = Convert.ToInt32(Math.Floor(MAXIMUM_POPULATION * PopulationPercentageToBreed));

            _goal_image = new System.Drawing.Bitmap(goal_image_path);

            for (int i = 0; i < 4; i++)
            {
                double resize_percentage = .25 + (i * .25);
                _goal_fitness_test[i] = DrawingHelper.ResizeImage(_goal_image, Convert.ToInt32(_goal_image.Width * resize_percentage), Convert.ToInt32(_goal_image.Height * resize_percentage));//thumbnail//thumbnail
            } 
            GeneticPainter.WIDTH = width;
            GeneticPainter.HEIGHT = height;
            if (_goal_image.Width != width || _goal_image.Height != height)
            {
                _goal_image = DrawingHelper.ResizeImage(_goal_image, width, height);
            }
            Chromosome.MAXIMUM_X = _goal_image.Width;
            Chromosome.MAXIMUM_Y = _goal_image.Height;
        }

        public GeneticPainter(string goal_image_path, int width, int height, int points_in_shape)
        {
#if DEBUG
            _save_to_dir = System.IO.Path.GetDirectoryName(goal_image_path);
#endif
            _breed_top_index = Convert.ToInt32(Math.Floor(MAXIMUM_POPULATION * PopulationPercentageToBreed));

            _goal_image = new System.Drawing.Bitmap(goal_image_path);
            for (int i = 0; i < 4; i++)
            {
                double resize_percentage = .25 + (i * .25);
                _goal_fitness_test[i] = DrawingHelper.ResizeImage(_goal_image, Convert.ToInt32(_goal_image.Width * resize_percentage), Convert.ToInt32(_goal_image.Height * resize_percentage));//thumbnail//thumbnail
            } 
            GeneticPainter.WIDTH = width;
            GeneticPainter.HEIGHT = height;
            if (_goal_image.Width != width || _goal_image.Height != height)
            {
                _goal_image = DrawingHelper.ResizeImage(_goal_image, width, height);
            }
            Chromosome.MAXIMUM_X = _goal_image.Width;
            Chromosome.MAXIMUM_Y = _goal_image.Height;
            Chromosome.MAXIMUM_POINTS = points_in_shape;
        }

        public GeneticPainter(string goal_image_path, int width, int height, int points_in_shape, int number_of_shapes)
        {
#if DEBUG
            _save_to_dir = System.IO.Path.GetDirectoryName(goal_image_path);
#endif
            _breed_top_index = Convert.ToInt32(Math.Floor(MAXIMUM_POPULATION * PopulationPercentageToBreed));

            _goal_image = new System.Drawing.Bitmap(goal_image_path);
            for (int i = 0; i < 4; i++)
            {
                double resize_percentage = .25 + (i * .25);
                _goal_fitness_test[i] = DrawingHelper.ResizeImage(_goal_image, Convert.ToInt32(_goal_image.Width * resize_percentage), Convert.ToInt32(_goal_image.Height * resize_percentage));//thumbnail//thumbnail
            } 
            GeneticPainter.WIDTH = width;
            GeneticPainter.HEIGHT = height;
            if (_goal_image.Width != width || _goal_image.Height != height)
            {
                _goal_image = DrawingHelper.ResizeImage(_goal_image, width, height);
            }
            Chromosome.MAXIMUM_X = _goal_image.Width;
            Chromosome.MAXIMUM_Y = _goal_image.Height;
            Chromosome.MAXIMUM_POINTS = points_in_shape;
            DNA.MAXIMUM_CHROMOSOMES = number_of_shapes;
        }

        public System.Drawing.Bitmap Evolve()
        {
            //initialize population
            _population = new SortedList<long,DNA>();
            for(int current_individual = 0; current_individual < MAXIMUM_POPULATION; current_individual++)
            {
                DNA individual = new DNA();
                _AddDnaToList(ref _population, ref individual); 
                //_population.Add(current_individual, new DNA());
            }

            while (_current_generation_tier < 3)
            {
                _best_generation_score = long.MaxValue;
                while (_best_generation_score > CLOSE_ENOUGH_SCORES[_current_generation_tier])
                {
                    if (_current_generation > MAXIMUM_ITERATIONS)
                    {
                        break;
                    }
                   // _ScorePopulation();
                    //get the top % of the population, so kill the remaining individuals
                    for (int i = _breed_top_index; i < MAXIMUM_POPULATION; i++)
                    {
                        _population.RemoveAt(_population.Count - 1);
                    }

                    //breed and score the top population
                    _BreedIndividuals();
                }
                _current_generation_tier++;
                _ScorePopulation(); //rescore everyone with the new thumbnail
            }
            return _best_generation.GetImage(_goal_image.Width, _goal_image.Height);
        }

        private void _BreedIndividuals()
        {
            System.Collections.Generic.SortedList<long, DNA> children = new SortedList<long, DNA>();

            int number_of_children = Convert.ToInt32(Math.Ceiling(1 / PopulationPercentageToBreed));
            if (!KillParents)
            {
                number_of_children--;
            }

            for (int i = 0; i < _breed_top_index; i++)
            {
                DNA dna = _population.Values[i];
                for (int j = 0; j < number_of_children; j++)
                {
                    int random_candidate = Convert.ToInt32(RandomNumber.GetRandom(0, _breed_top_index));
                    while (random_candidate == i)
                    {
                        random_candidate = Convert.ToInt32(RandomNumber.GetRandom(0, _breed_top_index));
                    }

                    DNA child = new DNA(dna, _population.Values[random_candidate]);
                    _AddDnaToList(ref children, ref child);
                }
            }
            if (KillParents)
            {
                _population = children;
            }
            else
            {
                for (int i = 0; i < children.Count; i++)
                {
                    long current_child_score = children.Keys[i];
                    while (_population.ContainsKey(current_child_score))
                    {
                        current_child_score++;
                    }
                    _population.Add(current_child_score, children.Values[i]);
                }
            }

            if (_population.Keys[0] < _best_generation_score)
            {
                _best_generation_score = _population.Keys[0];
                _best_generation = _population.Values[0];
#if DEBUG
    //            _best_generation.GetImage(_goal_image.Width, _goal_image.Height).Save(_save_to_dir + "\\best\\" + _current_generation.ToString() + ".png");
  /*              System.IO.Stream myStream;
                myStream = System.IO.File.OpenWrite(_save_to_dir + "\\best\\" + _current_generation.ToString() + ".gpi");
                if (myStream != null)
                {
                    IFormatter formatter =
                       new BinaryFormatter();
                    // serialize shapes
                    formatter.Serialize(myStream, _best_generation);
                    myStream.Close();
   
                }* */
        
#endif
            }

#if DEBUG
            if (_current_generation % 50 == 0)
                _population.Values[0].GetImage(_goal_image.Width, _goal_image.Height).Save(_save_to_dir + "\\intervals\\" + _current_generation.ToString() + ".png");
#endif
            _current_generation++;
        }

        private void _AddDnaToList(ref SortedList<long, DNA> list, ref DNA dna)
        {
            
                long score = _ScoreIndividual(dna);
                while(list.ContainsKey(score))
                {
                    score++;
                }
                list.Add(score, dna);
                return;
           
        }

        private void _ScorePopulation()
        {
            System.Collections.Generic.SortedList<long, DNA> fittest = new SortedList<long, DNA>();
            int duplicates = 1;
            foreach(KeyValuePair<long,DNA> current_individual in _population)
            {
                long current_score = _ScoreIndividual(current_individual.Value);
                if (!fittest.ContainsKey(current_score))
                    fittest.Add(current_score, current_individual.Value);
                else
                {
                    while (fittest.ContainsKey(current_score + duplicates))
                    {
                        duplicates++;
                    }
                    fittest.Add(current_score + duplicates, current_individual.Value);
                }

            }
            _population = fittest;
            if (_population.Keys[0] < _best_generation_score)
            {
                _best_generation_score = _population.Keys[0];
                _best_generation = _population.Values[0];
#if DEBUG
                _best_generation.GetImage(_goal_image.Width, _goal_image.Height).Save(_save_to_dir + "\\best\\" + _current_generation.ToString() + ".png");
#endif
            }

#if DEBUG
            if(_current_generation % 50 == 0)
                _population.Values[0].GetImage(_goal_image.Width, _goal_image.Height).Save(_save_to_dir + "\\intervals\\" + _current_generation.ToString() + ".png");
#endif

        }

        private long _ScoreIndividual(DNA individual)
        {


            System.Drawing.Bitmap current_individual_image = individual.GetImage(_goal_image.Width, _goal_image.Height);
            if (_current_generation_tier < 3)
            {
                double resize_percentage = .25 + (_current_generation_tier * .25);

                System.Drawing.Bitmap current_individual_small_image = DrawingHelper.ResizeImage(current_individual_image, Convert.ToInt32(_goal_image.Width * resize_percentage), Convert.ToInt32(_goal_image.Height * resize_percentage));
                long fitness = 0;
                for (int current_column = 0; current_column < _goal_fitness_test[_current_generation_tier].Width; current_column++)
                {
                    for (int current_row = 0; current_row < _goal_fitness_test[_current_generation_tier].Height; current_row++)
                    {
                        System.Drawing.Color current_color = _goal_fitness_test[_current_generation_tier].GetPixel(current_column, current_row);
                        System.Drawing.Color current_best_color = current_individual_small_image.GetPixel(current_column, current_row);

                        int deltaRed = current_color.R - current_best_color.R;
                        int deltaGreen = current_color.G - current_best_color.G;
                        int deltaBlue = current_color.B - current_best_color.B;

                        fitness += (deltaRed * deltaRed) +
                                    (deltaGreen * deltaGreen) +
                                    (deltaBlue * deltaBlue);
                    }
                }
                current_individual_small_image.Dispose();
                current_individual_image.Dispose();
                return fitness;
            }
            else
            {
                long fitness = 0;
                for (int current_column = 0; current_column < _goal_image.Width; current_column++)
                {
                    for (int current_row = 0; current_row < _goal_image.Height; current_row++)
                    {
                        System.Drawing.Color current_color = _goal_image.GetPixel(current_column, current_row);
                        System.Drawing.Color current_best_color = current_individual_image.GetPixel(current_column, current_row);

                        int deltaRed = current_color.R - current_best_color.R;
                        int deltaGreen = current_color.G - current_best_color.G;
                        int deltaBlue = current_color.B - current_best_color.B;

                        fitness += (deltaRed * deltaRed) +
                                    (deltaGreen * deltaGreen) +
                                    (deltaBlue * deltaBlue);
                    }
                }
                current_individual_image.Dispose();
                return fitness;
            }
            
          
            //rgba blending formula:
            //(1-as)rs + asrd = rf
            //(1-as)gs + asgd = gf
            //(1-as)bs + asbd = bf
            //(1-as)as + asad = af

            //steps:
            //find polygons in current individual that contain the point given by the pixel we are searching
            //blend their colors (each polygon's output becomes the rd in the equation above)
            //compare this color to destination color
            //add to the running fitness sum
            /*
            long fitness = 0;
            System.Collections.Generic.List<System.Drawing.Color> chromosome_colors = new List<System.Drawing.Color>();
            for (int current_column = 0; current_column < _goal_image.Width; current_column++)
            {
                for (int current_row = 0; current_row < _goal_image.Height; current_row++)
                {
                    chromosome_colors.Clear();
                    chromosome_colors.Add(System.Drawing.Color.White); //white background
            
                    System.Drawing.Color current_color = _goal_image.GetPixel(current_column, current_row);

                    for (int current_chromosome = 0; current_chromosome < individual.Chromosomes.Length; current_chromosome++)
                    {
                        if (individual.Chromosomes[current_chromosome].ContainsPoint(current_column, current_row))
                        {
                            chromosome_colors.Add(individual.Chromosomes[current_chromosome].Color);
                        }
                    }


                    //System.Drawing.Color current_best_color = current_individual_image.GetPixel(current_column, current_row);

                    System.Drawing.Color current_individual_color = DrawingHelper.BlendColors(chromosome_colors.ToArray());

                    int deltaRed = current_color.R - current_individual_color.R;
                    int deltaGreen = current_color.G - current_individual_color.G;
                    int deltaBlue = current_color.B - current_individual_color.B;

                    fitness += (deltaRed * deltaRed) +
                                (deltaGreen * deltaGreen) +
                                (deltaBlue * deltaBlue);
                }
            }
            return fitness;*/
        }

        

    }
}
