using System;
using DogClassifierCore;
using Microsoft.ML;

namespace ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var mlContext = new MLContext();
            var modelConfigurator = new ModelConfigurator(mlContext);


            // TODO !!! Chapter.1でダウンロードした画像の中で、testset のファイルのフルパスを入力してください。 
            var inputImagePath = @"C:\tmp\dogs\testset\n02099601_3004.jpg";


            var inputImage = InputImage.Create(inputImagePath);

            var result = modelConfigurator.Predict(inputImage);
            Console.WriteLine(result);

            Console.ReadKey();
        }
    }
}
