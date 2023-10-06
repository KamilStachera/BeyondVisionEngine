using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeyondVisionEngine.Utils
{
    public static class GeneralUtils
    {
        private static readonly Random random = new Random();
        private const string _chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        /// <summary>
        /// Function for generating random string, default length -> 10
        /// </summary>
        /// <returns>Random string</returns>
        public static string GenerateRandomName(int length = 10)
        {
            return new string(Enumerable.Repeat(_chars, length).Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
