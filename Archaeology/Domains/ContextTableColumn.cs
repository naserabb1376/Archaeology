namespace Domains
{
    public class ContextTableColumn : BaseEntity
    {
        public int ContextTableId { get; set; }
        public ContextTable ContextTable { get; set; }

        public string ColumnTitle { get; set; }     // مثلا "ابعاد" یا "طول"
        public int? ParentColumnId { get; set; }    // اگر null باشه یعنی سطح بالاست
        public ContextTableColumn ParentColumn { get; set; }

        public ICollection<ContextTableColumn> SubColumns { get; set; } // برای هدرهای تو در تو
        public int Order { get; set; }

    }
}
