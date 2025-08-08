namespace Models
{
    public class ContextTableListModel
    {
        public String TypeSearchName { get; set; }

        public ContextTableListModel()
        {
            ContextTableLists = new List<ContextTableList>();
            pagenumbers = new List<int>();
        }

        public List<int> pagenumbers { get; set; }
        public int page { get; set; }
        public int ContextID { get; set; }
        public IList<ContextTableList> ContextTableLists { get; set; }
    }

    public class ContextTableList
    {
        public int ID { get; set; }

        public string TableTitle { get; set; } // مثلاً "جدول آمار سفال‌ها"
        public int ContextId { get; set; }
    }
}