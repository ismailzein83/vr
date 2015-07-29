using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.BI.Entities
{
    public class BIConfigurationEntity
    {
        public string ColumnID { get; set; }
        
        public string ColumnName { get; set; }

        public string BehaviorFQTN { get; set; }
    }
}
