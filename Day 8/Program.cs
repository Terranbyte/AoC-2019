using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Day_8
{
    internal class Program
    {
        static SIFImage img;
        const int W = 25;
        const int H = 6;

        static void Main(string[] args)
        {
            img = new SIFImage(W, H, File.OpenRead("./image.sif"));

            Part2();
            Console.CursorVisible = false;
            Console.ReadLine();
        }

        static void Part1()
        {
            List<List<byte>> layers = new List<List<byte>>();
            int leastZeros = int.MaxValue;
            int layerIndex = -1;
            int layerSize = W * H;

            Console.WriteLine("Getting layers...");
            for (int i = 0; i < img.layers; i++)
            {
                layers.Add(img.GetRawData().Skip(layerSize * i).Take(layerSize).ToList());
            }
            Console.WriteLine("Done!");

            Console.WriteLine("Finding layer with least zeros...");
            for (int i = layers.Count - 1; i >= 0; i--)
            {
                int zeros = layers[i].FindAll(x => x == 0).ToArray().Length;

                if (zeros < leastZeros)
                {
                    leastZeros = zeros;
                    layerIndex = i;
                }
            }
            Console.WriteLine("Done!");

            Console.WriteLine("Calculating answer...");
            int ones = layers[layerIndex].FindAll(x => x == 1).ToArray().Length;
            int twos = layers[layerIndex].FindAll(x => x == 2).ToArray().Length;
            Console.WriteLine("Done!");
            Console.WriteLine($"Answer: {ones * twos}");
        }

        static void Part2()
        {
            img.RenderImage(0, 0);
        }
    }
}
