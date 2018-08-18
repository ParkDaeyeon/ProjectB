using Ext;
using Newtonsoft.Json;

namespace Program.Model.Domain.Asset.Base
{
    public interface IOrderedAssetable<CodeType> : IAssetable<CodeType>, IReadonlyIndexable<int>
    {
        new int Index { set; get; }
    }

    public class OrderedAssetData<CodeType> : AssetData<CodeType>, IOrderedAssetable<CodeType>
    {
        // INTERFACE: IIndexable
        [JsonProperty("i")]
        public int Index { set; get; }
    }
}
