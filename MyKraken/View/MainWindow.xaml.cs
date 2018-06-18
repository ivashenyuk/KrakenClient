using MyKraken.MyKrakenAPI.Models;
using MyKrakenAPI;
using OxyPlot.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using MyKraken.Helpers;
using MyKraken.NewChart;
using MyKraken.View.Pages;

namespace MyKraken
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Properties
        private NewChartWindow NewChartWindow { get; set; }
        private KrakenManager KrakenManager { get; set; } = new KrakenManager();
        private Dictionary<string, AssetPair> Currencies { get; set; }
        #endregion

        #region Constructors
        public MainWindow()
        {
            GetСurrenciesFromKrakenServer();
            InitializeComponent();
        }
        #endregion

        #region Private Methods

        #region Actions
        private void NewChart_Click(object sender, RoutedEventArgs e)
        {
            NewChartWindowOpen();
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            NewChartWindowOpen();
        }
        #endregion

        #region Methods
        private void NewChartWindowOpen()
        {
            try
            {
                if (Currencies == null)
                {
                    MessageBox.Show("Sorry, but the service is not available!");
                    return;
                }
                this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Render, new Action(() =>
                {
                    NewChartWindow = new NewChartWindow(Currencies);
                    NewChartWindow.Show();
                }));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private async void GetСurrenciesFromKrakenServer()
        {
            Currencies = await KrakenManager.GetAssetPairs();
        }
        #endregion

        #endregion


    }
}
