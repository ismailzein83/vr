using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMExtended.Main.Entities
{
    public class ServicePayment
    {

        public string Id { get; set; }
        public string PackageID { get; set; }
        public bool UpFront { get; set; }
    }
}
