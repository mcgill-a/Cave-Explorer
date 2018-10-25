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
                Console.WriteLine("No arguments added. File set to 'input1.cav'");
                filename = "input1.cav";
            }
            else
            {
                filename = args[0];
            }

            filename = EnsureExtension(filename);
            valid = CheckFileExists(filename);

            if (valid)
            {
                Console.WriteLine("File '" + filename + "' exists");
                ProcessData(LoadFile(filename));
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

        public static List<int> LoadFile(string filename)
        {
            List<string> values;
            string readContent;
            using (StreamReader streamReader = new StreamReader(filename, Encoding.UTF8))
            {
                readContent = streamReader.ReadToEnd();
            }

            values = new List<string>(readContent.Split(','));
            List <int> intValues = values.ConvertAll(int.Parse);
            return intValues;
        }

        public static void ProcessData(List<int> values)
        {
            int numOfCaverns = 0;
            int numOfCoordinateValues = 0;
            int numOfConnectivity = 0;
            
            if (values.Count > 0)
            {
                numOfCaverns = values[0];
                numOfCoordinateValues = numOfCaverns * 2;
                numOfConnectivity = numOfCaverns * numOfCaverns;
            }
            Console.WriteLine("Caverns: " + numOfCaverns + " | Coordinates: " + numOfCoordinateValues / 2 + " | Connectivity: " + numOfConnectivity);

            // First cavern is always start point
            // Last cavern is always end point
            int cavernStart = 1;
            int cavernEnd = numOfCaverns;
            Console.WriteLine("Start Cavern: " + cavernStart + " | End Cavern: " + cavernEnd);
            
            // Get all of the cavern coordinates
            var coordinates = new List<Tuple<int, int>>();
            for (int i = 0; i < numOfCoordinateValues; i++)
            {
                var coordinate = new Tuple<int, int>(values[i + 1], values[i + 2]);
                coordinates.Add(coordinate);
                i++;
            }

            // Print all of the cavern coordinates
            string strCoords = "";
            foreach (Tuple<int, int> coordinate in coordinates)
            {
                strCoords += coordinate.ToString() + " ";
            }
            Console.WriteLine(strCoords);


            // print out cavern x connectivity
            int connectivityStart = numOfCoordinateValues +1;

            string connectivityMatrixRow = "";
            int count = 0;
            int rows = 0;

            Console.WriteLine("\nCavern Connectivity Matrix:\n");
            for (int i = 0; i < numOfConnectivity; i++)
            {
                if (rows < numOfCaverns)
                {
                    if (count < numOfCaverns)
                    {
                        connectivityMatrixRow += values[connectivityStart + i] + " ";
                        if (count == numOfCaverns - 1)
                        {
                            Console.WriteLine(connectivityMatrixRow);
                            rows++;
                        }
                        count++;
                    }
                    else
                    {
                        connectivityMatrixRow = values[connectivityStart + i] + " ";
                        count = 1;
                    }
                }
                else
                {
                    break;
                }
            }
        }
    }
}
