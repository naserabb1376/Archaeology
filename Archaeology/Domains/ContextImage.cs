namespace Domains
{
    public class ContextImage : BaseEntity
    {
        public int ContextId { get; set; }
        public Context Context { get; set; }

        public string ImagePath { get; set; }
        public string Caption { get; set; } // متن رفرنس

    }
}
