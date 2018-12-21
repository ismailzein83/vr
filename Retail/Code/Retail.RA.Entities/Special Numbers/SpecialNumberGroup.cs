using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.RA.Entities
{
    public class SpecialNumberGroup
    {
        public int ID { get; set; }
        public string GroupName { get; set; }
        public SpecialNumbersSetting Settings { get; set; }
    }
}
