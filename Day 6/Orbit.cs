using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Day_6
{
    public class Orbit
    {
        public string id;
        public string directOrbit;
        public HashSet<string> indirectOrbits;

        public Orbit(string id, string directOrbit)
        {
            this.id = id;
            this.directOrbit = directOrbit;
            indirectOrbits = new HashSet<string>();
        }
    }
}
