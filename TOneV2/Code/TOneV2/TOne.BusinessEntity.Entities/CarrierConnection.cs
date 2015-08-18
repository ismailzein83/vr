using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.BusinessEntity.Entities
{
    public class CarrierConnection
    {
        public int ID { get; set; }
        public string DisplayName { get; set; }
        public string SwitchName { get; set; }
        public ConnectionType ConnectionType { get; set; }
        public string TAG { get; set; }
        public string Value { get; set; }
        public string GateWay { get; set; }
    }
}
