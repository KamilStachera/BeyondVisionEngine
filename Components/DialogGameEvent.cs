using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using BeyondVisionEngine.Enums;
using BeyondVisionEngine.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace BeyondVisionEngine.Components
{
    public class DialogGameEvent : GameEvent
    {
        [JsonIgnore]
        public Label Label;

        public string Name;

        public List<SingleDialog> Dialogs = new List<SingleDialog>();

        public int DecisionCount = 0;

        public string InputDialog;
        public string LeftOutputDialog;
        public string RightOutputDialog;

        [JsonConverter(typeof(StringEnumConverter))]
        public DecisionResult RightDecisionResult;
        [JsonConverter(typeof(StringEnumConverter))]
        public DecisionResult LeftDecisionResult;

        public DialogGameEvent(Label label)
        {
            Label = label;
            Name = GeneralUtils.GenerateRandomName();
        }
    }
}
