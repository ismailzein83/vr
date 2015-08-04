using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.BI.Entities
{
    public class BIConfigurationMeasure
    {
        public string ColumnName { get; set; }
        public string Expression { get; set; }
        public string RequiredPermissions { get; set; }
        public MeasureConfigurationType Type { get; set; }
    }
}
