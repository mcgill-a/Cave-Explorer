using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pathfinder
{
    public class Cavern
    {
        public int ID;
        public Tuple<int, int> Coordinates;
        public List<int> Connectivity;
        public double G;
        public double H;
        public double F;
        public Cavern Parent;
    }
}
