using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Invoice.Entities
{
    public class IPartnerManagerInfoContext
    {
        public string PartnerId { get; set; }
        public string InfoType { get; set; }
        public PartnerSettings PartnerSettings { get; set; }
    }
}
