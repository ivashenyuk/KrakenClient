using MyKraken.Helpers;
using MyKraken.NewChart;
using MyKrakenAPI;
using OxyPlot.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MyKraken.View.Pages
{
    /// <summary>
    /// Interaction logic for SettingsChart.xaml
    /// </summary>
    public partial class SettingsChart : Page
    {
        public TabItem TabItem { get; private set; }
        private KrakenManager KrakenManager { get; set; } = new KrakenManager();

        public SettingsChart(TabItem tabItem)
        {
            InitializeComponent();
            TabItem = tabItem;
        }
        private async void Minutes_Click(object sender, RoutedEventArgs e)
        {
            var minutes = Convert.ToInt32((sender as Button).Content as string);
            var timeSelected = new ComboBoxItem();
            timeSelected.Content = $"{minutes} minutes";
            timeSelected.Visibility = Visibility.Collapsed;
            TimeForBars.Items.Add(timeSelected);
            TimeForBars.SelectedItem = timeSelected;
            try
            {
                var currentPlotViewer = (TabItem.Content as Grid).Tag as PlotView;
                var chartOld = currentPlotViewer.Tag as Chart;
                var pair = chartOld.Pair;

                var oHLCResult = await KrakenManager.GetOHLC(pair, 1, chartOld.EndTime);
                var listOHLC = oHLCResult.Pairs[pair];
                oHLCResult.Pairs[pair] = Helper.CompressDate(listOHLC, minutes);

                var chart = new Chart(oHLCResult, pair);
                chartOld.Model = chart.Model;
                currentPlotViewer.Model = chartOld.Model;
            }
            catch (Exception ex)
            { }
            TimeForBars.IsDropDownOpen = false;
        }
    }
}
