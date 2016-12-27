using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NP.IVSwitch.Entities
{
    public class Firewall
    {
        public int Id { get; set; }
        public string Host { get; set; }
        public string Description { get; set; }
        public DateTime CreationDate { get; set; }
    }
}
