using Domains;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Models
{
    public class ManageCustomeRoleModel
    {
        public ManageCustomeRoleModel()
        {
            ListRoles = new List<UserRole>();
            AssignedRoles = new List<UserRole>();
        }
        public string fullName { get; set; }
        public  string Id { get; set; }
        public List<UserRole> ListRoles { get; set; }
        public List<UserRole> AssignedRoles { get; set; }
    }
}
