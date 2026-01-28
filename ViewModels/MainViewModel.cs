using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CsvDataProcessor.Models;
using CsvDataProcessor.Services;
using Microsoft.Win32;
using System.Collections.ObjectModel;

namespace CsvDataProcessor.ViewModels
{
    // ObservableObjectを継承することで、画面更新の通知機能が自動で備わります
    public partial class MainViewModel : ObservableObject
    {
        private readonly CsvService _csvService;

        // 1. プロパティ：画面のDataGridに表示するデータのリスト
        // [ObservableProperty]属性によって裏でRecordsプロパティが自動生成され、DataGridでItemsSource="{Binding Records}"とすることでデータが紐づく
        [ObservableProperty]
        private ObservableCollection<CsvRecord> _records = [];

        // 2. プロパティ：ステータスバーなどに表示するメッセージ
        // [ObservableProperty]属性によって裏でStatusMessageプロパティが自動生成され、TextBlock等でText="{Binding StatusMessage}"とすることでテキスト表示する
        [ObservableProperty]
        private string _statusMessage = "CSVファイルを選択してください。"; 

        public MainViewModel()
        {
            _csvService = new CsvService();
        }

        // 3. コマンド：ボタンが押された時の動作
        // [RelayCommand]を付けると、ViewのButtonから呼び出せるようになる ※XAMLでCommand="{Binding OpenFileCommand}"のように指定可能
        [RelayCommand]
        private void OpenFile()
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "CSVファイル (*.csv)|*.csv|すべてのファイル (*.*)|*.*"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    // Serviceを使ってデータを読み込む
                    var data = _csvService.ReadCsv(openFileDialog.FileName);

                    // 画面表示用のリストを更新
                    Records = [.. data];
                    StatusMessage = $"{data.Count} 件のデータを読み込みました。";
                }
                catch (Exception ex)
                {
                    StatusMessage = $"エラー: {ex.Message}";
                }
            }
        }
    }
}
