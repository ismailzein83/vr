using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Security.Entities
{
    public class UserPasswordHistory
    {
        public long Id { get; set; }
        public int UserId { get; set; }
        public string Password { get; set; }
        public bool IsChangedByAdmin { get; set; }
    }
}
