using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Security.Entities
{
    public class UserDetail
    {
        public User Entity { get; set; }
        public UserStatus Status { get; set; }
        public string GroupNames { get; set ; }
        public bool SupportPasswordManagement { get; set; }
    }
}
