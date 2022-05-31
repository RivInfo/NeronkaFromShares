using System;
using NeronkaFromShares.MlDataStruct;

namespace NeronkaFromShares
{
    internal class DataConvertToML
    {
        private SharisInData[] _sharisInDatas;

        public DataConvertToML(HistoricCandleToDB[] historicCandles, int dayPrdiction, int dayRows)
        {
            _sharisInDatas = new SharisInData[historicCandles.Length - dayPrdiction - dayRows];
            for (int i = dayPrdiction; i < historicCandles.Length-dayRows; i++)
            {
                int index = i - dayPrdiction;

                _sharisInDatas[index] = new SharisInData();

                _sharisInDatas[index].time = new DateTime[dayRows];
                _sharisInDatas[index].open = new float[dayRows];
                _sharisInDatas[index].close = new float[dayRows];
                _sharisInDatas[index].low = new float[dayRows];
                _sharisInDatas[index].volume = new long[dayRows];

                _sharisInDatas[index].high = historicCandles[i- dayPrdiction].High;

                for (int j = 0; j < dayRows; j++)
                {
                    _sharisInDatas[index].time[j] = historicCandles[i + j].Time;
                    _sharisInDatas[index].open[j] = historicCandles[i + j].Open;
                    _sharisInDatas[index].close[j] = historicCandles[i + j].Close;
                    _sharisInDatas[index].low[j] = historicCandles[i + j].Low;
                    _sharisInDatas[index].volume[j] = historicCandles[i + j].Volume;
                }               
            }
        }

        public SharisInData[] GetSharisMLData => _sharisInDatas;
    }
}
