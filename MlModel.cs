using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.ML;
using NeronkaFromShares.MlDataStruct;

namespace NeronkaFromShares
{
    abstract class MlModel
    {
        public string MetricInfo { get; protected set; }

        protected MLContext _mlContext;

        protected ITransformer _model;

        public abstract void Evaluate(SharisInData[] tastingData);

        public abstract string OnePredict(SharisInData predictData);
    }
}
