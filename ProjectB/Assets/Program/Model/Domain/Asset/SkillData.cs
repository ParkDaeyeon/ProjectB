using Newtonsoft.Json;
using Program.Model.Domain.Asset.Base;

namespace Program.Model.Domain.Asset
{
    public class UnitSkillData : AssetData<long>
    {
        [JsonProperty("ct")]
        public float coolTime { set; get; }

        [JsonProperty("pod")]
        public long cost { set; get; }
    }
}
