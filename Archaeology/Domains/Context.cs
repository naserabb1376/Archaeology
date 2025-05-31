using Domains;

namespace Domains
{
    public class Context : BaseEntity
    {
        public int ProjectId { get; set; }
        public Project Project { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public ICollection<ContextImage> Images { get; set; }
        public ICollection<ContextTable> Tables { get; set; }
        public ICollection<ContextDisplayItem> DisplayItems { get; set; }

    }
}
