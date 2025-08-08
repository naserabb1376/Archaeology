using Microsoft.AspNetCore.Mvc.Rendering;
using Models;

namespace Models
{
    public class ContextParagraphListModel
    {
        public String TypeSearchName { get; set; }

        public ContextParagraphListModel()
        {
            ContextParagraphLists = new List<ContextParagraphList>();
            pagenumbers = new List<int>();
        }

        public List<int> pagenumbers { get; set; }
        public int page { get; set; }
        public int ContextID { get; set; }
        public IList<ContextParagraphList> ContextParagraphLists { get; set; }
    }

    public class ContextParagraphList
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int ContextId { get; set; }
    }
}