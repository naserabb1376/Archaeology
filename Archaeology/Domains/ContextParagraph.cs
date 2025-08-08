namespace Domains
{
    public class ContextParagraph : BaseEntity
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public int ContextId { get; set; }
        public Context Context { get; set; }
    }
}