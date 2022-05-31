using System;
using System.Text;
using Microsoft.ML.Data;

namespace NeronkaFromShares.MlDataStruct
{
    public class SharisInData //DateTime time, float open, float close, float high, float low, long volume
    {
        public DateTime[] time;

        public float[] open;

        public float[] close;

        public float high;

        public float[] low;

        public long[] volume;


        public override string ToString()
        {
            StringBuilder combo = new StringBuilder();
            for (int i = 0; i < open.Length; i++)
                combo.Append(open[i]+" ");
            for (int i = 0; i < close.Length; i++)
                combo.Append(close[i] + " ");
            for (int i = 0; i < low.Length; i++)
                combo.Append(low[i] + " ");
            for (int i = 0; i < volume.Length; i++)
                combo.Append(volume[i] + " ");

            combo.Append(time[0].ToString() + " ");
            combo.Append(time[time.Length-1].ToString());

            return $"{high} {combo}";
        }
    }
}
