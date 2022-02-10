using System;
using System.Collections.Generic;
using System.Diagnostics;
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

            Opcode currentOpcode = new Opcode();
            do
            {
                currentOpcode = DecodeOpcode(_pc);
                if (!RunInstruction(currentOpcode))
                    return;
            }
            while (currentOpcode.opcode != 99);

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

        private bool RunInstruction(Opcode op)
        {
            switch (op.opcode)
            {
                case 1: // Add
                    _program[op.arg3.value] = ResolveParameter(op.arg1) + ResolveParameter(op.arg2);
                    _pc += 4;

                    //if (_debugEnable)
                    //    _debug.WriteLine($"Add: ${dest} = ${_program[_pc + 1]} (#{arg1}) + ${_program[_pc + 2]} (#{arg2}), res = #{_program[dest]}");
                    break;
                case 2: // Mult
                    _program[op.arg3.value] = ResolveParameter(op.arg1) * ResolveParameter(op.arg2);
                    _pc += 4;

                    //if (_debugEnable)
                    //    _debug.WriteLine($"Multiply: ${dest} = ${_program[_pc + 1]} (#{arg1}) * ${_program[_pc + 2]} (#{arg2}), res = #{_program[dest]}");
                    break;
                case 3: // Input
                    Console.Write("Input: ");
                    _program[op.arg1.value] = Convert.ToInt32(Console.ReadLine()); 
                    _pc += 2;
                    break;
                case 4: // Output
                    Console.WriteLine(ResolveParameter(op.arg1));
                    _pc += 2;
                    break;
                case 5:
                    _pc += 3;

                    if (ResolveParameter(op.arg1) != 0)
                        _pc = ResolveParameter(op.arg2);
                    break;
                case 6:
                    _pc += 3;

                    if (ResolveParameter(op.arg1) == 0)
                        _pc = ResolveParameter(op.arg2);
                    break;
                case 7:
                    _program[op.arg3.value] = Convert.ToInt32(ResolveParameter(op.arg1) < ResolveParameter(op.arg2));
                    _pc += 4;
                    break;
                case 8:
                    _program[op.arg3.value] = Convert.ToInt32(ResolveParameter(op.arg1) == ResolveParameter(op.arg2));
                    _pc += 4;
                    break;
                case 99:
                    //if (_debugEnable)
                    //    _debug.WriteLine("Halt");
                    break;
                default:
                    Exception e = new ArgumentOutOfRangeException($"Opcode \"{op.opcode}\" is not a valid opcode. Halting execution...");
                    //if (_debugEnable)
                    //    _debug.WriteLine(e);

                    Console.WriteLine(e);
                    //_debug.Dispose();
                    return false;
            }
            return true;
        }

        private int ResolveParameter(OpcodeParameter param)
        {
            switch (param.mode)
            {
                case ParameterMode.Address:
                    return _program[param.value];
                case ParameterMode.Immediate:
                    return param.value;
                default:
                    throw new ArgumentOutOfRangeException($"Parameter mode \"{param.mode}\" is not implemented/supported");
            }
        }

        private Opcode DecodeOpcode(int pc)
        {
            Opcode o = new Opcode();

            string fullOpcode = _program[pc].ToString().PadLeft(5, '0');

            o.opcode = Convert.ToInt32(fullOpcode.Substring(3));
            try
            {
                o.arg1 = new OpcodeParameter((ParameterMode)Convert.ToInt32(fullOpcode[2].ToString()), _program[pc + 1]);
                o.arg2 = new OpcodeParameter((ParameterMode)Convert.ToInt32(fullOpcode[1].ToString()), _program[pc + 2]);
                o.arg3 = new OpcodeParameter((ParameterMode)Convert.ToInt32(fullOpcode[0].ToString()), _program[pc + 3]);
            }
            catch (IndexOutOfRangeException)
            {
                Debug.WriteLine("Warning: couldn't read all instruction arguments, assuming EOF");
                return o;
            }
            catch (Exception e)
            {
                throw e;
            }

            return o;
        }
    }
}
