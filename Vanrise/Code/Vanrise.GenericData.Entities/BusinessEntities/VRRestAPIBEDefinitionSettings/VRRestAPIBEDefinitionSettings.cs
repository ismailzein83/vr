using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.GenericData.Entities
{
    public class VRRestAPIBEDefinitionSettings : BusinessEntityDefinitionSettings
    {
        public static Guid s_configId = new Guid("B4F22FFB-B663-4F5F-AF53-ACBEF7224DFB");
        public override Guid ConfigId { get { return s_configId; } }

        public override string DefinitionEditor
        {
            get { return "vr-genericdata-restapibedefinitions-editor"; }
        }

        public override string SelectorUIControl
        {
            get { return "vr-genericdata-businessentity-remoteselector"; }
        }

        public override string ManagerFQTN
        {
            get { return "Vanrise.GenericData.Business.VRRestAPIBusinessEntityManager, Vanrise.GenericData.Business"; }
        }

        public override string GroupSelectorUIControl { get; set; }

        public Guid ConnectionId { get; set; }
        public Guid RemoteBEDefinitionId { get; set; }
        public string SingularTitle { get; set; }
        public string PluralTitle { get; set; }
    }
}
