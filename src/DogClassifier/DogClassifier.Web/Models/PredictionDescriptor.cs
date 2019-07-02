using DogClassifierCore;
using System;
using System.Linq;

namespace DogClassifier.Web.Models
{
    public class PredictionDescriptor
    {
        private readonly string[] _labels;

        public PredictionDescriptor(string[] labels)
        {
            _labels = labels;
        }

        public string GetBestScore(PredictionResult predictionResult)
        {
            var best = predictionResult.Scores.Max();
            var bestScore = best.ToString("F5");

            return $"{_labels[predictionResult.Scores.AsSpan().IndexOf(best)]}: {bestScore}";
        }
    }
}