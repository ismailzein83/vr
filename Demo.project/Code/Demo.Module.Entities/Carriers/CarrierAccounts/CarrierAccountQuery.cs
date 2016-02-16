using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Module.Entities
{
    public class CarrierAccountQuery
    {
        public List<int> CarrierAccountsIds { get; set; }
        public List<int> CarrierProfilesIds { get; set; }
        public string Name { get; set; }
    }
}
