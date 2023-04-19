using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DxBotFunc
{
    public class SpotObject
    {
        [JsonProperty("de_call")]
        public string Call;
        public string Frequency;
        public DateTime Time;
        public string Info;
        [JsonProperty("dx_call")]
        public string DxCall;

    }
}
