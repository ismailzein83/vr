using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;
using Vanrise.Security.Entities;

namespace Vanrise.GenericData.Business
{
    public class LKUPBusinessEntityDefinitionSettings : BusinessEntityDefinitionSettings
    {
        public static Guid s_configId = new Guid("99E22964-F94E-4BCD-8383-22A613E5AE7F");
        public override Guid ConfigId { get { return s_configId; } }
        public override string DefinitionEditor
        {
            get { return "vr-genericdata-lookupbusinessentity-editor"; }
        }
        public override string IdType
        {
            get
            {
                return "System.String";
            }
        }
        public override string SelectorUIControl
        {
            get { return "vr-genericdata-lookupbusinessentity-selector"; }
        }
        public override string ManagerFQTN
        {
            get { return "Vanrise.GenericData.Business.LKUPBusinessEntityManager, Vanrise.GenericData.Business"; }
        }
        public string SelectorSingularTitle { get; set; }
        public string SelectorPluralTitle { get; set; }
        public LKUPBEDefinitionExtendedSettings ExtendedSettings { get; set; }
        public LKUPBEDefinitionSecurity Security { get; set; }

    }
    public class LKUPBEDefinitionSecurity
    {
        public RequiredPermissionSettings ViewRequiredPermission { get; set; }

        public RequiredPermissionSettings AddRequiredPermission { get; set; }

        public RequiredPermissionSettings EditRequiredPermission { get; set; }
    }
    
   
}
