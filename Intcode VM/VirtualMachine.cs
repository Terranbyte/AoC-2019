using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intcode_VM
{
    public class VirtualMachine
    {
        private int[] _program = null;
        private int _pc = 0;
        private bool _debugEnable = false;
        private StreamWriter _debug;

        public VirtualMachine(int[] program, bool debugEnable = false)
        {
            _program = program;
            _debugEnable = debugEnable;
        }

        public void Run()
        {
            if (_debugEnable)
                _debug = new StreamWriter(File.Open("./debug.log", FileMode.Create));

            int currentInstruction;
            do
            {
                currentInstruction = _program[_pc];
                if (!RunInstruction(currentInstruction))
                    return;
            }
            while (currentInstruction != 99);

            if (!_debugEnable)
                return;

            _debug.WriteLine("\nMemory dump:");
            for (int i = 0; i < _program.Length; i++)
            {
                _debug.WriteLine($"\t0x{Convert.ToString(i, 16)}: {_program[i]}");
            }

            _debug.Dispose();
        }

        public void DumpMemory(int from = 0)
        {
            for (int i = from; i < _program.Length; i++)
            {
                Console.WriteLine($"0x{Convert.ToString(i, 16)}: {_program[i]}");
            }
        }

        public void DumpMemory(int from, int to)
        {
            for (int i = from; i < to; i++)
            {
                Console.WriteLine($"0x{Convert.ToString(i, 16)}: {_program[i]}");
            }
        }

        public int GetMemoryValue(int addr)
        {
            return _program[addr];
        }

        private bool RunInstruction(int opcode)
        {
            int arg1;
            int arg2;
            int dest;

            switch (opcode)
            {
                case 1:
                    arg1 = _program[_program[_pc + 1]];
                    arg2 = _program[_program[_pc + 2]];
                    dest = _program[_pc + 3];
                    _program[dest] = arg1 + arg2;

                    if (_debugEnable)
                        _debug.WriteLine($"Add: ${dest} = ${_program[_pc + 1]} (#{arg1}) + ${_program[_pc + 2]} (#{arg2}), res = #{_program[dest]}");
                    break;
                case 2:
                    arg1 = _program[_program[_pc + 1]];
                    arg2 = _program[_program[_pc + 2]];
                    dest = _program[_pc + 3];
                    _program[dest] = arg1 * arg2;

                    if (_debugEnable)
                        _debug.WriteLine($"Multiply: ${dest} = ${_program[_pc + 1]} (#{arg1}) * ${_program[_pc + 2]} (#{arg2}), res = #{_program[dest]}");
                    break;
                case 99:
                    if (_debugEnable)
                        _debug.WriteLine("Halt");
                    break;
                default:
                    Exception e = new ArgumentOutOfRangeException($"Opcode \"{opcode}\" is not a valid opcode. Halting execution...");
                    if (_debugEnable)
                        _debug.WriteLine(e);

                    Console.WriteLine(e);
                    _debug.Dispose();
                    return false;
            }

            _pc += 4;
            return true;
        }
    }
}
