namespace Domains
{
    public class Project : BaseEntity
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }

        public ICollection<Context> Contexts { get; set; }
    }
}
