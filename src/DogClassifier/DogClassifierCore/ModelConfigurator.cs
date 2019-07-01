using Microsoft.ML;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace DogClassifierCore
{
    public class ModelConfigurator
    {
        private readonly MLContext _mlContext;
        private static readonly string TensorFlowModelBasePath = Path.Combine(Environment.CurrentDirectory, "TensorFlowModel");
        private static readonly string TensorFlowModelLocation = Path.Combine(TensorFlowModelBasePath, "model.pb");
        private static readonly string TensorFlowLabelsLocation = Path.Combine(TensorFlowModelBasePath, "labels.txt");

        private static string[] _labels;
        private ITransformer _mlModel;
        private readonly PredictionEngine<InputImage, PredictionResult> _predictionEngine;

        public ModelConfigurator(MLContext mlContext)
        {
            _mlContext = mlContext;
            _mlModel = CreateMlModel();
            _predictionEngine = mlContext.Model.CreatePredictionEngine<InputImage, PredictionResult>(_mlModel);
            _labels = File.ReadAllLines(TensorFlowLabelsLocation);
        }


        public string Predict(InputImage inputImage)
        {
            var scores = _predictionEngine.Predict(inputImage).Scores;
            var best =  scores.Max();
            var bestScore = best.ToString("F5");

            var resultText = $"{_labels[scores.AsSpan().IndexOf(best)]}: {bestScore}";
            return resultText;
        }


        private ITransformer CreateMlModel()
        {
            var pipeline = _mlContext.Transforms.ResizeImages(
                    outputColumnName: TensorFlowModelSettings.InputColumnName,
                    imageWidth: TensorFlowModelSettings.InputImageWidth,
                    imageHeight: TensorFlowModelSettings.InputImageHeight,
                    inputColumnName: nameof(InputImage.Image))
                .Append(_mlContext.Transforms.ExtractPixels(
                    outputColumnName: TensorFlowModelSettings.InputColumnName,
                    interleavePixelColors: TensorFlowModelSettings.InterleavePixelColors
                ))
                .Append(_mlContext.Model.LoadTensorFlowModel(TensorFlowModelLocation)
                    .ScoreTensorFlowModel(
                        outputColumnNames: new[] { TensorFlowModelSettings.OutputColumnName },
                        inputColumnNames: new[] { TensorFlowModelSettings.InputColumnName },
                        addBatchDimensionInput: false));


            var model = pipeline.Fit(GetEmptyDataView());
            return model;
        }


        private IDataView GetEmptyDataView()
        {
            var enumerableData = new List<InputImage>
            {
                new InputImage()
                {
                    Image = new Bitmap(TensorFlowModelSettings.InputImageWidth, TensorFlowModelSettings.InputImageHeight)
                }
            };
            return _mlContext.Data.LoadFromEnumerable(enumerableData);
        }
    }
}