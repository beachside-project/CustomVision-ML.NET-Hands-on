namespace DogClassifierCore
{
    public struct TensorFlowModelSettings
    {
        public const string InputColumnName = "Placeholder";
        public const int InputImageWidth = 224;
        public const int InputImageHeight = 224;
        public const bool InterleavePixelColors = true;
        public const string OutputColumnName = "loss";
    }
}