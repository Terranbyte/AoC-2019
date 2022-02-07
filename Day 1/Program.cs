using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Day_1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            List<double> nums = new List<double>();

            using (StreamReader sr = new StreamReader(File.Open("./input.txt", FileMode.Open)))
            {
                while (!sr.EndOfStream)
                {
                    List<double> fuels = new List<double>();
                    double initialFuel = Math.Floor(Convert.ToDouble(sr.ReadLine()) / 3) - 2;

                    while (initialFuel > 0)
                    {
                        fuels.Add(initialFuel);
                        initialFuel = Math.Floor(initialFuel / 3) - 2;
                    }

                    nums.Add(fuels.Sum());
                }
            }

            Console.WriteLine(nums.Sum());
        }
    }
}
