namespace Domains
{
    public class ContextTableCell : BaseEntity
    {

        public int RowId { get; set; }
        public ContextTableRow Row { get; set; }

        public int ColumnId { get; set; }
        public ContextTableColumn Column { get; set; }

        public string Value { get; set; }
    }
}
