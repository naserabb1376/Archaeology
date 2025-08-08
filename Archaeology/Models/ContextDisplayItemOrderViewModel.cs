using Domains;

namespace Models
{
    // ویومدل برای مدیریت ترتیب محتوا در کانتکس
    public class ContextDisplayItemOrderViewModel
    {
        public int ContextId { get; set; }
        public List<DisplayItemEntry> Items { get; set; } = new List<DisplayItemEntry>();

        public class DisplayItemEntry
        {
            public int Id { get; set; }           // ContextDisplayItem.Id
            public string ItemName { get; set; }
            public DisplayItemType ItemType { get; set; }
            public int DisplayOrder { get; set; }
        }
    }
}