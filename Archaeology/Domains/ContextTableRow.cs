namespace Domains
{
    public class ContextTableRow : BaseEntity
    {
        public int ContextTableId { get; set; }
        public ContextTable ContextTable { get; set; }

        public ICollection<ContextTableCell> Cells { get; set; }

    }
}
