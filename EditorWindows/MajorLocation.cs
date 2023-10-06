using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeyondVisionEngine.Components;

namespace BeyondVisionEngine
{
    public class MajorLocation
    {
        public string locationName;
        public List<GameEvent> EventsList = new List<GameEvent>();

        public MajorLocation(string locationName)
        {
            this.locationName = locationName;
        }
    }
}
