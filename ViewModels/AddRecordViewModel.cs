using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CsvDataProcessor.Models;
using System.ComponentModel.DataAnnotations;

namespace CsvDataProcessor.ViewModels
{
    public partial class AddRecordViewModel : ObservableValidator
    {
        // 日付
        [ObservableProperty]
        private DateTime _date = DateTime.Now;

        // 商品名
        [ObservableProperty]
        [Required(ErrorMessage = "商品名は必須です")]
        [MinLength(2, ErrorMessage = "2文字以上で入力してください")]
        [NotifyCanExecuteChangedFor(nameof(AddCommand))]
        private string _productName = "";

        // カテゴリ
        [ObservableProperty]
        [Required(ErrorMessage = "カテゴリは必須です")]
        [NotifyCanExecuteChangedFor(nameof(AddCommand))]
        private string _category = "";

        // 単価
        [ObservableProperty]
        [Range(1, 1000000, ErrorMessage = "単価は1〜1,000,000の間で入力してください")]
        [NotifyCanExecuteChangedFor(nameof(AddCommand))]
        private decimal _price;

        // 数量
        [ObservableProperty]
        [Range(1, 1000, ErrorMessage = "数量は1〜1,000の間で入力してください")]
        [NotifyCanExecuteChangedFor(nameof(AddCommand))]
        private int _quantity;

        // コンストラクタ
        public AddRecordViewModel()
        {
            // 画面起動時に全ての項目をチェックし、エラー状態（HasErrors）を確定させる
            ValidateAllProperties();
        }

        // 保存された結果をメイン画面が受け取るためのプロパティ
        public CsvRecord? NewRecord { get; private set; }

        // ウィンドウを閉じるためのアクション
        public Action? CloseAction { get; set; }

        // 追加ボタンに設定しているCommand="{Binding AddCommand}"の処理
        [RelayCommand(CanExecute = nameof(CanAdd))]
        private void Add()
        {
            // 入力された値からモデルを作成
            NewRecord = new CsvRecord
            {
                Date = Date,
                ProductName = ProductName,
                Category = Category,
                Price = Price,
                Quantity = Quantity
            };
            // ウィンドウを閉じる(追加用画面のDialogResultをtrueに設定してるだけ)
            CloseAction?.Invoke();
        }

        // 入力エラーがないかチェックする判定用メソッド
        private bool CanAdd() => !HasErrors;

        // 値が変わるたびにエラーチェックを実行し、ボタンの有効/無効を更新する(CommunityToolkitの機能)
        partial void OnProductNameChanged(string value) => ValidateProperty(value, nameof(ProductName));
        partial void OnCategoryChanged(string value) => ValidateProperty(value, nameof(Category));
        partial void OnPriceChanged(decimal value) => ValidateProperty(value, nameof(Price));
        partial void OnQuantityChanged(int value) => ValidateProperty(value, nameof(Quantity));
    }
}
