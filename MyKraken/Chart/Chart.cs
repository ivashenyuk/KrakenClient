using MyKraken.Helpers;
using MyKraken.MyKrakenAPI.Models;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MyKraken.NewChart
{
    public class Chart 
    {
        #region Properties
        public PlotModel Model { get; set; }
        public string Pair { get; set; }
        public OHLCResult OHLCResult { get; set; }
        public List<OHLC> OHLCListBeginer { get; }
        public int EndTime { get; set; }
        public int BeginTime { get; set; }
        #endregion

        #region Constructors
        public Chart()
        {
            Model = new PlotModel();
        }
        public Chart(OHLCResult result, string pair, int beginTime=0, int endTime=0)
        {
            Model = CreatePlotModel(result, pair);
            Pair = pair;
            OHLCResult = result;
            OHLCListBeginer = result.Pairs[pair];
            EndTime = endTime;
            BeginTime = beginTime;
        }
        #endregion

        #region Private Methods
        private PlotModel CreatePlotModel(OHLCResult result, string pair)
        {
            var tmpModel = new PlotModel { Title = "Chart", Subtitle = pair };
            //tmpModel.ZoomAllAxes(0);
            DateTimeAxis timeSPanAxis1 = new DateTimeAxis()
            {
                Position = AxisPosition.Bottom,
                MinorIntervalType = DateTimeIntervalType.Auto,
                MajorGridlineStyle = LineStyle.Dot,
                MinorGridlineStyle = LineStyle.Dot,
                MajorGridlineColor = OxyColor.FromRgb(44, 44, 44),
                TicklineColor = OxyColor.FromRgb(82, 82, 82)
            };
            tmpModel.Axes.Add(timeSPanAxis1);

            LinearAxis YAxisRight = new LinearAxis()
            {
                Position = AxisPosition.Right,
                Minimum = GetMinValueY(result, pair),
                Maximum = GetMaxValueY(result, pair),
                StartPosition = 0,
                Selectable = true,
                MajorGridlineStyle = LineStyle.Dot,
                MinorGridlineStyle = LineStyle.Dot,
                MajorGridlineColor = OxyColor.FromRgb(44, 44, 44),
                TicklineColor = OxyColor.FromRgb(82, 82, 82)
            };
            LinearAxis XAxisBottom = new LinearAxis()
            {
                Position = AxisPosition.Bottom,

                StartPosition = 0,

                MajorGridlineStyle = LineStyle.Dot,
                MinorGridlineStyle = LineStyle.Dot,
                MajorGridlineColor = OxyColor.FromRgb(44, 44, 44),
                TicklineColor = OxyColor.FromRgb(82, 82, 82)
            };
            tmpModel.Axes.Add(YAxisRight);
            tmpModel.Axes.Add(XAxisBottom);

            CandleStickSeries candle = new CandleStickSeries()
            {
                Color = OxyColors.Black,
                IncreasingColor = OxyColor.FromRgb(0, 197, 49),
                DecreasingColor = OxyColor.FromRgb(255, 95, 95),
                DataFieldX = "Time",
                DataFieldHigh = "H",
                DataFieldLow = "L",
                DataFieldClose = "C",
                DataFieldOpen = "O",
                TrackerFormatString = "Date: {2}\nOpen: {5:0.00000}\nHigh: {3:0.00000}\nLow: {4:0.00000}\nClose: {6:0.00000}",
            };

            var points = new LineSeries { Title = "Series 1", MarkerType = MarkerType.Circle };


            candle.AddToHighLowSeries(result.Pairs[pair]);
            //candle.Tag = result.Pairs[pair];

            // tmp.Series.Add(points);
            tmpModel.Series.Add(candle);

            return tmpModel;
        }
        private double GetMaxValueY(OHLCResult result, string pair)
        {
            return (double)result.Pairs[pair].Max(m => m.Low);
        }
        private double GetMinValueY(OHLCResult result, string pair)
        {
            return (double)result.Pairs[pair].Min(m => m.Low);
        }
        private double GetMaxValueX(OHLCResult result, string pair)
        {
            return (double)result.Pairs[pair].Last().Time;
        }
        private double GetMinValueX(OHLCResult result, string pair)
        {
            return (double)result.Pairs[pair].First().Time;
        }
        #endregion
    }
}
