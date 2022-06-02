using System;
using System.Threading.Tasks;
using NeronkaFromShares.MlDataStruct;

namespace NeronkaFromShares
{
    internal class DataConvertToML
    {
        private SharisInData[] _sharisInDatas;
        private SharisInData[] _sharisInDatasWithoutHigh;

        private HistoricCandleToDB[] _historicCandles;
        private int _dayPrdiction;
        private int _dayRows;

        public DataConvertToML(HistoricCandleToDB[] historicCandles, int dayPrdiction, int dayRows)
        {
            _historicCandles = historicCandles;
            _dayPrdiction = dayPrdiction;
            _dayRows = dayRows;
        }

        public SharisInData[] GetSharisMLData => _sharisInDatas;
        public SharisInData[] GetSharisMLDataWithoutHigh => _sharisInDatasWithoutHigh;

        public bool Convert()
        {
            if (_dayPrdiction <= 0 || _dayRows <= 0)
                return false;

            _sharisInDatasWithoutHigh = new SharisInData[_dayPrdiction];
            for (int i = 0; i < _sharisInDatasWithoutHigh.Length; i++)
            {
                _sharisInDatasWithoutHigh[i] = new SharisInData();

                _sharisInDatasWithoutHigh[i].time = new DateTime[_dayRows];
                _sharisInDatasWithoutHigh[i].open = new float[_dayRows];
                _sharisInDatasWithoutHigh[i].close = new float[_dayRows];
                _sharisInDatasWithoutHigh[i].low = new float[_dayRows];
                _sharisInDatasWithoutHigh[i].volume = new long[_dayRows];

                _sharisInDatasWithoutHigh[i].high = 0;

                for (int j = 0; j < _dayRows; j++)
                {
                    _sharisInDatasWithoutHigh[i].time[j] = _historicCandles[i + j].Time;
                    _sharisInDatasWithoutHigh[i].open[j] = _historicCandles[i + j].Open;
                    _sharisInDatasWithoutHigh[i].close[j] = _historicCandles[i + j].Close;
                    _sharisInDatasWithoutHigh[i].low[j] = _historicCandles[i + j].Low;
                    _sharisInDatasWithoutHigh[i].volume[j] = _historicCandles[i + j].Volume;
                }
            }

            _sharisInDatas = new SharisInData[_historicCandles.Length - _dayPrdiction - _dayRows];
            for (int i = _dayPrdiction; i < _historicCandles.Length - _dayRows; i++)
            {
                int index = i - _dayPrdiction;

                _sharisInDatas[index] = new SharisInData();

                _sharisInDatas[index].time = new DateTime[_dayRows];
                _sharisInDatas[index].open = new float[_dayRows];
                _sharisInDatas[index].close = new float[_dayRows];
                _sharisInDatas[index].low = new float[_dayRows];
                _sharisInDatas[index].volume = new long[_dayRows];

                _sharisInDatas[index].high = _historicCandles[i - _dayPrdiction].High;

                for (int j = 0; j < _dayRows; j++)
                {
                    _sharisInDatas[index].time[j] = _historicCandles[i + j].Time;
                    _sharisInDatas[index].open[j] = _historicCandles[i + j].Open;
                    _sharisInDatas[index].close[j] = _historicCandles[i + j].Close;
                    _sharisInDatas[index].low[j] = _historicCandles[i + j].Low;
                    _sharisInDatas[index].volume[j] = _historicCandles[i + j].Volume;
                }
            }

            return true;
        }

        public async Task ConvertAsync()
        {
            await Task.Run(()=> Convert());
        }
    }
}
