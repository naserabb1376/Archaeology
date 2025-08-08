using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class ContextImageModel
    {
        public int ID { get; set; }

        [Required]
        public List<IFormFile> ImageFiles { get; set; } // تغییر اینجا

        public List<string> Captions { get; set; } // کپشن‌ها به ترتیب
        public int ContextId { get; set; }
    }
}