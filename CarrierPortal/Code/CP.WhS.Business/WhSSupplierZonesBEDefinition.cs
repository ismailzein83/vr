using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;

namespace CP.WhS.Business
{
    public class WhSSupplierZonesBEDefinition : BusinessEntityDefinitionSettings
    {
        public static Guid s_configId = new Guid("ECBDB7D3-0664-4F32-A8D0-596E11C3FB3E");
        public override Guid ConfigId { get { return s_configId; } }

        public override string DefinitionEditor { get { return "cp-whs-supplierzonesbedefinition-editor"; } }

        public override string IdType { get { return "System.Int64"; } }

        public override string SelectorUIControl { get { return "cp-whs-supplierzones-selector"; } }

        public override string ManagerFQTN { get { return "CP.WhS.Business.WhSSupplierZoneBEManager, CP.WhS.Business"; } }

        public Guid VRConnectionId { get; set; }
    }
}
