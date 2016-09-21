using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace QM.BusinessEntity.Entities
{
    public class ConnectorZoneReaderConfig : ExtensionConfiguration
    {
        public const string EXTENSION_TYPE = "QM_BE_ConnectorZoneReader";
        public string Editor { get; set; }
    }
}
