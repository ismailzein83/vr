using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace QM.BusinessEntity.Entities
{
    public class SourceZoneReaderConfig : ExtensionConfiguration
    {
        public const string EXTENSION_TYPE = "QM_BE_SourceZoneReader";
        public string Editor { get; set; }
    }
}
