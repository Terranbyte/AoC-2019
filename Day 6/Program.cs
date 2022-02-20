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
            SearchPath();
        }

        static List<string> SearchPath()
        {
            List<string> path = new List<string>();
            HashSet<string> SANToCOM = new HashSet<string>();
            HashSet<string> YOUToCOM = new HashSet<string>();

            Orbit current = orbits.Find(x => x.id == "SAN");
            Orbit parent = orbits.Find(x => x.id == current.directOrbit);

            // Find path from SAN to COM
            while (parent != null && parent.id != "COM")
            {
                SANToCOM.Add(parent.id);
                parent = orbits.Find(x => x.id == parent.directOrbit);
            }

            current = orbits.Find(x => x.id == "YOU");
            parent = orbits.Find(x => x.id == current.directOrbit);

            // Find path from YOU to COM
            while (parent != null && parent.id != "COM")
            {
                YOUToCOM.Add(parent.id);
                parent = orbits.Find(x => x.id == parent.directOrbit);
            }

            List<string> temp = SANToCOM.ToList();
            // Find where both paths intersect and merge them
            for (int i = 0; i < temp.Count; i++)
            {
                if (YOUToCOM.Contains(temp[i]))
                {
                    // Todo: Splice together the two paths
                    Console.WriteLine($"Found intersecting path at {temp[i]}");
                    return path;
                }
            }

            return path;
        }
    }
}
