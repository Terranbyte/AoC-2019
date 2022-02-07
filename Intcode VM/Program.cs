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
        static void Main(string[] args)
        {
            List<int> program = new List<int>();

            using (StreamReader sr = new StreamReader(File.Open("./program.txt", FileMode.Open)))
            {
                string input = sr.ReadToEnd();

                foreach (string s in input.Split(','))
                {
                    program.Add(Convert.ToInt32(s));
                }
            }

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
            Console.ReadLine();
        }
    }
}
