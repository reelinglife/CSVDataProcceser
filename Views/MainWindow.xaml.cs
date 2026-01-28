using System.Windows;

namespace CsvDataProcessor.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            //画面のデータソースにMainViewModelを設定
            DataContext = new ViewModels.MainViewModel();
        }
    }
}