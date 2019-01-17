using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.MobileNetwork.Entities
{
   public class NumberPrefix
    {
        public long Id { get; set; }
        public long Code { get; set; }
        public DateTime BED { get; set; }
        public DateTime EED { get; set; }
        public int MobileNetworkId { get; set; }
    }
}
