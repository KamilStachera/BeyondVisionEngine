using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace BeyondVisionEngine.Enums
{
    public enum DecisionResult
    {
        [EnumMember(Value = "dialog")]
        Dialog,
        [EnumMember(Value = "hploss")]
        HpLoss,
        [EnumMember(Value = "hpgain")]
        HpGain,
        [EnumMember(Value = "key")]
        Key,
        [EnumMember(Value = "picklock")]
        Picklock,
        [EnumMember(Value = "weapon")]
        Weapon,
        [EnumMember(Value = "fight")]
        Fight,
    }
}
