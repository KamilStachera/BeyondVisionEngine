using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeyondVisionEngine.Components
{
    public class Enemy
    {
        public int Hp { get; set; } = 0;
        public int Attack { get; set; } = 0;
        public Tuple<int, int> Position { get; set; }
    }
}
