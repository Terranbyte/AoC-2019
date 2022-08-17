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
        public event Func<long> OnInput;
        public event Action<long> OnOutput;

        public bool IsHalted = false;

        private Queue<long> _outputQueue = new Queue<long>();
        private long[] _program = null;
        private long _pc = 0;
        private long _relBase = 0;
        private bool _debugEnable = false;
        private bool _autoInput = false;
        private bool _noOutput = false;
        private bool _asyncOutput = false;
        private StreamWriter _debug;

        [Obsolete("This constructor is obsolete, use the VMSettings variant")]
        public VirtualMachine(long[] program, bool debugEnable = false)
        {
            _program = program;
            _debugEnable = debugEnable;
        }

        public VirtualMachine(long[] program, VMSettings settings)
        {
            _program = program;
            _debugEnable = settings.HasFlag(VMSettings.DebugEnable);
            _autoInput = settings.HasFlag(VMSettings.AutomaticInput);
            _noOutput = settings.HasFlag(VMSettings.NoOutput);
            _asyncOutput = settings.HasFlag(VMSettings.AsyncOutput);
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

            IsHalted = true;

            _debug.WriteLine("\nMemory dump:");
            for (int i = 0; i < _program.Length; i++)
            {
                _debug.WriteLine($"\t0x{Convert.ToString(i, 16)}: {_program[i]}");
            }

            _debug.Dispose();
        }

        public async Task RunAsync()
        {
            await Task.Run(() => Run());
        }

        public void LoadProgram(long[] program)
        {
            _pc = 0;
            _program = program;
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

        public long GetMemoryValue(int addr)
        {
            return _program[addr];
        }

        public async Task<long> GetOutputAsync()
        {
            if (!_asyncOutput)
                throw new Exception("Trying to await output on syncronus machine");

            while (_outputQueue.Count <= 0)
            {
                await Task.Delay(50);
            }

            return _outputQueue.Dequeue();
        }

        private bool RunInstruction(Opcode op)
        {
            switch (op.opcode)
            {
                case 1: // Add
                    _program[ResolveParameter(op.arg3)] = ResolveParameter(op.arg1) + ResolveParameter(op.arg2);
                    _pc += 4;

                    //if (_debugEnable)
                    //    _debug.WriteLine($"Add: ${dest} = ${_program[_pc + 1]} (#{arg1}) + ${_program[_pc + 2]} (#{arg2}), res = #{_program[dest]}");
                    break;
                case 2: // Mult
                    _program[ResolveParameter(op.arg3)] = ResolveParameter(op.arg1) * ResolveParameter(op.arg2);
                    _pc += 4;

                    //if (_debugEnable)
                    //    _debug.WriteLine($"Multiply: ${dest} = ${_program[_pc + 1]} (#{arg1}) * ${_program[_pc + 2]} (#{arg2}), res = #{_program[dest]}");
                    break;
                case 3: // Input
                    long input = 0;

                    if (_autoInput)
                    {
                        input = OnInput.Invoke();
                    }
                    else
                    {
                        if (!_noOutput)
                            Console.Write("Input: ");
                        input = Convert.ToInt32(Console.ReadLine());
                    }

                    _program[op.arg1.value] = input;
                    _pc += 2;
                    break;
                case 4: // Output
                    long output = ResolveParameter(op.arg1);

                    if (!_noOutput)
                        Console.WriteLine(output);
                    
                    if (_asyncOutput)
                        _outputQueue.Enqueue(output);

                    if (OnOutput != null)
                        OnOutput.Invoke(output);

                    _pc += 2;
                    break;
                case 5: // Jump if true
                    _pc += 3;

                    if (ResolveParameter(op.arg1) != 0)
                        _pc = ResolveParameter(op.arg2);
                    break;
                case 6: // Jump if false
                    _pc += 3;

                    if (ResolveParameter(op.arg1) == 0)
                        _pc = ResolveParameter(op.arg2);
                    break;
                case 7: // Less than
                    _program[ResolveParameter(op.arg3)] = Convert.ToInt32(ResolveParameter(op.arg1) < ResolveParameter(op.arg2));
                    _pc += 4;
                    break;
                case 8: // Equals
                    _program[ResolveParameter(op.arg3)] = Convert.ToInt32(ResolveParameter(op.arg1) == ResolveParameter(op.arg2));
                    _pc += 4;
                    break;
                case 9: // Relative Base Offset
                    _pc += 2;

                    _relBase += ResolveParameter(op.arg1);
                    break;
                case 99: // Halt
                    //if (_debugEnable)
                    //    _debug.WriteLine("Halt");
                    break;
                default: // *fuck*
                    Exception e = new ArgumentOutOfRangeException($"Opcode \"{op.opcode}\" is not a valid opcode. Halting execution...");
                    //if (_debugEnable)
                    //    _debug.WriteLine(e);

                    Console.WriteLine(e);
                    //_debug.Dispose();
                    return false;
            }
            return true;
        }

        private long ResolveParameter(OpcodeParameter param)
        {
            switch (param.mode)
            {
                case ParameterMode.Address:
                    if (_program.Length <= param.value)
                        Array.Resize(ref _program, (int)param.value + 1);

                    return _program[param.value];
                case ParameterMode.Immediate:
                    return param.value;
                case ParameterMode.Relative:
                    if (_program.Length < _relBase + param.value)
                        Array.Resize(ref _program, (int)param.value + 1);

                    return _program[_relBase + param.value];
                default:
                    throw new ArgumentOutOfRangeException($"Parameter mode \"{param.mode}\" is not implemented/supported");
            }
        }

        private Opcode DecodeOpcode(long pc)
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
                Console.Error.WriteLine("Warning: couldn't read all instruction arguments");
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
