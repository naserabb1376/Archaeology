namespace Domains
{
    public class ContextDisplayItem : BaseEntity
    {
        public int ContextId { get; set; }
        public Context Context { get; set; }

        public DisplayItemType ItemType { get; set; } // جدول یا تصویر
        public int ItemId { get; set; }               // ID جدول یا تصویر

        public int DisplayOrder { get; set; }         // ترتیب نمایش در خروجی
    }
    public enum DisplayItemType
    {
        Table = 1,
        Image = 2
    }

}
