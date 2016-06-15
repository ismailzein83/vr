using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Entities
{
    public class Switch
    {
        public int SwitchId { get; set; }

        public string Name { get; set; }

        public SwitchSettings Settings { get; set; }
    }
}
