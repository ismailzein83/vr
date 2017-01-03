using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;

namespace Vanrise.Common.Business
{
    public class StatusDefinitionBESettings : BusinessEntityDefinitionSettings
    {
        public static Guid s_configId = new Guid("3F26B7E1-33D8-4428-9A3C-986805289C91");
        public override Guid ConfigId { get { return s_configId; } }
        public override string SelectorFilterEditor { get; set; }
        public override string DefinitionEditor
        {
            get { return "vr-common-statusdefinitionbe-editor"; }
        }

        public override string IdType
        {
            get { return "System.Guid"; }
        }

        public override string ManagerFQTN
        {
            get { return "Vanrise.Common.Business.StatusDefinitionManager, Vanrise.Common.Business"; }
        }

        public override string SelectorUIControl
        {
            get { return "vr-common-statusdefinition-selector"; }
        }
    }
}
