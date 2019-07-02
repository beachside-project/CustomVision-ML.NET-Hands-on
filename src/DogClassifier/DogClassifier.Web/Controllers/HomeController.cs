using DogClassifier.Web.Models;
using DogClassifierCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.ML;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;

namespace DogClassifier.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly PredictionEnginePool<InputImage, PredictionResult> _predictionEnginePool;
        private readonly PredictionDescriptor _predictionDescriptor;

        public HomeController(PredictionEnginePool<InputImage, PredictionResult> predictionEnginePool, PredictionDescriptor predictionDescriptor)
        {
            _predictionEnginePool = predictionEnginePool;
            _predictionDescriptor = predictionDescriptor;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public async Task<IActionResult> Predict(IFormFile imageFile)
        {
            if (imageFile.Length == 0) return BadRequest();

            using (var stream = new MemoryStream())
            {
                await imageFile.CopyToAsync(stream);
                var inputImage = new InputImage()
                {
                    Image = (Bitmap)Image.FromStream(stream)
                };

                var predictionResult = _predictionEnginePool.Predict(inputImage);
                var resultText = _predictionDescriptor.GetBestScore(predictionResult);

                return Ok(resultText);
            }
        }
    }
}