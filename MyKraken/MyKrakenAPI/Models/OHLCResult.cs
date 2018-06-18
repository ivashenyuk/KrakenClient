using System.Collections.Generic;
using System;

namespace MyKraken.MyKrakenAPI.Models
{
    public class OHLCResult 
    {
        public Dictionary<string, List<OHLC>> Pairs;
        public long Last;

        public object Clone()
        {
            var pairs = new Dictionary<string, List<OHLC>>();

            foreach (var pair in Pairs)
            {
                var key = pair.Key;
                var values = new List<OHLC>();
                foreach (var p in pair.Value)
                {
                    values.Add(p.Clone() as OHLC);
                }
                pairs.Add(key, values);
            }

            return new OHLCResult
            {
                Last = this.Last,
                Pairs = pairs
            };
        }
    }
    public class OHLC : ICloneable
    {
        public int Time;
        public decimal Open;
        public decimal High;
        public decimal Low;
        public decimal Close;
        public decimal Vwap;
        public decimal Volume;
        public int Count;

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
