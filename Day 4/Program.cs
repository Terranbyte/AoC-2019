using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Day_4
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Part2();
            Console.ReadLine();
        }

        static void Part1()
        {
            int possiblePasswords = 0;

            for (int i = 136760; i < 595730; i++)
            {
                string password = i.ToString();
                int prevNum = 0;
                bool skip = false;

                // Going left to right, any singular digit cannot decrease
                for (int j = 0; j < password.Length; j++)
                {
                    int current = Convert.ToInt32(password[j]);

                    if (current < prevNum)
                    {
                        skip = true;
                        break;
                    }

                    prevNum = current;
                }

                if (skip)
                    continue;

                skip = true;

                // Two adjacent numbers must match
                for (int j = 0; j < password.Length - 1; j++)
                {
                    if (password[j] == password[j + 1])
                        skip = false;
                }

                if (skip)
                    continue;

                possiblePasswords += 1;
            }

            Console.WriteLine(possiblePasswords);
        }

        static void Part2()
        {
            int possiblePasswords = 0;

            for (int i = 136760; i < 595730; i++)
            {
                string password = i.ToString();

                if (TestPassword(password))
                    possiblePasswords += 1;
            }

            Console.WriteLine(possiblePasswords);
        }

        static bool TestPassword(string password)
        {

            int prevNum = 0;
            bool skip = false;

            // Going left to right, any singular digit cannot decrease
            for (int j = 0; j < password.Length; j++)
            {
                int current = Convert.ToInt32(password[j]);

                if (current < prevNum)
                {
                    skip = true;
                    break;
                }

                prevNum = current;
            }

            if (skip)
                return false;

            skip = false;

            // Two adjacent numbers must match
            for (int j = 1; j < password.Length; j++)
            {
                if (password.Count(c => c == password[j]) == 2)
                    skip = true;
            }

            return skip;
        }
    }
}


// 122456