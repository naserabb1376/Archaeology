namespace Models
{
    // ویومدل‌ها برای ردیف‌ها و سلول‌ها
    public class CellInputModel
    {
        public int ColumnId { get; set; }
        public string ColumnTitle { get; set; }
        public List<string> FullPathTitles { get; set; } = new List<string>();
        public string Value { get; set; }
    }

    public class TableRowEditModel
    {
        public int TableId { get; set; }
        public int? RowId { get; set; } // برای حالت ویرایش (در آینده)
        public List<CellInputModel> Cells { get; set; } = new List<CellInputModel>();

        public List<ExistingRowViewModel> ExistingRows { get; set; } = new List<ExistingRowViewModel>();

        public class ExistingRowViewModel
        {
            public int RowId { get; set; }
            public List<string> CellValues { get; set; } = new List<string>();
        }
    }
}