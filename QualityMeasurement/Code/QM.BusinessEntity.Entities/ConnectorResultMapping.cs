using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QM.BusinessEntity.Entities
{
    public class ConnectorResultMapping
    {
        public int ConnectorResultMappingId { get; set; }
        public string ConnectorType { get; set; }
        public int ResultId { get; set; }
        public string ResultName { get; set; }
        public List<string> ConnectorResults { get; set; }
    }
}
