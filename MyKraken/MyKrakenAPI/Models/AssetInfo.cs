using Newtonsoft.Json;
using System.Collections.Generic;

namespace MyKraken.MyKrakenAPI.Models
{
    public class AssetInfo
    {
        public string Altname;
        public string Aclass;
        public int Decimals;
        
        [JsonProperty(PropertyName = "display_decimals ")]
        public int DisplayDecimals;
    }
    public class AssetInfoResponse : ResponseBase
    {
        public Dictionary<string, AssetInfo> Result;
    }
}
