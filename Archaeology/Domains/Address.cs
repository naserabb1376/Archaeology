using System.Collections.Generic;

namespace Domains
{
    public class Address : BaseEntity
    {
        public string CityName { get; set; }
        public string PostalCode { get; set; }
        public string Street { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }
    }
}
