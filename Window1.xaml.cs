using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace NeronkaFromShares
{
    /// <summary>
    /// Логика взаимодействия для Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        private int _dayPrdictionValue = 1;
        private int _dataPredictionRows = 1;

        private DataConvertToML _dataTraining;

        public Window1()
        {
            InitializeComponent();
        }

        private void OpenNewWindowDataLoader(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
        }

        private void MLWindowActivated(object sender, EventArgs e)
        {
            UpdatingMainWindowsCount(MainWindow.MainWindows);
        }

        private void MlWindowLoad(object sender, RoutedEventArgs e)
        {
            MainWindow.UpdateMainWindowsCount += UpdatingMainWindowsCount;
        }

        private void MlWindowUnload(object sender, RoutedEventArgs e)
        {
            MainWindow.UpdateMainWindowsCount -= UpdatingMainWindowsCount;
        }

        private void UpdatingMainWindowsCount(List<MainWindow> obj)
        {
            string saveInfo = DataLoaderComboBox.SelectedItem?.ToString();
            DataLoaderComboBox.Items.Clear();
            for (int i = 0; i < obj.Count; i++)
            {
                DataLoaderComboBox.Items.Add(i +" " + obj[i].Title);
            }

            for (int i = 0; i < DataLoaderComboBox.Items.Count; i++)
            {
                if(DataLoaderComboBox.Items[i].ToString() == saveInfo)
                {
                    DataLoaderComboBox.SelectedIndex = i;
                    break;
                }
            }

            if(obj.Count>0 && saveInfo == null)
            {
                DataLoaderComboBox.SelectedIndex = 0;
            }

        }

        private void DayPredictionChange(object sender, TextChangedEventArgs e)
        {
            if (int.TryParse(DayPrdictionValue.Text, out int value))
            {
                _dayPrdictionValue = value;
            }
            else
            {
                MessageBox.Show(nameof(DayPrdictionValue) + " Не число!");
                DayPrdictionValue.Text = _dayPrdictionValue.ToString();
            }
        }

        private void DataPredictionRowsChange(object sender, TextChangedEventArgs e)
        {
            if (int.TryParse(DataPredictionRows.Text, out int value))
            {
                _dataPredictionRows = value;
            }
            else
            {
                MessageBox.Show(nameof(DataPredictionRows) + " Не число!");
                DataPredictionRows.Text = _dataPredictionRows.ToString();
            }
        }

        private void ConvertTrainingData(object sender, RoutedEventArgs e)
        {
            if (DataLoaderComboBox.SelectedIndex == -1)
            {
                MessageBox.Show("Неоткуда брать данные! (укажите окно загрузки данных сверху)");
                return;
            }

            _dataTraining = new DataConvertToML(MainWindow.
                MainWindows[DataLoaderComboBox.SelectedIndex].GetHistoricCandles,
                _dayPrdictionValue, _dataPredictionRows);

            PrintDataConvertid(_dataTraining);
        }

        private void ConvertTastingData(object sender, RoutedEventArgs e)
        {

        }

        private void PrintDataConvertid(DataConvertToML dataConvert)
        {
            DataText.Text = "";

            for (int i = 0; i < _dataTraining.GetSharisMLData.Length; i++)
                DataText.Text += _dataTraining.GetSharisMLData[i].ToString() + "\n";
        }
    }
}
