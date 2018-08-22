using Newtonsoft.Json;
using Program.Model.Domain.Asset.Base;

namespace Program.Model.Domain.Asset
{
    public class UnitData : AssetData<long>
    {
        [JsonProperty("na")]
        public string name { set; get; }
        [JsonProperty("hp")]
        public long hp { set; get; }
        [JsonProperty("da")]
        public long damage { set; get; }
        [JsonProperty("ms")]
        public long moveSpeed { set; get; }
        [JsonProperty("ra")]
        public long range { set; get; }
        [JsonProperty("as")]
        public float attackSpeed { set; get; }
        [JsonProperty("kp")]
        public long knockbackPower { set; get; }
    }
}
