using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;
using Vanrise.Security.Entities;

namespace Vanrise.Common.Business
{
    public class VRCommentBEDefinitionSettings : BusinessEntityDefinitionSettings
    {
        public static Guid s_configId = new Guid("98154422-B815-4843-9304-CE63930CED84");
        public override Guid ConfigId { get { return s_configId; } }
        public VRCommentDefinitionSecurity Security { get; set; }
        public override string SelectorFilterEditor { get; set; }

        public override string DefinitionEditor { get { return "vr-commentbe-editor"; } }

        public override string IdType { get { return "System.Int64"; } }

        public override string ManagerFQTN { get { return "Vanrise.Common.Business.VRCommentManager, Vanrise.Common.Business"; } }

        public override string SelectorUIControl { get { return ""; } }

    }

    public class VRCommentDefinitionSecurity
    {
        public RequiredPermissionSettings ViewRequiredPermission { get; set; }
        public RequiredPermissionSettings AddRequiredPermission { get; set; }
    }
    
}
