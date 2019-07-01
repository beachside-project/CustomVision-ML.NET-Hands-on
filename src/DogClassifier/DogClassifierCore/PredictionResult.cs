using Microsoft.ML.Data;

namespace DogClassifierCore
{
    public class PredictionResult
    {
        [ColumnName(TensorFlowModelSettings.OutputColumnName)]
        public float[] Scores { get; set; }
    }
}