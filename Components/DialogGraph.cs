using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeyondVisionEngine.Components
{
    public class DialogGraph
    {
        public List<DialogGameEvent> dialogGameEvents;

        public Tuple<int, int> positionInMajorLocation;

        public DialogGraph(Tuple<int, int> positionInMajorLocation)
        {
            dialogGameEvents = new List<DialogGameEvent>();
            this.positionInMajorLocation = positionInMajorLocation;
        }
    }
}
