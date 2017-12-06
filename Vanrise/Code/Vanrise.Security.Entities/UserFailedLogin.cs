using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Security.Entities
{
    public class UserFailedLogin
    {
        public long Id { get; set; }
        public int UserId { get; set; }
        public int FailedResultId { get; set; }
        public DateTime CreatedTime { get; set; }
    }
}
