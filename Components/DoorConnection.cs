using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeyondVisionEngine.Components
{
    public class DoorConnection
    {
        public Tuple<int,int> location;
        public bool IsSource;
        public string Destination;
        public string Source;
    }
}
