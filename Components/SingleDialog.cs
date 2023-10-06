using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Newtonsoft.Json;

namespace BeyondVisionEngine.Components
{
    public class SingleDialog
    {
        public string Text;
        public string Voice;

        [JsonIgnore]
        public TextBox TextBox;
        [JsonIgnore]
        public Button DeleteButton;

        public bool IsLeftDecision;
        public bool IsRightDecision;

        public SingleDialog(string text)
        {
            Text = text;
        }

    }
}
