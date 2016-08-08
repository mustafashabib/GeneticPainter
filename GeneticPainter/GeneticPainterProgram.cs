using System;
using System.Collections.Generic;
using System.Text;

namespace GeneticPainter
{
    class GeneticPainterProgram
    {
        static void Main(string[] args)
        {
            try
            {
                
                Console.Write("Path to image: ");
                string source_image = Console.ReadLine();
                GeneticPainterLib.GeneticPainter painter = new GeneticPainterLib.GeneticPainter(source_image,false);
                System.Drawing.Bitmap output = painter.Evolve();
                output.Save(
                    System.IO.Path.Combine(
                    System.IO.Path.GetDirectoryName(source_image),
                    System.IO.Path.GetRandomFileName() + System.IO.Path.GetExtension(source_image)));
                output.Dispose();

            }
            catch (Exception ex)
            {
                Exception current = ex;
                while (current != null)
                {
                    Console.WriteLine(current.Message);
                    current = current.InnerException;

                }
                Console.WriteLine();
                Console.Write(ex.StackTrace);
            }
            finally
            {
                Console.WriteLine("Press any key to exit.");
                Console.ReadKey();
            }
        }
    }
}
