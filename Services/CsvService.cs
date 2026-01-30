using CsvDataProcessor.Models;
using System.IO;

namespace CsvDataProcessor.Services
{
    public class CsvService
    {
        /// <summary>
        /// CSVファイルを読み込み、CsvRecordのリストを返す。
        /// </summary>
        /// <param name="filePath">ファイルのパス</param>
        public List<CsvRecord> ReadCsv(string filePath)
        {
            var records = new List<CsvRecord>();
            try
            {
                // ファイルの全行を読み込み（1行目はヘッダーと想定してスキップ）
                var lines = File.ReadAllLines(filePath).Skip(1);

                foreach (var line in lines)
                {
                    var columns = line.Split(',');
                    if (columns.Length >= 5)
                    {
                        records.Add(new CsvRecord
                        {
                            Date = DateTime.Parse(columns[0]),
                            ProductName = columns[1],
                            Category = columns[2],
                            Price = decimal.Parse(columns[3]),
                            Quantity = int.Parse(columns[4])
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                // ログ出力
                // ログ出力処理作成までは上位（ViewModel）に例外を投げて知らせる
                throw new Exception("CSVの読み込みに失敗しました。", ex);
            }
            return records;
        }

        /// <summary>
        /// データをCSVとして保存
        /// </summary>
        public void WriteCsv(string filePath, IEnumerable<CsvRecord> records)
        {
            var lines = new List<string> { "日付,商品名,カテゴリ,単価,数量" }; // ヘッダー
            lines.AddRange(records.Select(r => $"{r.Date:yyyy/MM/dd},{r.ProductName},{r.Category},{r.Price},{r.Quantity}"));
            File.WriteAllLines(filePath, lines, System.Text.Encoding.UTF8);
        }
    }
}
