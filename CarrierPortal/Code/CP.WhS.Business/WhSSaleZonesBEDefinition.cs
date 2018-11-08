using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;

namespace CP.WhS.Business
{
    public class WhSSaleZonesBEDefinition : BusinessEntityDefinitionSettings
    {
        public static Guid s_configId = new Guid("AEEF5962-D050-4F40-B914-E26977775FDD");
        public override Guid ConfigId { get { return s_configId; } }

        public override string DefinitionEditor { get { return "cp-whs-salezonesbedefinition-editor"; } }

        public override string IdType { get { return "System.Int64"; } }

        public override string SelectorUIControl { get { return "cp-whs-salezones-selector"; } }

        public override string ManagerFQTN { get { return "CP.WhS.Business.WhSSaleZoneBEManager, CP.WhS.Business"; } }

        public Guid VRConnectionId { get; set; }
    }
}
