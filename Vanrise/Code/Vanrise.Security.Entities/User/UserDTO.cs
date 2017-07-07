using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Security.Entities
{
    public class UserDTO
    {
        public User Entity { get; set; }
        public List<int> AssignedGroupIds { get; set; }
    }
}
