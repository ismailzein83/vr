using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Security.Entities
{
    public class EmailInfo
    {
        public string Id { get { return Email.Replace("@", ""); } }
        public string Email { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }

    public class EmailInfoFilter
    {
    }
}
