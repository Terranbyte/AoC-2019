using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intcode_VM
{
    public struct Opcode
    {
        public OpcodeParameter arg1;
        public OpcodeParameter arg2;
        public OpcodeParameter arg3;
        public int opcode;

        public Opcode(int opcode, OpcodeParameter argA, OpcodeParameter argB, OpcodeParameter argC)
        {
            this.arg1 = argA;
            this.arg2 = argB;
            this.arg3 = argC;
            this.opcode = opcode;
        }
    }

    public struct OpcodeParameter
    {
        public ParameterMode mode;
        public int value;

        public OpcodeParameter(ParameterMode mode, int value)
        {
            this.mode = mode;
            this.value = value;
        }
    }

    public enum ParameterMode
    {
        Address = 0,
        Immediate = 1,
    }
}
