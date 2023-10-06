using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace BeyondVisionEngine.Visuals
{
    public class LocationCell
    {
        [JsonIgnore]
        public double StartWidth { get; set; }
        [JsonIgnore]
        public double StartHeight { get; set; }
        [JsonIgnore]
        public double Width { get; set; }
        [JsonIgnore]
        public double Height { get; set; }

        public bool IsCollision { get; set; } = false;
        public bool IsDoors { get; set; } = false;
        public bool IsKey { get; set; } = false;
        public bool IsEnemy { get; set; } = false;

        public bool HasDialog { get; set; } = false;

        public LocationCell(double startWidth, double startHeight, double width, double height)
        {
            StartWidth = startWidth;
            StartHeight = startHeight;
            Width = width;
            Height = height;
        }


        public bool IsFilled()
        {
            return IsDoors || IsKey || IsCollision || HasDialog || IsEnemy;
        }

        public bool IsEmpty()
        {
            return !IsDoors && !IsKey && IsCollision && !HasDialog && !IsEnemy;
        }

        public void ClearCell()
        {
            IsKey = false;
            IsDoors = false;
            IsCollision = false;
            HasDialog = false;
            IsEnemy = false;
        }

    }
}
