using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intcode_VM
{
    public static class Extensions
    {
        public static IEnumerable<int> Swap(this IEnumerable<int> t, int first, int second)
        {
            List<int> l = t.ToList();
            int temp = l[first];
            l[first] = l[second];
            l[second] = temp;
            return l;
        }
    }
}
