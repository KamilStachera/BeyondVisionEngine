using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using BeyondVisionEngine.Components;
using BeyondVisionEngine.Visuals;
using Newtonsoft.Json;
using Label = System.Windows.Controls.Label;

namespace BeyondVisionEngine
{
    public class MajorLocation
    {
        public string LocationName;

        [JsonIgnore]
        public Label Label;
        public readonly int Width;
        public readonly int Height;
        public LocationCell[][] LocationCells;
        public int ImageCount = 1;

        public Point StartPoint = new Point(-1,-1);
        public Point EndPoint = new Point(-1,-1);

        public List<DialogGraph> DialogsList = new List<DialogGraph>();
        public List<DoorConnection> DoorConnections = new List<DoorConnection>();
        public List<Point> DoorsLocationList = new List<Point>();
        public List<Point> DoorsLocationOnGrid = new List<Point>();
        public List<Enemy> Enemies = new List<Enemy>();

        public MajorLocation(string locationName, Label label, int width, int height)
        {
            LocationName = locationName;
            Label = label;
            Width = width;
            Height = height;
        }
    }
}
