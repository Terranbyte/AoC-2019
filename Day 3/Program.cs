using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Day_3
{
    internal class Program
    {
        static HashSet<Vector2> coords = new HashSet<Vector2>();
        static List<int> coordsSteps = new List<int>();
        static int secondSteps = 0;
        static int closestTotal = int.MaxValue;

        static void Main(string[] args)
        {
            List<string[]> wires = new List<string[]>();

            using (StreamReader sr = new StreamReader(File.Open("./input.txt", FileMode.Open)))
            {
                while (!sr.EndOfStream)
                {
                    wires.Add(sr.ReadLine().Split(','));
                }
            }

            DrawWire1(wires[0]);
            DrawWire2(wires[1]);

            Console.WriteLine(closestTotal);
            Console.ReadLine();
        }

        static void DrawWire1(string[] commands)
        {
            HashSet<Vector2> localCoords = new HashSet<Vector2>();
            List<int> localSteps = new List<int>();
            Vector2 pos = Vector2.Zero;
            int totalSteps = 0;

            foreach (string s in commands)
            {
                for (int i = 0; i < int.Parse(s.Substring(1)); i++)
                {
                    switch (s[0])
                    {
                        case 'U':
                            pos.Y += 1;
                            break;
                        case 'D':
                            pos.Y -= 1;
                            break;
                        case 'L':
                            pos.X -= 1;
                            break;
                        case 'R':
                            pos.X += 1;
                            break;
                        default:
                            throw new ArgumentException($"\"{s[0]}\" is not a valid direction");
                    }

                    totalSteps += 1;

                    if (!localCoords.Contains(pos))
                    {
                        localCoords.Add(pos);
                        localSteps.Add(totalSteps);
                    }
                }
            }

            foreach (Vector2 v in localCoords)
            {
                coords.Add(v);
            }

            coordsSteps.AddRange(localSteps);
        }

        static void DrawWire2(string[] commands)
        {
            Vector2 pos = Vector2.Zero;

            foreach (string s in commands)
            {
                for (int i = 0; i < int.Parse(s.Substring(1)); i++)
                {
                    switch (s[0])
                    {
                        case 'U':
                            pos.Y += 1;
                            break;
                        case 'D':
                            pos.Y -= 1;
                            break;
                        case 'L':
                            pos.X -= 1;
                            break;
                        case 'R':
                            pos.X += 1;
                            break;
                        default:
                            throw new ArgumentException($"\"{s[0]}\" is not a valid direction");
                    }

                    secondSteps += 1;

                    if (!coords.Contains(pos))
                        continue;

                    int localTotal = secondSteps + coordsSteps[coords.ToList().IndexOf(pos)];

                    if (localTotal < closestTotal)
                        closestTotal = localTotal;
                }
            }
        }

        private static int ManhattanDistance(Vector2 v)
        {
            return (int)(Math.Abs(v.X) + Math.Abs(v.Y));
        }
    }
}
