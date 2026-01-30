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

        // 現在のファイルパスを保持するプライベート変数
        private string? _currentFilePath;

        // ボタンが押せるかどうかを判定するロジック
        private bool CanExecuteFileOps() => !string.IsNullOrEmpty(_currentFilePath);

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
                    // パスを記憶しておく
                    _currentFilePath = openFileDialog.FileName;
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

                    // ボタンの状態（CanExecute）が変化したことを通知する
                    SaveFileCommand.NotifyCanExecuteChanged();
                    ShowAddWindowCommand.NotifyCanExecuteChanged();
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

        [RelayCommand(CanExecute = nameof(CanExecuteFileOps))] //追加コマンドに、判定メソッドを紐付け
        private void ShowAddWindow()
        {
            var vm = new AddRecordViewModel();
            // 追加用ウィンドウのインスタンスを作成し、DataContextにViewModelをセット(画面とViewModelを紐づけ)
            var window = new Views.AddRecordWindow { DataContext = vm };

            // ウィンドウを閉じる処理をセット
            vm.CloseAction = () => window.DialogResult = true;

            // モーダルダイアログとして表示
            // window.ShowDialog()で追加用画面が開きモーダルなので以降の処理は待機状態。
            // 追加用画面側の追加ボタン押下処理の最後にAction?.Invoke()でDialogResult=trueになりwindow.Close()が自動的に呼ばれ画面が閉じる。
            // その後、ここに戻ってきてif文の中が実行される。
            if (window.ShowDialog() == true && vm.NewRecord != null)
            {
                // 全データリストに追加
                _allRecords.Add(vm.NewRecord);

                // フィルタを再適用
                ApplyFilter();

                // カテゴリ一覧に新しいカテゴリがあれば追加
                if (!Categories.Contains(vm.NewRecord.Category))
                {
                    Categories.Add(vm.NewRecord.Category);
                }
            }
        }

        [RelayCommand(CanExecute = nameof(CanExecuteFileOps))] //保存コマンドに、判定メソッドを紐付け
        private void SaveFile()
        {
            if (string.IsNullOrEmpty(_currentFilePath))
            {
                StatusMessage = "保存先が見つかりません。一度ファイルを開いてください。";
                return;
            }

            try
            {
                // フィルタ後のデータではなく、必ず「全データ」を保存する
                _csvService.WriteCsv(_currentFilePath, _allRecords);
                StatusMessage = "ファイルを保存しました！";
            }
            catch (Exception ex)
            {
                StatusMessage = $"保存エラー: {ex.Message}";
            }
        }
    }
}
