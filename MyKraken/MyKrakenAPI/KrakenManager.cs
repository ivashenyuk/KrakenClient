using MyKraken.MyKrakenAPI.Exceptions;
using MyKraken.MyKrakenAPI.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MyKrakenAPI
{
    public class KrakenManager
    {
        #region Fields
        private string _hostName;
        private int _version;
        private string _apiKey;
        private string _apiSecret;
        #endregion

        #region Constructors
        public KrakenManager()
        {
            _hostName = "https://api.kraken.com";
            _version = 0;
            _apiKey = "YOUR KRAKEN KEY";
            _apiSecret = "YOUR KRAKEN SECRET";
        }
        #endregion

        #region Public Methods
        public async Task<ServerTimeResult> GetServerTime()
        {
            string res = await QueryPublic("Time");
            var ret = JsonConvert.DeserializeObject<ServerTimeResponse>(res);
            if (ret.Error.Count != 0)
                throw new KrakenException(ret.Error[0], ret);
            return ret.Result;
        }
        public async Task<string> GetTickerRealTime(List<string> pairs)
        {
            if (pairs == null || pairs.Count == 0)
                return null;

            StringBuilder pairString = new StringBuilder("pair=");

            foreach (var item in pairs)
            {
                pairString.Append(item + ",");
            }
            pairString.Length--;

            try
            {
                var url = $"{_hostName}/{_version}/public/Ticker";
                var request = WebRequest.Create(url);
                request.ContentType = "application/x-www-form-urlencoded";
                request.Method = "POST";

                using (var writer = new StreamWriter(request.GetRequestStream()))
                {
                   await writer.WriteAsync(pairString.ToString());
                }

                using (var response = await request.GetResponseAsync())
                using (var str = response.GetResponseStream())
                using (var sr = new StreamReader(str))
                {
                    var res = await sr.ReadToEndAsync();
                    return JsonConvert.SerializeObject(res);
                }
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }
        public async Task<OHLCResult> GetOHLC(string pair, int interval = 1, long since = 0)
        {
            var param = new Dictionary<string, string>();
            param.Add("pair", pair);

            param.Add("since", since.ToString());
            param.Add("interval", interval.ToString());

            var res = await QueryPublic("OHLC", param);

            JObject obj = (JObject)JsonConvert.DeserializeObject(res);
            JArray err = (JArray)obj["error"];
            if (err.Count != 0)
                throw new KrakenException(err[0].ToString(), JsonConvert.DeserializeObject<ResponseBase>(res));

            JObject result = obj["result"].Value<JObject>();

            var ret = new OHLCResult();
            ret.Pairs = new Dictionary<string, List<OHLC>>();

            foreach (var o in result)
            {
                if (o.Key == "last")
                {
                    ret.Last = o.Value.Value<long>();
                }
                else
                {
                    var ohlc = new List<OHLC>();
                    foreach (var v in o.Value.ToObject<decimal[][]>())
                        ohlc.Add(new OHLC() { Time = (int)v[0], Open = v[1], High = v[2], Low = v[3], Close = v[4], Vwap = v[5], Volume = v[6], Count = (int)v[7] });
                    ret.Pairs.Add(o.Key, ohlc);
                }
            }

            return ret;
        }
        public async Task<Dictionary<string, AssetPair>> GetAssetPairs(string info = null, string pair = null)
        {
            var param = new Dictionary<string, string>();
            if (info != null)
                param.Add("info", info);
            if (pair != null)
                param.Add("pair", pair);

            var res = await QueryPublic("AssetPairs", param);
            var ret = JsonConvert.DeserializeObject<AssetPairsResponse>(res);
            if (ret.Error.Count != 0)
                throw new KrakenException(ret.Error[0], ret);
            return ret.Result;
        }
        public async Task<string> QueryPublic(string method, Dictionary<string, string> param = null)
        {
            string address = string.Format(CultureInfo.InvariantCulture, "{0}/{1}/public/{2}", _hostName, _version, method);
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(new Uri(address));
            webRequest.ContentType = "application/x-www-form-urlencoded";
            webRequest.Method = "POST";

            string postData = BuildPostData(param);

            if (!String.IsNullOrEmpty(postData))
            {
                using (var writer = new StreamWriter(await webRequest.GetRequestStreamAsync()))
                    await writer.WriteAsync(postData);
            }

            try
            {
                using (WebResponse webResponse = await webRequest.GetResponseAsync())
                {
                    Stream str = webResponse.GetResponseStream();
                    using (StreamReader sr = new StreamReader(str))
                        return await sr.ReadToEndAsync();
                }
            }
            catch (WebException wex)
            {
                using (HttpWebResponse response = (HttpWebResponse)wex.Response)
                {
                    if (response == null)
                        throw;

                    Stream str = response.GetResponseStream();
                    using (StreamReader sr = new StreamReader(str))
                    {
                        if (response.StatusCode != HttpStatusCode.InternalServerError)
                            throw;
                        return sr.ReadToEnd();
                    }
                }
            }
        }
        #endregion

        #region Private Methods
        private string BuildPostData(Dictionary<string, string> param)
        {
            if (param == null)
                return "";

            StringBuilder b = new StringBuilder();
            foreach (var item in param)
                b.Append(string.Format("&{0}={1}", item.Key, item.Value));

            try
            {
                return b.ToString().Substring(1);
            }
            catch (Exception)
            {
                return "";
            }
        }
        #endregion
    }
}
