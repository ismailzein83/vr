using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CP.SupplierPriceList.TOneV1Integration
{
    public class RootObject
    {
        public string TokenName { get; set; }
        public int ExpirationIntervalInMinutes { get; set; }
        public string Username { get; set; }
        public string UserDisplayName { get; set; }
        public string Token { get; set; }
    }
}
