namespace Models
{
    public class ContextTableColumnViewModel
    {
        public int Id { get; set; }
        public string ColumnTitle { get; set; }
        public int? ParentColumnId { get; set; }
        public int Order { get; set; }
        public string ParentTitle { get; set; } // فقط برای نمایش
    }

    public class ManageTableColumnsViewModel
    {
        public int TableId { get; set; }

        public List<ContextTableColumnViewModel> Columns { get; set; } = new List<ContextTableColumnViewModel>();

        public AddColumnModel AddModel { get; set; } = new AddColumnModel();
    }
}