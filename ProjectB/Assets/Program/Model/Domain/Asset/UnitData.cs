using Newtonsoft.Json;
using Program.Model.Domain.Asset.Base;

namespace Program.Model.Domain.Asset
{
    public class UnitData : AssetData<long>
    {
        [JsonProperty("hp")]
        public long hp { set; get; }
        [JsonProperty("ms")]
        public long moveSpeed { set; get; }
        [JsonProperty("da")]
        public long damage { set; get; }
        [JsonProperty("ra")]
        public long range { set; get; }
        [JsonProperty("as")]
        public long attackSpeed { set; get; }
        [JsonProperty("od")]
        public short order { set; get; }
    }
}
