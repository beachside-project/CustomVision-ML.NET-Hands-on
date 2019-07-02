# Azure Custom Vision と ML.NET を使ったアプリケーション開発ハンズオン

Microsoft Azure の Cognitive Serivces の一つ、Custom Vision を活用して、AI を組み込んだコンソールアプリと Web アプリケーションを開発をする入門者向けハンズオンです。  
**TensorFlow** の学習モデルを、[ML.NET](https://dotnet.microsoft.com/apps/machinelearning-ai/ml-dotnet) を活用して .NET Core のアプリケーションに組み込みます。

## 概要

このハンズオンでは、以下の内容を開発します。

- Custom Vision を活用して、機械学習のコーディング無しで犬種の画像分類の学習モデルを作成します。
- Custom Vision で作成した犬種の画像分類の学習モデルを、**TensorFlow** の学習モデルとしてエクスポートします。
- エクスポートした学習モデルを、[ML.NET](https://dotnet.microsoft.com/apps/machinelearning-ai/ml-dotnet) を活用してコンソールアプリに組み込み、画像分類ができる AI アプリにします。
- エクスポートした学習モデルを、[ML.NET](https://dotnet.microsoft.com/apps/machinelearning-ai/ml-dotnet) を活用して ASP.NET Core の Web アプリに組み込み、画像分類ができる AI アプリにします。

## ゴール

- Custom Vision の利用方法を理解する
- 学習モデルをエクスポートし、.NET のアプリケーションでの活用方法を体験する

## 前提・準備

- Azure のサブスクリプションが必要となります。開始前にご準備ください。
- Azure ポータルや Custom Vision ポータルでの操作には、モダンなブラウザー(Chrome や FireFox, Microsoft Edgeなど)が必要です。
- C#, .NET Core で、コンソールアプリケーション、ASP.NET Core のアプリケーションの開発を行います。PCで必要なセットアップをお願いします。本コンテンツでは、Visual Studio 2019 での操作することを前提に解説を進めます。

## ハンズオンの構成

|Chapter|概要|
|--:|---|
|1|[Custom Vision で犬種判別の分類器作成](./docs/01_create-custom-vision.md)|
|2|[TensorFlow + ML.NET で画像分類のコンソールアプリを開発](./docs/02_create-console-app.md)|
|3|[TensorFlow + ML.NET で画像分類の Web API を開発](./docs/03_create-aspnetcore-app.md)|
|4|[TAzure のリソースの削除](./docs/04_cleanup-resources.md)|

---

ハンズオンを開始しましょう。

[ハンズオンへ進む](./docs/01_create-custom-vision.md)