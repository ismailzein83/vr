using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QM.BusinessEntity.Entities
{
    public class ConnectorZoneInfo
    {
        public long ConnectorZoneInfoId { get; set; }
        public string ConnectorType { get; set; }

        public string ConnectorZoneId { get; set; }

        public List<string> Codes { get; set; }
    }
}
