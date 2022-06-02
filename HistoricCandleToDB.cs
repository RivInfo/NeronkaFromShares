using System;

public struct HistoricCandleToDB
{
    public HistoricCandleToDB(/*string time*/DateTime time, float open, 
        float close, float high, float low, long volume)
    {
        Time = time;
        Open = open;
        Close = close;
        High = high;
        Low = low;
        Volume = volume;
    }

    //public string Time { get; private set; }
    public DateTime Time { get; private set; }
    public float Open { get; private set; }
    public float Close { get; private set; }
    public float High { get; private set; }
    public float Low { get; private set; }
    public long Volume { get; private set; }

    public override string ToString()
    {
        return $"{High:f2} {Time} {Open:f2} {Close:f2} {Low:f2} {Volume}";
    }
}
