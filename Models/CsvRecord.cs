using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CsvDataProcessor.Models
{
    public class CsvRecord
    {
        // 1. 日付（いつ売れたか）
        public DateTime Date { get; set; }

        // 2. 商品名（何が売れたか）
        public string ProductName { get; set; } = string.Empty;

        // 3. カテゴリ（分類）
        public string Category { get; set; } = string.Empty;

        // 4. 単価（いくらで）
        public decimal Price { get; set; }

        // 5. 数量（いくつ売れたか）
        public int Quantity { get; set; }

        // 6. 合計金額（計算プロパティ）
        public decimal TotalAmount => Price * Quantity;

    }
}
