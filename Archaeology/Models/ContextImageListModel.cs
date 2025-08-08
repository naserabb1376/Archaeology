namespace Models
{
    public class ContextImageListModel
    {
        public String TypeSearchName { get; set; }

        public ContextImageListModel()
        {
            ContextImageLists = new List<ContextImageList>();
            pagenumbers = new List<int>();
        }

        public List<int> pagenumbers { get; set; }
        public int page { get; set; }
        public int ContextID { get; set; }
        public IList<ContextImageList> ContextImageLists { get; set; }
    }

    public class ContextImageList
    {
        public int ID { get; set; }
        public string ImagePath { get; set; }
        public string Caption { get; set; } // متن رفرنس
        public int ContextId { get; set; }
    }
}