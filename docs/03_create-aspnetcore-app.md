# Chapter 3. ASP.NET Core + ML.NET で 画像分類器アプリを開発

この Chapter では、Custom Vision Service でエクスポートした TensorFlow のモデルを利用し、ASP.NET Core の WebAPI で画像分類ができるアプリを開発します。

## ASP.NET Core で ML.NET を利用する際の注意

前 Chapter でコンソールアプリで画像分類を行った `PredictionEngine` 型のオブジェクトは、スレッドセーフではありません。Web アプリでは複数のスレッドからオブジェクトが読み込まれるため、スレッドセーフであることとパフォーマンスを考慮する必要があります。  
この問題を解決するため、`PredictionEnginePool` を利用して実装を行います。

&nbsp;

## STEP3-1 **DogClassifierCore** のプロジェクトでモデルの保存処理を追加

> # 作成中