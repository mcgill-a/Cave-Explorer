using System;
using System.IO;
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
            Boolean valid = false;
            string filename = "";
            Console.WriteLine("App Loaded");
            if (args.Length < 1)
            {
                Console.WriteLine("No arguments added");
            }
            else
            {
                filename = args[0];
                filename = EnsureExtension(filename);
                valid = CheckFileExists(filename);
            }

            if (valid)
            {
                Console.WriteLine("File '" + filename + "' exists");
                LoadFile(filename);
            }
            else
            {
                Console.WriteLine("Please enter a valid file name");
            }
            Console.ReadLine();
        }

        public static String EnsureExtension(string name)
        {
            if (!name.ToLower().EndsWith(".cav"))
            {
                name += ".cav";
            }
            return name;
        }

        public static Boolean CheckFileExists(string filename)
        {
            if (File.Exists(filename))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static void LoadFile(string filename)
        {
            List<string> values;
            string readContent;
            using (StreamReader streamReader = new StreamReader(filename, Encoding.UTF8))
            {
                readContent = streamReader.ReadToEnd();
            }

            values = new List<string>(readContent.Split(','));

            Console.WriteLine("File Length: " + values.Count + " values");
        }
    }
}
