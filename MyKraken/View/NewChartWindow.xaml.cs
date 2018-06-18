using MyKraken.NewChart;
using MyKrakenAPI;
using OxyPlot.Wpf;
using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System;
using MyKraken.Enums;
using MyKraken.MyKrakenAPI.Models;
using System.Collections.Generic;
using System.Linq;
using MyKraken.Helpers;
using MyKraken.View.Pages;

namespace MyKraken
{
    /// <summary>
    /// Interaction logic for NewChartWindow.xaml
    /// </summary>
    public partial class NewChartWindow : Window
    {
        #region Properties
        private KrakenManager KrakenManager { get; set; }
        #endregion

        #region Constructors
        public NewChartWindow(Dictionary<string, AssetPair> currencies)
        {
            KrakenManager = new KrakenManager();

            InitializeComponent();

            SetDefaultValue(currencies);
        }
        #endregion

        #region Private Methods

        #region Methods
        private void SetDefaultValue(Dictionary<string, AssetPair> currencies)
        {
            SetDefaultValuesForTypes();
            SetDefaultValuesForСurrency(currencies);
        }
        private void SetDefaultValuesForСurrency(Dictionary<string, AssetPair> currencies)
        {
            Сurrency.Items.Clear();
            foreach (var currency in currencies)
            {
                var boxItem = new ComboBoxItem();
                boxItem.Content = currency.Key;
                Сurrency.Items.Add(boxItem);
            }
            Сurrency.SelectedIndex = 0;
        }
        private void SetDefaultValuesForTypes()
        {
            var types = Enum.GetValues(typeof(Types)) as IList;
            foreach (var type in types)
            {
                var boxItem = new ComboBoxItem();
                boxItem.Content = type;
                boxItem.Tag = type;
                TypeValues.Items.Add(boxItem);
            }
        }
        private void AddChartToForm(Chart chart)
        {
            this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Render, new Action(()=> {
                var mainWindow = (MainWindow)App.Current.MainWindow;
                var tabItem = new TabItem();

                tabItem.Header = (this.DropDownInstruments.SelectedItem as ComboBoxItem).Name;

                var plotViewr = new PlotView
                {
                    Model = chart.Model,
                    Tag = chart
                };

                var settingsView = new SettingsChart(tabItem);
                var frameForSettingsView = new Frame()
                {
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Left
                };

                var grid = new Grid();
                grid.Tag = plotViewr;
                grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(30) });
                grid.RowDefinitions.Add(new RowDefinition());

                frameForSettingsView.Content = settingsView;

                grid.Children.Add(frameForSettingsView);
                grid.Children.Add(plotViewr);

                Grid.SetRow(frameForSettingsView, 0);
                Grid.SetRow(plotViewr, 1);


                tabItem.Content = grid;

                mainWindow.TabCharts.Items.Insert(mainWindow.TabCharts.Items.Count - 1, tabItem);
                mainWindow.TabCharts.SelectedIndex = mainWindow.TabCharts.Items.Count - 2;
            }));
        }
        private async void CreateChart()
        {
            var pair = (Сurrency.SelectedItem as ComboBoxItem).Content.ToString();

            if (string.IsNullOrEmpty(Interval.Text))
                return;

            var interval = Convert.ToInt32(Interval.Text);

            if (string.IsNullOrEmpty(pair) || interval < 1)
                return;

            var result = new OHLCResult();
            var endTime = DatePickerEnd.SelectedDate.Value.Millisecond;
            if (!Enum.IsDefined(typeof(Minutes), interval))
            {
                result = await KrakenManager.GetOHLC(pair, 1, endTime);
                result.Pairs[pair] = Helper.CompressDate(result.Pairs[pair], interval);
            }
            else
            {
                result = await KrakenManager.GetOHLC(pair, interval, endTime);
            }
            var chart = new Chart(result, pair, endTime: endTime);

            AddChartToForm(chart);
        }
        #endregion

        #region Actions
        private void OkBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Background, new Action(CreateChart));
                //CreateChart();
                this.Close();
            }
            catch (Exception ex)
            { MessageBox.Show(ex.Message); }
        }
        private void DropDownInstruments_DataContextChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = sender as ComboBox;
            var it = (item?.SelectedItem as ComboBoxItem);
            ListDropDownInstruments?.Items?.Add(it?.Name);
        }
        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedItem = (TypeValues.SelectedItem as ComboBoxItem).Tag.ToString();
            if (selectedItem == Types.Minute.ToString()) ;

        }
        private void LoadDataBasedOn_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.IsLoaded)
            {
                if (LoadDataBasedOn.SelectedIndex == 0)
                {
                    TextBoxToLoad.Text = "Bars to load";
                    DatePickerStart.Visibility = Visibility.Hidden;
                    ValueToLoadData.Visibility = Visibility.Visible;
                }
                else if (LoadDataBasedOn.SelectedIndex == 1)
                {
                    TextBoxToLoad.Text = "Days to load";
                    DatePickerStart.Visibility = Visibility.Hidden;
                    ValueToLoadData.Visibility = Visibility.Visible;
                }
                else if (LoadDataBasedOn.SelectedIndex == 2)
                {
                    TextBoxToLoad.Text = "Start date";
                    ValueToLoadData.Visibility = Visibility.Hidden;
                    DatePickerStart.Visibility = Visibility.Visible;

                }
            }
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DatePickerStart.SelectedDate = DateTime.Now.Date;
            DatePickerEnd.SelectedDate = DateTime.Now.Date.AddDays(-1);
        }
        #endregion

        #endregion  
    }
}
