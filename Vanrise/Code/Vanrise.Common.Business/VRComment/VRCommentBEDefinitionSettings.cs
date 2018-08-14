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
        public override Guid ConfigId
        {
            get { return new Guid("99E22964-F94E-4BCD-8383-22A613E5AE7F"); }
        }
        public VRCommentDefinitionSecurity Security { get; set; }

    }

    public class VRCommentDefinitionSecurity
    {
        public RequiredPermissionSettings ViewRequiredPermission { get; set; }
        public RequiredPermissionSettings AddRequiredPermission { get; set; }
    }
    
}
