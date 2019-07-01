using System.Drawing;
using Microsoft.ML.Transforms.Image;

namespace DogClassifierCore
{
    public class InputImage
    {
        [ImageType(TensorFlowModelSettings.InputImageHeight, TensorFlowModelSettings.InputImageWidth)]
        public Bitmap Image { get; set; }

        public static InputImage Create(string imagePath)
        {
            return new InputImage()
            {
                Image = (Bitmap)System.Drawing.Image.FromFile(imagePath)
            };
        }
    }
}