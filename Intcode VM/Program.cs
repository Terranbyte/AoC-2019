using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intcode_VM
{
    internal class Program
    {
        private static List<long> program = new List<long>();

        private static void Main(string[] args)
        {
            using (StreamReader sr = new StreamReader(File.Open("./program.txt", FileMode.Open)))
            {
                string input = sr.ReadToEnd();

                foreach (string s in input.Split(','))
                {
                    program.Add(Convert.ToInt64(s));
                }
            }

            Day8Part1();
            Console.ReadLine();
        }

        private static void Day8Part1()
        {
            VirtualMachine vm = new VirtualMachine(program.ToArray(), VMSettings.Standard);

            vm.Run();
            vm.DumpMemory();
        }

        private static void Day7Part1()
        {
            bool inputThingy = true;
            long highestSignal = 0;
            int nextAlignment = 0;
            long nextSignal = 0;

            VirtualMachine vm = new VirtualMachine(program.ToArray(), VMSettings.AutomaticInput | VMSettings.NoOutput);
            vm.OnInput += () => 
            {
                inputThingy = !inputThingy;
                return inputThingy ? nextSignal : nextAlignment; 
            };
            vm.OnOutput += output => nextSignal = output;

            int[][] combos = GenerateAmplifierCombinations( new int[] { 0, 1, 2, 3, 4 } );

            for (int i = 0; i < combos.Length; i++)
            {
                //string alignmentString = $"{combo[0]}{combo[1]}{combo[2]}{combo[3]}{combo[4]}";
                Console.WriteLine($"Trying aligmnent: " + i);
                foreach (int alignment in combos[i])
                {
                    nextAlignment = alignment;
                    vm.Run();
                    vm.LoadProgram(program.ToArray());
                }

                //Console.WriteLine("Final signal strength: " + nextSignal);

                if (nextSignal > highestSignal)
                    highestSignal = nextSignal;

                // Reset for next attempt
                nextSignal = 0;
                vm.LoadProgram(program.ToArray());
            }

            Console.WriteLine("Highest signal strength: " + highestSignal);
        }

        private static async Task Day7Part2()
        {
            long highestSignal = 0;
            int[] highestSignalAlignment = null;
            long nextSignal = 0;
            bool alignmentPass = true;
            int[] alignment = null;

            VirtualMachine vmA = new VirtualMachine(program.ToArray(), VMSettings.AutomaticInput | VMSettings.NoOutput | VMSettings.AsyncOutput);
            VirtualMachine vmB = new VirtualMachine(program.ToArray(), VMSettings.AutomaticInput | VMSettings.NoOutput | VMSettings.AsyncOutput);
            VirtualMachine vmC = new VirtualMachine(program.ToArray(), VMSettings.AutomaticInput | VMSettings.NoOutput | VMSettings.AsyncOutput);
            VirtualMachine vmD = new VirtualMachine(program.ToArray(), VMSettings.AutomaticInput | VMSettings.NoOutput | VMSettings.AsyncOutput);
            VirtualMachine vmE = new VirtualMachine(program.ToArray(), VMSettings.AutomaticInput | VMSettings.NoOutput | VMSettings.AsyncOutput);

            Func<Task<long>> funcA = async () =>
            { 
                return await vmA.GetOutputAsync(); 
            };
            Func<Task<long>> funcB = async () =>
            { 
                return await vmB.GetOutputAsync(); 
            };
            Func<Task<long>> funcC = async () =>
            { 
                return await vmC.GetOutputAsync(); 
            };
            Func<Task<long>> funcD = async () =>
            { 
                return await vmD.GetOutputAsync(); 
            };
            Func<Task<long>> funcE = async () =>
            { 
                return await vmE.GetOutputAsync(); 
            };

            vmA.OnInput += () => 
            {
                long value = alignmentPass ? alignment[0] : funcE.Invoke().Result;
                alignmentPass = false;
                return value;
            };
            vmB.OnInput += () => 
            {
                long value = alignmentPass ? alignment[1] : funcA.Invoke().Result;
                alignmentPass = false;
                return value;
            };
            vmC.OnInput += () => 
            {
                long value = alignmentPass ? alignment[2] : funcB.Invoke().Result;
                alignmentPass = false;
                return value;
            };
            vmD.OnInput += () => 
            {
                long value = alignmentPass ? alignment[3] : funcC.Invoke().Result;
                alignmentPass = false;
                return value;
            };
            vmE.OnInput += () => 
            {
                long value = alignmentPass ? alignment[4] : funcD.Invoke().Result;
                alignmentPass = false;
                return value;
            };

            vmE.OnOutput += (x) => nextSignal = x;

            int[][] combos = GenerateAmplifierCombinations(new int[] { 5, 6, 7, 8, 9 });

            for (int i = 0; i < combos.Length; i++)
            {
                //string alignmentString = $"{combo[0]}{combo[1]}{combo[2]}{combo[3]}{combo[4]}";
                Console.WriteLine($"Trying aligmnent: " + i);

                alignment = combos[i];

                Task.Run(() => vmA.Run());
                Task.Run(() => vmB.Run());
                Task.Run(() => vmC.Run());
                Task.Run(() => vmD.Run());
                Task.Run(() => vmE.Run());

                while (!vmE.IsHalted) { await Task.Delay(100); };

                Console.WriteLine("Final signal strength: " + nextSignal);

                if (nextSignal > highestSignal)
                {
                    highestSignal = nextSignal;
                    highestSignalAlignment = alignment;
                }

                // Reset for next attempt
                nextSignal = 0;
                alignmentPass = true;
                vmA.LoadProgram(program.ToArray());
                vmB.LoadProgram(program.ToArray());
                vmC.LoadProgram(program.ToArray());
                vmD.LoadProgram(program.ToArray());
                vmE.LoadProgram(program.ToArray());
            }

            Console.WriteLine($"Result:\nAlignment: {highestSignalAlignment}\nSignal: {highestSignal}");
        }

        private static int[][] GenerateAmplifierCombinations(int[] initial)
        {
            List<int[]> combos = new List<int[]>();

            // This is probably the dumbest approach to generating this data but it works
            for (int i = 56789; i <= 98765; i++)
            {
                bool skip = false;
                List<int> ints = new List<int>(initial);
                string s = i.ToString().PadLeft(5, '0');

                for (int j = s.Length - 1; j >= 0; j--)
                {
                    int n = Convert.ToInt32(s[j].ToString());

                    if (!ints.Contains(n))
                    {
                        skip = true;
                        break;
                    }

                    ints.Remove(n);
                }

                if (skip)
                    continue;

                ints.Clear();

                foreach (char c in s)
                {
                    ints.Add(Convert.ToInt32(c.ToString()));
                }

                combos.Add(ints.ToArray());
            }

            return combos.Distinct().ToArray();
        }

        private static void Day5()
        {
            VirtualMachine vm = new VirtualMachine(program.ToArray());

            vm.Run();
        }

        private static void Day2()
        {
            VirtualMachine vm = null;

            for (int i = 0; i < 99; i++)
            {
                for (int j = 0; j < 99; j++)
                {
                    Console.WriteLine($"Iteration {i * 99 + j}");

                    program[1] = i;
                    program[2] = j;
                    vm = new VirtualMachine(program.ToArray());
                    vm.Run();

                    if (vm.GetMemoryValue(0) == 19690720)
                        break;
                }

                if (vm.GetMemoryValue(0) == 19690720)
                    break;
            }

            Console.WriteLine((100 * vm.GetMemoryValue(1)) + vm.GetMemoryValue(2));
        }
    }
}
