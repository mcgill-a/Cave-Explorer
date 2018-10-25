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


            // Read connectivity matrix
            int connectivityStart = numOfCoordinateValues +1;

            string connectivityMatrixRow = "";
            int count = 0;
            int rows = 0;

            //Create dictionary to store caverns and their connectivity
            Dictionary<int, List<int>> caverns = new Dictionary<int, List<int>>();
            List<int> connectivity = new List<int>();

            Console.WriteLine("\nCavern Connectivity Matrix:\n");
            for (int i = 0; i < numOfConnectivity; i++)
            {
                if (rows < numOfCaverns)
                {
                    if (count < numOfCaverns)
                    {
                        connectivityMatrixRow += values[connectivityStart + i] + " ";
                        connectivity.Add(values[connectivityStart + i]);
                        if (count == numOfCaverns - 1)
                        {
                            //Console.WriteLine(connectivityMatrixRow);
                            rows++;
                            caverns.Add(rows, connectivity);
                        }
                        count++;
                    }
                    else
                    {
                        connectivity = new List<int>();
                        connectivityMatrixRow = values[connectivityStart + i] + " ";
                        count = 1;
                        connectivity.Add(values[connectivityStart + i]);
                    }
                }
                else
                {
                    break;
                }
            }

            // Print connectivity matrix using the dictionary key values
            int counter = 0;
            foreach (var item in caverns.Values)
            {
                string line = "";
                for (int i = 0; i < item.Count; i++)
                {
                    line += item[i] + " ";
                }
                counter++;
                if (counter < 10)
                {
                    Console.WriteLine("0" + counter + " >> " + line);
                }
                else
                {
                    Console.WriteLine(counter + " >> " + line);
                }
            }

            Console.WriteLine("");

            Dictionary<int, List<int>> openList = new Dictionary<int, List<int>>();

            // For testing purposes
            int currentCavern = 3;

            openList.Add(currentCavern, caverns[currentCavern]);

            List<int> closedList = new List<int>();
            bool connectionFound = false;
            while(openList.Count > 0)
            {
                // get the lowest scoring node in openlist (calculate distance between start point and valid points)
                int lowestCavern = 0;
                double lowestDistance = double.MaxValue;
                double distance = 0;
                //List<int> current = openList[1];
                for (int i = 0; i < caverns[1].Count; i++)
                {
                    Tuple<int, int> start = coordinates[currentCavern - 1];
                    if (caverns[currentCavern][i].Equals(1))
                    {
                        connectionFound = true;
                        Tuple<int, int> destination = coordinates[i];
                        distance = CalculateDistance(start, destination);
                        Console.WriteLine("Distance between: " + start.ToString() + " and " + destination.ToString() + " : " + distance);

                        if (distance < lowestDistance)
                        {
                            lowestDistance = distance;
                            lowestCavern = i +1;
                        }
                    }
                }
                if (connectionFound)
                {
                    Console.WriteLine("\nClosest Cavern (" + currentCavern + "): " + (lowestCavern) + " | Distance: " + lowestDistance);
                }
                openList.Remove(currentCavern);
            }
        }

        public static double CalculateDistance(Tuple<int, int> one, Tuple<int, int> two)
        {
            double distance = Math.Sqrt(Math.Pow((two.Item1 - one.Item1), 2) + Math.Pow((two.Item2 - one.Item2), 2));
            return distance;
        }
    }
}
