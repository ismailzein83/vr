using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.GenericData.Entities
{
    public class GenericBEDefinitionSettings : BusinessEntityDefinitionSettings
    {
        public static Guid _configId = new Guid("6F3FBD7B-275A-4D92-8E06-AD7F7B04C7D6");
        public override Guid ConfigId { get { return _configId; } }

        public override string DefinitionEditor 
        { 
            get { return "vr-genericdata-genericbusinessentity-editor"; } 
        }

        public override string IdType
        {
            get { return "System.Int64"; }
        }

        public override string SelectorUIControl
        {
            get { return "vr-genericdata-genericbusinessentity-selector"; }
        }

        public override string ManagerFQTN
        {
            get { return "Vanrise.GenericData.Business.GenericBusinessEntityManager, Vanrise.GenericData.Business"; }
        }

        public override string GroupSelectorUIControl { get; set; }

        public GenericRuleDefinitionSecurity Security { get; set; }
        public Guid DataRecordTypeId { get; set; }
        public string FieldPath { get; set; }
        public GenericEditor EditorDesign { get; set; }
        public GenericManagement ManagementDesign { get; set; }
    }
}
