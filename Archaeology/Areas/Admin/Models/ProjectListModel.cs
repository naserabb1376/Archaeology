using Domains;
using Microsoft.Build.ObjectModelRemoting;

namespace Models
{
    public class ProjectListModel
    {
        public String TypeSearchName { get; set; }

        public ProjectListModel()
        {
            Projectlist = new List<ProjectModel>();
            pagenumbers = new List<int>();
        }

        public IList<ProjectModel> Projectlist { get; set; }
        public List<int> pagenumbers { get; set; }
        public int page { get; set; }
    }
}