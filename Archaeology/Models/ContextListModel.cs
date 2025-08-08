using Domains;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Models
{
    public class ContextListModel
    {
        public String TypeSearchName { get; set; }

        public ContextListModel()
        {
            ProjectNames = new List<SelectListItem>();
            ContextLists = new List<ContextModel>();
            pagenumbers = new List<int>();
        }

        public IList<ContextModel> ContextLists { get; set; }
        public List<int> pagenumbers { get; set; }
        public int page { get; set; }
        public int ProjectNameID { get; set; }
        public IList<SelectListItem> ProjectNames { get; set; }
    }

    public class ContextModel
    {
        public int ID { get; set; }
        public int ProjectId { get; set; }

        public string Title { get; set; }
        public string Description { get; set; }
    }
}