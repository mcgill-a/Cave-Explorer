using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pathfinder
{
    class Program
    {
        public static List<Cavern> caverns = new List<Cavern>();

        static void Main(string[] args)
        {
            Boolean fileExists = false;
            string filename = "";
            string output = "";
            if (args.Length < 1)
            {
                Console.WriteLine("Usage: pathfinder.exe <filename>");
            }
            else
            {
                filename = args[0];

                filename = EnsureExtension(filename);
                fileExists = CheckFileExists(filename);

                if (fileExists)
                {
                    output = ProcessData(LoadFile(filename));
                    WriteResultToFile(filename, output);
                }
                else
                {
                    Console.WriteLine("Usage: pathfinder.exe <filename>");
                }
            }
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

        public static void WriteResultToFile(string filenameOriginal, string output)
        {
            string filenameOutputNoExt = RemoveExtension(filenameOriginal);
            string filenameOutput = filenameOutputNoExt + ".csn";

            bool existsAlready = CheckFileExists(filenameOutput);

            int count = 1;
            // Add counter to file if it already exists. For example: input1 (2).cav, input1 (3).cav
            while(existsAlready)
            {
                count++;
                if (count > 2)
                {
                    filenameOutputNoExt = filenameOutputNoExt.Substring(0, filenameOutputNoExt.Length - 4);
                }
                filenameOutputNoExt += " (" + count + ")";

                filenameOutput = filenameOutputNoExt + ".csn";
                existsAlready = CheckFileExists(filenameOutput);
            }

            File.WriteAllText(filenameOutput, output);
        }

        public static string RemoveExtension(string input)
        {
            return Path.GetFileNameWithoutExtension(input);
        }

        public static string ProcessData(List<int> values)
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
            //Console.WriteLine("Caverns: " + numOfCaverns + " | Coordinates: " + numOfCoordinateValues / 2 + " | Connectivity: " + numOfConnectivity);

            // Get all of the cavern coordinates
            int id_counter = 1;
            for (int i = 0; i < numOfCoordinateValues; i++)
            {
                var coordinate = new Tuple<int, int>(values[i + 1], values[i + 2]);
                caverns.Add(new Cavern { ID = id_counter, Coordinates = coordinate });
                i++;
                id_counter++;
            }

            // Read connectivity matrix
            int connectivityStart = numOfCoordinateValues + 1;
            int tracker = 1;

            for (int i = 0; i < numOfConnectivity; i++)
            {
                if (tracker > numOfCaverns)
                {
                    tracker = 1;
                }
                if (caverns[tracker-1].Connectivity != null)
                {
                    caverns[tracker-1].Connectivity.Add(values[connectivityStart + i]);
                }
                else
                {
                    caverns[tracker-1].Connectivity = new List<int> { values[connectivityStart + i] };
                }
                tracker++;
            }

            // Setup variables for A* Search
            Cavern current = null;
            Cavern start = caverns.FirstOrDefault(cav => cav.ID == 1);
            Cavern end = caverns.FirstOrDefault(cav => cav.ID == numOfCaverns);
            List<Cavern> openList = new List<Cavern>();
            List<Cavern> closedList = new List<Cavern>();
            double g = 0;
            bool found = false;

            openList.Add(start);
            while (openList.Count > 0)
            {
                // Get the cavern with the lowest F score
                double lowest = openList.Min(cav => cav.F);
                current = openList.First(cav => cav.F == lowest);

                // Add the current cavern to the closed list
                closedList.Add(current);

                // Remove it from the open list
                openList.Remove(current);

                // If the destination is added to the closed list, a path is found
                if (closedList.FirstOrDefault(cav => cav.ID == end.ID) != null)
                {
                    found = true;
                    break;
                }

                List<int> connected = new List<int>();
                for (int i = 0; i < current.Connectivity.Count; i++)
                {
                    if (current.Connectivity[i] == 1)
                    {
                        // Cavern names start at 1 not 0 so add 1
                        connected.Add(i+1);
                    }
                }
                List<Cavern> connections = GetConnectedCaverns(connected);

                foreach (Cavern cavern in connections)
                {
                    //Console.WriteLine(cavern.ID);
                    g = current.G + CalculateDistance(current.Coordinates, cavern.Coordinates);
                    // If the neighbour id is already on the closed list then ignore it
                    if (closedList.FirstOrDefault(cav => cav.ID == cavern.ID) != null)
                    {
                        //Console.WriteLine("Already explored " + cavern.ID);
                        continue;
                    }

                    // If the neighbour id is not in the open list
                    if (openList.FirstOrDefault(cav => cav.ID == cavern.ID) == null)
                    {
                        // Calculate scores & set parent cavern
                        cavern.G = g;
                        cavern.H = CalculateDistance(cavern.Coordinates, end.Coordinates);
                        cavern.F = cavern.G + cavern.H;
                        cavern.Parent = current;
                        // Add it to the open list
                        openList.Insert(0, cavern);
                    }
                    else
                    {
                        // Check if using the current G score makes the cavern F score lower
                        // If it is quicker then change the parent to current
                        if (g + cavern.H < cavern.F)
                        {
                            cavern.G = g;
                            cavern.F = cavern.G + cavern.H;
                            cavern.Parent = current;
                        }
                    }
                }
            }

            string output = "";

            if (found)
            {
                Console.WriteLine("\nPASS");
                // Follow the nodes backwards to display result
                while (current != null)
                {
                    output = current.ID + " " + output;
                    current = current.Parent;
                }
                output = output.TrimEnd(' ');
            }
            else
            {
                Console.WriteLine("\nFAIL");
                output = "0";
            }

            return output;
        }

        public static List<Cavern> GetConnectedCaverns(List<int> connectedCavernIDs)
        {
            List<Cavern> connected = new List<Cavern>();
            for (int i = 0; i < connectedCavernIDs.Count; i++)
            {
                int id = connectedCavernIDs[i] - 1;
                Cavern toAdd = caverns[id];
                connected.Add(toAdd);
            }
            return connected;
        }

        public static double CalculateDistance(Tuple<int, int> one, Tuple<int, int> two)
        {
            return Math.Sqrt(Math.Pow((two.Item1 - one.Item1), 2) + Math.Pow((two.Item2 - one.Item2), 2));
        }
    }
}