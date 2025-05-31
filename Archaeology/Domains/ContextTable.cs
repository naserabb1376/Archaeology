using Domains;

namespace Domains
{
    public class ContextTable : BaseEntity
    {
        public int ContextId { get; set; }
        public Context Context { get; set; }

        public string TableTitle { get; set; } // مثلاً "جدول آمار سفال‌ها"
        public ICollection<ContextTableColumn> Columns { get; set; }
        public ICollection<ContextTableRow> Rows { get; set; }
    }
}
