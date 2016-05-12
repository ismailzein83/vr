using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QM.BusinessEntity.Entities
{
    public class ConnectorZoneInfoToUpdate
    {
        public string ConnectorZoneId { get; set; }

        public List<string> Codes { get; set; }
    }
}
