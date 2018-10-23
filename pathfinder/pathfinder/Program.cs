using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pathfinder
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("App Loaded");
            if (args.Length < 1)
            {
                Console.WriteLine("No arguments added");
            }
            else
            {
                for (int i = 0; i < args.Length; i++)
                {
                    Console.WriteLine(i + " " + args[i]);
                }
            }
            Console.ReadLine();
        }
    }
}
