using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
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

        private int _rowPredictionIndex = 0;

        private DataConvertToML _dataTraining;
        private DataConvertToML _dataTesting;

        private MlModel _mlModel;

        private DateTime _timeModelCreatidStart;
        private DateTime _timeModelCreatidEnd;

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

        private async void ConvertTrainingData(object sender, RoutedEventArgs e)
        {
            /*await*/ DataConvertLoad(ref _dataTraining, "Тренировачных");
        }

        private /*async*/ void ConvertTastingData(object sender, RoutedEventArgs e)
        {
            /*await*/ DataConvertLoad(ref _dataTesting, "Тестовых");
        }

        private /*async Task*/ void DataConvertLoad(ref DataConvertToML dataConvert, string dataName)
        {
            if (DataLoaderComboBox.SelectedIndex == -1)
            {
                MessageBox.Show("Неоткуда брать данные! (укажите окно загрузки данных сверху)");
                dataConvert = null;
                return;
            }

            PrintLog($"Начата обработка {dataName} данных");

            dataConvert = new DataConvertToML(MainWindow.
                    MainWindows[DataLoaderComboBox.SelectedIndex].GetHistoricCandles,
                    _dayPrdictionValue, _dataPredictionRows);

            /*await*/
            if (dataConvert.Convert())
            {
                PrintLog($"Обработка {dataName} данных окончена: {dataConvert.GetSharisMLData.Length}");

                PrintDataConvertid(dataConvert);
            }
            else
            {
                PrintLog($"Что-то пошло не так! Обработка {dataName} данных не завершена.");
                PrintLog("Число дней должно быть положительным.");
            }
        }

        private void PrintDataConvertid(DataConvertToML dataConvert, int rows = 100)
        {
            if (dataConvert == null)
            {
                return;
            }

            DataText.Text = "";

            StringBuilder stringBuilder = new StringBuilder();

            int lengthOut = rows;
            if (rows == -1)
                lengthOut = dataConvert.GetSharisMLData.Length;

            if (lengthOut > dataConvert.GetSharisMLData.Length)
                lengthOut = dataConvert.GetSharisMLData.Length;

            for (int i = 0; i < dataConvert.GetSharisMLDataWithoutHigh.Length; i++)
                stringBuilder.Append($"{i:d2} "+
                    dataConvert.GetSharisMLDataWithoutHigh[i].ToString() + "\n");

            for (int i = 0; i < lengthOut; i++)
                stringBuilder.Append($"{i+ dataConvert.GetSharisMLDataWithoutHigh.Length:d2} " 
                    + dataConvert.GetSharisMLData[i].ToString() + "\n");

            DataText.Text = stringBuilder.ToString();
        }

        private async void BuildModel(object sender, RoutedEventArgs e)
        {
            if (_dataTraining == null)
                return;

            MLModelCreator mlModelCreate = new MLModelCreator();
            mlModelCreate.BuildStart += MlModelCreate_BuildStart;
            mlModelCreate.BuildEnd += MlModelCreate_BuildEnd;

            await mlModelCreate.BuildModelAsync(_dataTraining.GetSharisMLData);

            _mlModel = mlModelCreate;

            mlModelCreate.BuildStart -= MlModelCreate_BuildStart;
            mlModelCreate.BuildEnd -= MlModelCreate_BuildEnd;
        }

        private void MlModelCreate_BuildStart()
        {
            _timeModelCreatidStart = DateTime.Now;
            PrintLog("Старт сборки модели \n");
        }

        private void MlModelCreate_BuildEnd()
        {
            _timeModelCreatidEnd = DateTime.Now;
            TimeSpan timeSpan = _timeModelCreatidEnd - _timeModelCreatidStart;

            PrintLog("Cборки модели завершена! Время сборки: "+ timeSpan.ToString("c"));
        }

        private void EvaluateModel(object sender, RoutedEventArgs e)
        {
            if (_dataTesting == null)
                return;

            _mlModel.Evaluate(_dataTesting.GetSharisMLData);

            PrintLog(_mlModel.MetricInfo);
        }

        private void PrintLog(string logText)
        {
            OutText.Text += logText + '\n';
        }

        private void Prediction(object sender, RoutedEventArgs e)
        {
            if (_mlModel == null)
                return;
            if (_rowPredictionIndex < 0)
                return;
            if (_dataTesting==null)
                return;
            if (_rowPredictionIndex > _dataTesting.GetSharisMLData.Length +
                _dataTesting.GetSharisMLDataWithoutHigh.Length)
                return;


            if (_rowPredictionIndex< _dataTesting.GetSharisMLDataWithoutHigh.Length)
            {
                PrintLog($"{_mlModel.OnePredict(_dataTesting.GetSharisMLDataWithoutHigh[_rowPredictionIndex])}");
            }
            else
            {
                PrintLog(_mlModel.OnePredict(_dataTesting.
                    GetSharisMLData[_rowPredictionIndex - _dataTesting.GetSharisMLDataWithoutHigh.Length])
                    + " " + _dataTesting.
                    GetSharisMLData[_rowPredictionIndex - _dataTesting.GetSharisMLDataWithoutHigh.Length].high);
            }           
        }

        private void RowPredictionIndexChange(object sender, TextChangedEventArgs e)
        {
            if (int.TryParse(RowIndex.Text, out int value))
            {
                _rowPredictionIndex = value;
            }
            else
            {
                MessageBox.Show(nameof(_rowPredictionIndex) + " Не число!");
                RowIndex.Text = _rowPredictionIndex.ToString();
            }
        }
    }
}
