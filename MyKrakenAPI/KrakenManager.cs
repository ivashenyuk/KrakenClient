using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MyKrakenAPI
{
    public class KrakenManager
    {
        private string _hostName;
        private int _version;
        private string _apiKey;
        private string _apiSecret;

        public KrakenManager()
        {
            _hostName = "https://api.kraken.com";
            _version = 0;
            _apiKey = "YOUR KRAKEN KEY";
            _apiSecret = "YOUR KRAKEN SECRET";
        }

        public async Task<string> GetServerTime()
        {
            try
            {
                var url = $"{_hostName}/{_version}/public/Time";
                var request = WebRequest.Create(url);

                request.ContentType = "application/json";
                request.Method = "GET";

                using (var response = await request.GetResponseAsync())
                {
                    return response.Headers["Date"].ToString();
                }
            }
            catch (WebException)
            {
                return string.Empty;
            }
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
                    writer.Write(pairString.ToString());
                }

                using (var response = await request.GetResponseAsync())
                using (var str = response.GetResponseStream())
                using (var sr = new StreamReader(str))
                {
                    var res = sr.ReadToEnd();
                    return JsonConvert.SerializeObject(res);
                }
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        public async Task<string> GetTickerOHLC(List<string> pairs)
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
                var url = $"{_hostName}/{_version}/public/OHLC";
                var request = WebRequest.Create(url);
                request.ContentType = "application/x-www-form-urlencoded";
                request.Method = "POST";
                using (var writer = new StreamWriter(request.GetRequestStream()))
                {
                    writer.Write("pair=BCHEUR");
                    writer.Write("&");
                    writer.Write("interval:(1, 5, 15, 30, 60, 240, 1440, 10080, 21600)");
                }

                using (var response = await request.GetResponseAsync())
                using (var str = response.GetResponseStream())
                using (var sr = new StreamReader(str))
                {
                    var res = sr.ReadToEnd();
                    return JsonConvert.SerializeObject(res);
                }
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }
    }
}
