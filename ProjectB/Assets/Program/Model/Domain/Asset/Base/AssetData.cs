using Newtonsoft.Json;
using Program.Core;

namespace Program.Model.Domain.Asset.Base
{
    public interface IAssetable<CodeType>
    {
        CodeType code { set; get; }
    }

    public class AssetData<CodeType> : IAssetable<CodeType>
    {
        [JsonProperty("c")]
        public CodeType code { set; get; }

        [JsonProperty("code")]
        public CodeType setCode { set { this.code = value; } }

        public override string ToString()
        {
            return ConvertString.Execute(this);
        }
    }
}
