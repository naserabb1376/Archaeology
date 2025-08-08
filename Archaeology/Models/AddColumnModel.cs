using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class AddColumnModel
    {
        public int TableId { get; set; }

        [Required]
        public string ColumnTitle { get; set; }

        public int? ParentColumnId { get; set; }

        public int Order { get; set; }
    }
}