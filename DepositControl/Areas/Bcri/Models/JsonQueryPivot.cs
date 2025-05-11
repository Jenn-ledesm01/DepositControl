using System.Collections.Generic;
using Newtonsoft.Json;

namespace DepositControl.Areas.Bcri.Models
{
    [JsonObject]
    public class JsonQueryPivot
    {
        [JsonProperty("hiddenAttributes")]
        public List<object> HiddenAttributes { get; set; }

        [JsonProperty("menuLimit")]
        public int MenuLimit { get; set; }

        [JsonProperty("cols")]
        public List<object> Cols { get; set; }

        [JsonProperty("rows")]
        public List<object> Rows { get; set; }

        [JsonProperty("vals")]
        public List<object> Vals { get; set; }

        [JsonProperty("exclusions")]
        public Dictionary<string, List<object>> Exclusions { get; set; }

        [JsonProperty("inclusions")]
        public Dictionary<string, List<object>> Inclusions { get; set; }

        [JsonProperty("unusedAttrsVertical")]
        public int UnusedAttrsVertical { get; set; }

        [JsonProperty("autoSortUnusedAttrs")]
        public bool AutoSortUnusedAttrs { get; set; }

        [JsonProperty("inclusionsInfo")]
        public Dictionary<string, List<object>> InclusionsInfo { get; set; }

        [JsonProperty("aggregatorName")]
        public string AggregatorName { get; set; }

        [JsonProperty("rendererName")]
        public string RendererName { get; set; }
    }
}

