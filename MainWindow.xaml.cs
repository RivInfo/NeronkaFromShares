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
using Microsoft.Win32;

namespace NeronkaFromShares
{
    public partial class MainWindow : Window
    {
        public static List<MainWindow> MainWindows = new List<MainWindow>();

        public static event Action<List<MainWindow>> UpdateMainWindowsCount;

        private string _fileName;

        private DBDataLoader _dBDataLoader;

        private DateTime _startDate = new DateTime(2000, 1, 1);
        private DateTime _endDate = DateTime.Now;

        public HistoricCandleToDB[] GetHistoricCandles => _dBDataLoader?.GetHistoricslCandles;

        public MainWindow()
        {
            InitializeComponent();

            StartData.SelectedDate = _startDate;
            EndData.SelectedDate = _endDate;
        }

        private void ButtonSelectDB(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "db files (*.db)|*.db|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                _fileName = openFileDialog.FileName;
                FileName.Text = _fileName;
            }
        }

        private void ButtonLoadData(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(MinimumValue.Text, out int minVal))
                _dBDataLoader = new DBDataLoader(_startDate, _endDate, _fileName, TableName.Text, minVal);
            else
                _dBDataLoader = new DBDataLoader(_startDate, _endDate, _fileName, TableName.Text);

            HistoricCandleToDB[] historicCandles = _dBDataLoader.GetHistoricslCandles;
            string visualText = "";
            if (historicCandles != null)
            {
                for (int i = 0; i < historicCandles.Length; i++)
                {
                    visualText += historicCandles[i].ToString() + '\n';
                }
                InfoWrite($"Загружено строк: {historicCandles.Length}");
            }
            else
            {
                InfoWrite("Что то пошло не так");
            }
            DataText.Text = visualText;            
        }

        private void MinimumDayVolumeChange(object sender, TextChangedEventArgs e)
        {
            MinimumValue.Text = MinimumValue.Text.Trim();

            if (int.TryParse(MinimumValue.Text, out int minVal))
                MinimumValue.Text = minVal.ToString();
            else
                MinimumValue.Text = 999999.ToString();
        }
        
        private void InfoWrite(string text) 
        {
            Info.Content = text;
        }

        private void StartDataSelected(object sender, SelectionChangedEventArgs e)
        {
            if(StartData.SelectedDate!=null)
                _startDate = (DateTime)StartData.SelectedDate;
            InfoWrite(_startDate.ToString("u"));
        }

        private void EndDataSelected(object sender, SelectionChangedEventArgs e)
        {
            if (EndData.SelectedDate != null)
                _endDate = (DateTime)EndData.SelectedDate;
            InfoWrite(_endDate.ToString("u"));
        }

        private void OpenMLWindow(object sender, RoutedEventArgs e)
        {
            Window1 window1 = new Window1();
            window1.Show();
        }

        private void DataWindowLoad(object sender, RoutedEventArgs e)
        {
            MainWindows.Add(this);

            Title = "Окно загрузки данных: "+ MainWindows.Count.ToString() +" - " + GetHashCode();

            UpdateMainWindowsCount?.Invoke(MainWindows);
        }

        private void DataWindowUnloadoad(object sender, RoutedEventArgs e)
        {
            MainWindows.Remove(this);
            UpdateMainWindowsCount?.Invoke(MainWindows);
        }
    }
}
