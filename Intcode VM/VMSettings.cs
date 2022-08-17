using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intcode_VM
{
    [Flags]
    public enum VMSettings
    {
        Standard = 0,
        DebugEnable = 1,
        AutomaticInput = 2,
        NoOutput = 4,
        AsyncOutput = 8,
    }
}
