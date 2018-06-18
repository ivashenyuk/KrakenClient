using MyKraken.MyKrakenAPI.Models;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace MyKraken.Helpers
{
    delegate List<OHLC> CompressDel(List<OHLC> items, int interval);
    delegate List<List<OHLC>> SplitDel(List<OHLC> source, int interval);
    delegate CandleStickSeries AddHighLow(CandleStickSeries candle, List<OHLC> oHLCs);

    public static class Helper
    {
        public static List<OHLC> CompressDate(this List<OHLC> items, int interval)
        {
            var compress = new CompressDel(CompressDel);
            return compress.Invoke(items, interval);
        }
        public static List<List<OHLC>> SplitToSublists(this List<OHLC> source, int count)
        {
            var compress = new SplitDel(SplitToSublistsDel);
            return compress.Invoke(source, count);
        }
        public static CandleStickSeries AddToHighLowSeries(this CandleStickSeries candle, List<OHLC> oHLCs)
        {
            var compress = new AddHighLow(AddToHighLowSeriesDel);
            return compress.Invoke(candle, oHLCs);
        }


        private static CandleStickSeries AddToHighLowSeriesDel(CandleStickSeries candle, List<OHLC> oHLCs)
        {
            foreach (var item1 in oHLCs)
            {
                candle.Items.Add(new HighLowItem(item1.Time, (double)item1.High, (double)item1.Low, (double)item1.Open, (double)item1.Close));
                //points.Points.Add(new DataPoint((double)item1.Time, (double)item1.Vwap));
            }
            return candle;
        }
        private static List<OHLC> CompressDel(List<OHLC> items, int interval)
        {
            var count = items.Count / interval;
            var compressedArray = new List<OHLC>(count);

            var chunks = items.SplitToSublists(interval);
            var countChuks = chunks.Count;
            for (int i = 0; i < countChuks; i++)
            {
                var tmpOHLCObjects = chunks[i];
                var tmpOHLC = tmpOHLCObjects[0];

                tmpOHLC.Close = tmpOHLCObjects[tmpOHLCObjects.Count - 1].Close;
                tmpOHLC.Low = tmpOHLCObjects.Min(o => o.Low);
                tmpOHLC.High = tmpOHLCObjects.Max(o => o.High);

                compressedArray.Add(tmpOHLC);
            }
            return compressedArray;
        }
        private static List<List<OHLC>> SplitToSublistsDel(this List<OHLC> source, int count)
        {
            return source
                     .Select((x, i) => new { Index = i, Value = x })
                     .GroupBy(x => x.Index / count)
                     .Select(x => x.Select(v => v.Value).ToList())
                     .ToList();
        }
    }
}
