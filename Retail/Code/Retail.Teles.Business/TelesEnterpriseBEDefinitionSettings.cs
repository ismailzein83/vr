using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;

namespace Retail.Teles.Business
{
    public class TelesEnterpriseBEDefinitionSettings : BusinessEntityDefinitionSettings
    {
        public static Guid s_configId = new Guid("E02E72EA-56BD-4F86-A404-F08BE3A2E619");
        public override Guid ConfigId { get { return s_configId; } }

        public override string DefinitionEditor
        {
            get { return "retail-teles-enterprisebedefinition-editor"; }
        }

        public override string IdType
        {
            get { return "System.Int32"; }
        }

        public override string SelectorUIControl
        {
            get { return "retail-teles-enterprises-selector"; }
        }

        public override string ManagerFQTN
        {
            get { return "Retail.Teles.Business.TelesEnterpriseManager, Retail.Teles.Business"; }
        }

        public override string GroupSelectorUIControl { get; set; }
        public Guid VRConnectionId { get; set; }
    }
}
