using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CP.WhS.Entities
{
    public class ClientReleaseCodeQuery
    {
        public ClientReleaseCodeFilter Filter { get; set; }
        public DateTime From { get; set; }
        public DateTime? To { get; set; }
    }
}
