using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyKraken.MyKrakenAPI.Models
{
    public class AssetPair
    {
        public string Altname;

        [JsonProperty(PropertyName = "aclass_base")]
        public string AclassBase;

        public string Base;

        [JsonProperty(PropertyName = "aclass_quote")]
        public string AclassQuote;

        public string Quote;

        public string Lot;

        [JsonProperty(PropertyName = "pair_decimals")]
        public int PairDecimals;

        [JsonProperty(PropertyName = "lot_decimals")]
        public int LotDecimals;

        [JsonProperty(PropertyName = "lot_multiplier")]
        public int LotMultiplier;

        [JsonProperty(PropertyName = "leverage_buy")]
        public decimal[] LeverageBuy;

        [JsonProperty(PropertyName = "leverage_sell")]
        public decimal[] LeverageSell;

        public decimal[][] Fees;

        [JsonProperty(PropertyName = "fees_maker")]
        public decimal[][] FeesMaker;

        [JsonProperty(PropertyName = "fee_volume_currency")]
        public string FeeVolumeCurrency;

        [JsonProperty(PropertyName = "margin_call")]
        public decimal MarginCall;

        [JsonProperty(PropertyName = "margin_stop")]
        public decimal MarginStop;
    }

    public class AssetPairsResponse : ResponseBase
    {
        public Dictionary<string, AssetPair> Result;
    }
}
