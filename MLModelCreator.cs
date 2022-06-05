using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Trainers.FastTree;

using NeronkaFromShares.MlDataStruct;

namespace NeronkaFromShares
{
    internal class MLModelCreator : MlModel
    {
        public event Action BuildStart;
        public event Action BuildEnd;

        public MLModelCreator()
        {
            _mlContext = new MLContext(seed: 0);
        }

        public async Task BuildModelAsync(SharisInData[] trainingData)
        {
            BuildStart?.Invoke();
            await Task.Run(()=> BuildModel(trainingData));
            BuildEnd?.Invoke();
        }

        public void BuildModel(SharisInData[] trainingData)
        {
            IDataView trainingDataView = _mlContext.Data.LoadFromEnumerable(trainingData, 
                CreateSchema(trainingData[0].open.Length));

            var dataProcessPipeline = _mlContext.Transforms.CopyColumns("Label", nameof(SharisInData.high))
                .Append(_mlContext.Transforms.NormalizeMeanVariance("Fetch1", nameof(SharisInData.low)))
                .Append(_mlContext.Transforms.NormalizeMeanVariance("Fetch2", nameof(SharisInData.open)))
                .Append(_mlContext.Transforms.NormalizeMeanVariance("Fetch3", nameof(SharisInData.close)))
                .Append(_mlContext.Transforms.Conversion.ConvertType("Fetch4", nameof(SharisInData.volume)))
                .Append(_mlContext.Transforms.Conversion.ConvertType("Fetch5", nameof(SharisInData.time)))

                .Append(_mlContext.Transforms.NormalizeMinMax("Fetch4", "Fetch4"))

                .Append(_mlContext.Transforms.Concatenate("Features", "Fetch1",
                "Fetch2", "Fetch3", "Fetch4", "Fetch5"));
                //.Append(_mlContext.Transforms.Concatenate("Features", nameof(SharisInData.low),
                //nameof(SharisInData.open), nameof(SharisInData.close), "Fetch4", "Fetch5"));


            FastTreeRegressionTrainer.Options options
                = new FastTreeRegressionTrainer.Options();

            //FastTreeTweedieTrainer.Options options
            //    = new FastTreeTweedieTrainer.Options();

            var trainer = _mlContext.Regression.Trainers.FastTree(options);

            //var trainer = _mlContext.Regression.Trainers.FastTreeTweedie(options);

            var trainingPipeline = dataProcessPipeline.Append(trainer);

            _model = trainingPipeline.Fit(trainingDataView);

        }

        public override void Evaluate(SharisInData[] tastingData)
        {
            IDataView testingDataView = _mlContext.Data.LoadFromEnumerable(tastingData,
                CreateSchema(tastingData[0].open.Length));

            IDataView predictions = _model.Transform(testingDataView);

            var metrics = _mlContext.Regression.Evaluate(predictions);

            MetricInfo = "";
            MetricInfo += $"{nameof(metrics.LossFunction)} - {metrics.LossFunction}\n";
            MetricInfo += $"{nameof(metrics.RSquared)} - {metrics.RSquared}\n";
            MetricInfo += $"{nameof(metrics.MeanSquaredError)} - {metrics.MeanSquaredError}\n";
            MetricInfo += $"{nameof(metrics.MeanAbsoluteError)} - {metrics.MeanAbsoluteError}\n";
            MetricInfo += $"{nameof(metrics.RootMeanSquaredError)} - {metrics.RootMeanSquaredError}\n";
        }

        public override string OnePredict(SharisInData predictData)
        {
            var predEngine = _mlContext.Model.CreatePredictionEngine<SharisInData, SharesOut>(_model, false,
                CreateSchema(predictData.open.Length));

            var resultprediction = predEngine.Predict(predictData);

            return resultprediction.HighAmount.ToString();
        }

        private static SchemaDefinition CreateSchema(int length)
        {
            SchemaDefinition autoSchema = SchemaDefinition.Create(typeof(SharisInData));
            for (int i = 0; i < 5; i++)
            {
                var featureColumn = autoSchema[i];
                var itemType = ((VectorDataViewType)featureColumn.ColumnType).ItemType;
                featureColumn.ColumnType = new VectorDataViewType(itemType, length);
            }

            return autoSchema;
        }
    }
}

//FastTreeRegressionTrainer.Options options
//    = new FastTreeRegressionTrainer.Options()
//    {
//        AllowEmptyTrees = false,
//        BaggingExampleFraction = 0.7,
//        BaggingSize = 0,
//        BestStepRankingRegressionTrees = true,
//        Bias = 0.3,
//        MaximumBinCountPerFeature = 3,
//        CategoricalSplit = false,
//        CompressEnsemble = true,
//        DropoutRate = 0.1,
//        EnablePruning = true,
//        EntropyCoefficient = 0.5
//    };

/*{
                   NumberOfLeaves = 98,
                   MinimumExampleCountPerLeaf = 10,
                   NumberOfTrees = 500,
                   LearningRate = 0.07655732f,
                   Shrinkage = 0.2687001f
               }*/
