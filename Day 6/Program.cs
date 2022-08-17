using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Day_6
{
    internal class Program
    {
        static List<Orbit> orbits = new List<Orbit>();
        static int moves = 0;

        static void Main(string[] args)
        {
            orbits.Add(new Orbit("COM", string.Empty));

            using (StreamReader sr = new StreamReader(File.Open("./input.txt", FileMode.Open)))
            {
                while (!sr.EndOfStream)
                {
                    string[] orbitStrings = sr.ReadLine().Split(')');

                    // Step 1: Add all direct orbits
                    orbits.Add(new Orbit(orbitStrings[1], orbitStrings[0]));
                }
            }

            // Step 2: Go through orbits backwards and add all IDs until COM is reached
            for (int i = orbits.Count - 1; i >= 0; i--)
            {

                Orbit current = orbits[i];
                Orbit parent = orbits.Find(x => x.id == current.directOrbit);

                while (parent != null && parent.id != "COM")
                {
                    parent = orbits.Find(x => x.id == parent.directOrbit);

                    if (parent == null)
                        continue;

                    current.indirectOrbits.Add(parent.id);
                }
            }

            Part2();

            Console.ReadLine();
        }

        static void Part1()
        {
            int checksum = 0;

            foreach (Orbit orbit in orbits)
            {
                checksum += orbit.indirectOrbits.Count + (string.IsNullOrEmpty(orbit.directOrbit) ? 0 : 1);
            }

            Console.WriteLine(checksum);
        }

        static void Part2()
        {
            /*
             * The theory:
             * 
             * Beacuse every planet originates from the same root planet (COM) they will always cross paths at some point
             * 
             * 
             * 
             * The algorithm:
             * 
             * 1. Create paths YOU->COM and SAN->COM
             * 2. Find the first intersection of the paths (first planet that shares an ID)
             * 3. Splice together YOU->COM and SAN->COM at the intersection to create the path YOU->SAN
             * 4. The answer is the length of the path (-2 if including YOU and SAN)
             */

            List<string> path = SearchPath().Distinct().ToList();

            Console.WriteLine("Minimum transfers needed to reach santa: " + (path.Count - 3));
        }

        static List<string> SearchPath()
        {
            List<string> path = new List<string>();
            HashSet<string> SANToCOM = new HashSet<string>();
            HashSet<string> YOUToCOM = new HashSet<string>();

            Orbit current = orbits.Find(x => x.id == "SAN");

            // Create path from SAN to COM
            while (current != null && current.id != "COM")
            {
                SANToCOM.Add(current.id);
                current = orbits.Find(x => x.id == current.directOrbit);
            }

            current = orbits.Find(x => x.id == "YOU");

            // Create path from YOU to COM
            while (current != null && current.id != "COM")
            {
                YOUToCOM.Add(current.id);
                current = orbits.Find(x => x.id == current.directOrbit);
            }

            List<string> temp = YOUToCOM.ToList();
            // Find where both paths intersect and merge them
            for (int i = 1; i < temp.Count; i++)
            {
                if (SANToCOM.Contains(temp[i]))
                {
                    Console.WriteLine($"Found intersecting path at {temp[i]} ({i})");



                    Console.WriteLine("Splicing paths together");

                    path.AddRange(temp.Where(s => temp.IndexOf(s) < i));
                    path.AddRange(SANToCOM.Where(s => YOUToCOM.ToList().IndexOf(s) <= i).Reverse());

                    return path;
                }
            }

            return path;
        }
    }
}
