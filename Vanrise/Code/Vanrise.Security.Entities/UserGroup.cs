using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Security.Entities
{
    public class UserGroup
    {
        public int UserId { get; set; }

        public int GroupId { get; set; }
    }

    public class UserGroupDetail
    {
        public int UserId { get; set; }

        public int GroupId { get; set; }
    }
}
