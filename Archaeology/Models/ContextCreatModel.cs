using Microsoft.AspNetCore.Mvc.Rendering;

namespace Models
{
    public class ContextCreatModel
    {
        public ContextCreatModel()
        {
            Project = new List<SelectListItem>();
        }

        public int ProjectId { get; set; }
        public int ID { get; set; }
        public IList<SelectListItem> Project { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
    }
}