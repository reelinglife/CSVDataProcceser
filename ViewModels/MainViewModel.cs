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

        // プロパティ：画面のDataGridに表示するデータのリスト
        // [ObservableProperty]属性によって裏でRecordsプロパティが自動生成され、DataGridでItemsSource="{Binding Records}"とすることでデータが紐づく
        [ObservableProperty]
        private ObservableCollection<CsvRecord> _records = [];

        // 全データを保持するプライベートなリスト
        private List<CsvRecord> _allRecords = [];

        // フィルタ用のプロパティ
        [ObservableProperty]
        private ObservableCollection<string> _categories = [];

        // プロパティ：ステータスバーなどに表示するメッセージ
        // [ObservableProperty]属性によって裏でStatusMessageプロパティが自動生成され、TextBlock等でText="{Binding StatusMessage}"とすることでテキスト表示する
        [ObservableProperty]
        private string _statusMessage = "CSVファイルを選択してください。";

        // 総売上金額を保持するプロパティ
        [ObservableProperty]
        private decimal _totalSales;

        // コンボボックスの選択値(SelectedItem)を保持するプロパティ
        // [ObservableProperty]属性によって裏でSelectedCategoryプロパティが自動生成され、ComboBoxでSelectedItem="{Binding SelectedCategory}"とする選択値が紐づく
        [ObservableProperty]
        private string? _selectedCategory;

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
                    // Private変数に全データを保存
                    _allRecords = data;
                    // カテゴリ一覧リストの生成（重複なし）
                    var catList = data.Select(x => x.Category).Distinct().ToList();
                    // 先頭に解除用項目を追加
                    catList.Insert(0, "すべて");
                    // カテゴリ選択コンボボックス用のリストを更新
                    Categories = [.. catList]; //Categories = new ObservableCollection<string>(catList);
                    // 初期値(すべて)を設定
                    SelectedCategory = "すべて";

                    ApplyFilter();
                }
                catch (Exception ex)
                {
                    StatusMessage = $"エラー: {ex.Message}";
                }
            }
        }

        // SelectedCategoryが変わった時に実行されるメソッド（CommunityToolkitの機能）
        // ComboBoxのSelectedItemにバインドされているSelectedCategoryプロパティが変わると自動で呼ばれる
        partial void OnSelectedCategoryChanged(string? value)
        {
            ApplyFilter();
        }

        private void ApplyFilter()
        {
            IEnumerable<CsvRecord> filtered = _allRecords;

            if (!string.IsNullOrEmpty(SelectedCategory) && SelectedCategory != "すべて")
            {
                filtered = _allRecords.Where(x => x.Category == SelectedCategory);
            }

            // 画面表示用リストを更新
            Records = [.. filtered]; //Records = new ObservableCollection<CsvRecord>(filtered);
            // 合計金額もフィルタ後の内容で再計算
            TotalSales = filtered.Sum(x => x.TotalAmount);
            StatusMessage = $"{filtered.Count()} 件を表示中";
        }
    }
}
