# Chapter 2. ConsoleApp + ML.NET で 画像分類器アプリを開発

この Chapter では、Custom Vision Service でエクスポートした TensorFlow のモデルを利用し、Console App で、オフラインで画像分類ができるアプリを開発します。

## ML.NET について

ML.NET を利用すると、.NET アプリケーションで機械学習を行ったり、学習済みのモデルを読み込んで機械学習を行うことができます。  
ここでは、Chapter 1 で Custom Vision Service からエクスポートした TensorFlow のモデルを使って、画像分類ができるコンソールアプリを開発します。

&nbsp;

## STEP2-1 プロジェクトの作成

Visual Studio 2019 を起動し、**新しいプロジェクトを作成** をクリックしプロジェクトを作成します。2つのプロジェクトを作成します。

- コンソールアプリ（.NET Core）
- クラス ライブラリ（.NET Standard）

まず、コンソールアプリのプロジェクトを作成します。

- プロジェクトのテンプレートは、**コンソールアプリ（.NET Core）** を選択します。
- 「プロジェクト名」に **ConsoleApp"** と入力します。
- 「場所」は任意の場所を入力します。
- 「ソリューション名」に **DogClassifier** と入力します。

&nbsp;

次の Chapter でも再利用できるよう、クラスライブラリのプロジェクトを作成します。

- ソリューションエクスプローラのソリューション名を右クリック > **追加** > **新しいプロジェクト** をクリック
- プロジェクトのテンプレートは、**クラス ライブラリ（.NET Standard）** を選択します。
- 「プロジェクト名」には、**DogClassifierCore** と入力します。
- 「場所」は任意の名称を入力します。

### Nuget パッケージのインストール

ML.NET を利用する上で必要となる Nuget パッケージをインストールします。  
Visual Studio のショートカットキー `Ctrl` + `Q` をクリックし、「nuget」と入力し、「ソリューションのNugetパッケージの管理」をクリックします。

![02-01](../images/02-01.png)

&nbsp;

**DogClassifierCore** のプロジェクトに、以下4つのパッケージをインストールします。検索にパッケージ名を入力してインストールしましょう。

- **Microsoft.ML**
- **Microsoft.ML.TensorFlow**
- **Microsoft.ML.ImageAnalytics**
- **System.Drawing.Common**

![02-02](../images/02-02.png)

&nbsp;

## STEP2-2 **DogClassifierCore** プロジェクトの実装

まず、**DogClassifierCore** のプロジェクトを右クリック > 追加 > **フォルダ** をクリックして、フォルダを追加します。フォルダ名は、「TensorFlowModel」とします。

また、自動生成された `Class1.cs` のファイルは削除します。

&nbsp;

### TensorFlow の Model の利用準備

次に、Chapter.1 にて、Custom Vision Service でエクスポートした TensorFlow のモデルをこのフォルダに入れましょう。

エクスポートしたモデルの zip ファイルを解凍すると、以下2つのファイルが出力されます。

- labels.txt
- model.pb

これらを前述で作成したフォルダ「TensorFlowModel」にドラッグ & ドロップで追加します。

![02-03](../images/02-03.png)

&nbsp;

ファイルを置いたら、`labels.txt` を右クリック > **プロパティ** > **出力ディレクトリにコピー** の値を「新しい場合はコピーする」にします。`model.pb` も同様に設定します（2ファイル同時に選択して設定もできます）。

![02-03-2](../images/02-03-2.png)

&nbsp;


ここで参考までに、[Netron](https://electronjs.org/apps/netron) というツールを使って、今回利用するモデルがどうなっているかを確認してみます。

> ここでは、確認した結果の概要を説明するだけですが、興味のある方は、後日、自身でダウンロードして確認してみましょう。

モデルの最初の方を確認すると、インプットとなる部分の id は **Placeholder** で、typeは**float32[None, 244,224, 3] であることがわかります。

![02-04](../images/02-04.png)

&nbsp;

また、モデルの最後の方を確認すると、出力の id が **loss** であることがわかります。

![02-05](../images/02-05.png)

&nbsp;

これらの値は、以降の実装で利用します。

### TensorFlow のモデルの定義ファイルを追加

TensorFlow のモデルを利用する際に必要な設定を格納します。先ほど Netron で確認した値も利用します。

**DogClassifierCore** のプロジェクト上で右クリック > **追加** > **クラス** をクリックし、`TensorFlowModelSettings.cs` というファイル名でクラスを追加し、以下のように実装します。

```cs
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
```

&nbsp;

### 入力データ、出力結果の class を追加

予測する画像データを格納する class を追加します。

**DogClassifierCore** のプロジェクト上で右クリック > **追加** > **クラス** をクリックし、`InputImage.cs` というファイル名でクラスを追加し、以下のように実装します。

```cs
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
                Image = (Bitmap) System.Drawing.Image.FromFile(imagePath)
            };
        }
    }
}
```

&nbsp;

次に予測した結果を格納する class を追加します。

**DogClassifierCore** のプロジェクト上で右クリック > **追加** > **クラス** をクリックし、`PredictionResult.cs` というファイル名でクラスを追加し、以下のように実装します。

```cs
using Microsoft.ML.Data;

namespace DogClassifierCore
{
    public class PredictionResult
    {
        [ColumnName(TensorFlowModelSettings.OutputColumnName)]
        public float[] Scores { get; set; }
    }
}
```

&nbsp;

### `ModelConfigurator` class の追加

**DogClassifierCore** のプロジェクト上で右クリック > **追加** > **クラス** をクリックし、`ModelConfigurator.cs` というファイル名でクラスを追加し、以下のように実装します。

```cs
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
```

&nbsp;

>  # TODO: 追記予定

## STEP2-3 ConsoleApp から呼び出す

### 参照の追加

**ConsoleApp** プロジェクトを右クリック > **追加** > **参照** をクリックします。  
左ペインで **プロジェクト** をクリックし、**DogClassifierCore** にチェックを入れて **OK** ボタンをクリックします。  
これで、**ConsoleApp** プロジェクトから **DogClassifierCore** プロジェクトが参照できるようになります。

![02-06](../images/02-06.png)

&nbsp;

### Program.cs の実装

**ConsoleApp** プロジェクトの `Program.cs` を開き、以下のように実装しましょう。  
注意として、 `inputImagePath` には、自身のファイルパスを入力しましょう。

```cs
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


            // TODO !!!!! Chapter.1でダウンロードした画像の中で、testset のファイルのフルパスを入力してください。 
            var inputImagePath = @"C:\Users\yokohama\Desktop\dogs\dogs\testset\n02099601_3004.jpg";


            var inputImage = InputImage.Create(inputImagePath);

            var result = modelConfigurator.Predict(inputImage);
            Console.WriteLine(result);

            Console.ReadKey();
        }
    }
}
```

ローカルにあるファイルを読み込み、分類器で予測した結果をコンソールに表示できるようになりました。

&nbsp;

## NEXT 

**おめでとうございます！**:star2:  
Custom Vision で学習した犬の画像分類器と ML.NET を利用して、コンソールアプリで画像の分類ができるようになりました。

ここでは、1つのファイルの結果を予測し、TOP 1の結果を返す実装をしましたが、応用することで複数ファイルを分析したり、上位Nこの結果を返すことも容易に可能です。

次の Chapter では、 ASP.NET Core を利用して画像分類ができるWebAPIを開発します。

---

[戻る](../README.md) | [次へ進む](./03_create-aspnetcore-app.md)