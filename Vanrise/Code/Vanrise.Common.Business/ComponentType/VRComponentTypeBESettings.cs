using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;

namespace Vanrise.Common.Business
{
    public class VRComponentTypeBESettings : BusinessEntityDefinitionSettings
    {
        public static Guid s_configId = new Guid("0873C4D8-C6CC-4DDB-ACCA-E8EC850C9186");
        public override Guid ConfigId { get { return s_configId; } }

        public override string SelectorFilterEditor { get; set; }

        public override string DefinitionEditor { get { return "vr-common-componenttypebe-editor"; } }

        public override string IdType { get { return "System.Guid"; } }

        public override string ManagerFQTN { get { return "Vanrise.Common.Business.VRComponentTypeManager, Vanrise.Common.Business"; } }

        public override string SelectorUIControl { get { return ""; } }

        public Guid VRComponentTypeConfigId { get; set; }
    }
}
