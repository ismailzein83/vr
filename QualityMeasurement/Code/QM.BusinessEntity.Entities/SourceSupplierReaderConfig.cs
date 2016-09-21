using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace QM.BusinessEntity.Entities
{
    public class SourceSupplierReaderConfig : ExtensionConfiguration
    {
        public const string EXTENSION_TYPE = "QM_BE_SourceSupplierReader";
        public string Editor { get; set; }
    }
}
