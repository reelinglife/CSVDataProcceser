# CsvDataProcessor

WPF (MVVM) で開発した、モダンなUIを持つ売上データ解析デスクトップアプリです。

## 概要
CSV形式の売上データを読み込み、グリッド表示およびデータの自動統計（総売上の算出）を行うツールです。
単なるデータの表示にとどまらず、実務を意識したデータ型（decimal）の採用や、保守性の高いMVVMパターンを徹底しています。



## 主な機能
- **CSVインポート**: 売上データ（日付、商品名、カテゴリ、単価、数量）をワンクリックで読み込み
- **自動計算機能**: 各行の合計金額（単価×数量）および、読み込んだ全データの総売上をリアルタイムに集計
- **動的フィルタリング**: 読み込んだデータからカテゴリ一覧を自動抽出。コンボボックスで選択するだけで、表示データと合計金額がリアルタイムに更新されます。
- **レコードの新規追加機能**: UIから直接新しい売上データを追加可能。追加と同時に合計金額も更新
- **保存機能の実装**: 追加したデータを反映したCSVファイルとして保存
- **モダンUI**: Material Designを採用し、直感的で清潔感のあるユーザーインターフェースを提供

## プレビュー
![アプリのスクリーンショット](https://github.com/user-attachments/assets/95fbb8b1-c82a-45df-9212-7cc9dc3d948a)

## 技術スタック
- **Language**: C# / .NET 8
- **Framework**: WPF (Windows Presentation Foundation)
- **Architecture**: MVVM パターン
- **Library**: 
  - [CommunityToolkit.Mvvm](https://github.com/CommunityToolkit/dotnet) (Source Generatorsによるボイラープレート削減)
  - [MaterialDesignInXamlToolkit](https://github.com/MaterialDesignInXAML/MaterialDesignInXamlToolkit) (マテリアルデザインの実装)
- **Design**: 游ゴシック (Yu Gothic) による視認性の向上

## 設計のこだわり
### 1. 関心の分離（Separation of Concerns）
`Service`（CSV読み込みロジック）、`ViewModel`（画面制御）、`Model`（データ構造）を完全に分離しています。これにより、将来的なデータベース(SQL)への移行や、ユニットテストの導入が容易な設計になっています。

### 2. 精度の高い計算ロジック
金融・売上データを扱うことを想定し、金額計算には浮動小数点の誤差が発生しない `decimal` 型を採用しています。

### 3. 動的なUIバインディング
`DynamicResource` を活用したテーマカラー管理や、`StringFormat` による数値のカンマ区切り・単位付与など、ユーザーの利便性を高めるXAML実装を行っています。

### 4. LINQによる効率的なデータ処理
フィルタリング処理には **LINQ (Language Integrated Query)** を活用しています。
元のデータソース（`List<T>`）を保持したまま、表示用の `ObservableCollection` に対して `Where` や `Distinct` を適用することで、非破壊的かつ高速なデータ操作を実現しました。また、フィルタリングと同時に「合計金額の再計算」を走らせることで、UIとデータの整合性を常に保つ設計にしています。

### 5. 堅牢なデータバリデーション
`ObservableValidator` と `DataAnnotations` を組み合わせ、リアルタイムな入力チェックを実装。不正なデータ入力を UI レベルで遮断することで、データの整合性とユーザーの利便性を両立しています。

### 6. コンテキストに応じたUI制御
データの読み込み状態を監視し、ファイル未選択時には「保存」や「追加」ボタンを無効化。ユーザーによる不正な操作（データ不在時の保存実行など）をUIレベルで制限し、直感的なUXを提供しています。

## 開発環境
- Visual Studio 2022
- .NET 8.0