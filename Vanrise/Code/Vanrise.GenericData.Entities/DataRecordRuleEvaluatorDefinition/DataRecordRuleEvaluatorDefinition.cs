using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Security.Entities;

namespace Vanrise.GenericData.Entities
{
    public class DataRecordRuleEvaluatorDefinition : Vanrise.Entities.VRComponentType<DataRecordRuleEvaluatorDefinitionSettings>
    {
    }
    public class DataRecordRuleEvaluatorDefinitionSettings : Vanrise.Entities.VRComponentTypeSettings
    {
        public override Guid VRComponentTypeConfigId
        {
            get { return new Guid("AA969BAD-225D-4D83-B76C-68BFDBC0F045"); }
        }

        public List<Guid> DataRecordStorageIds { get; set; }
        public Guid AlertRuleTypeId { get; set; }
        public bool AreDatesHardCoded { get; set; }
        public DataRecordRuleEvaluatorDefinitionSecurity Security { get; set; }

    }

    public class DataRecordRuleEvaluatorDefinitionSecurity
    {
        public RequiredPermissionSettings ViewPermission { get; set; }
        public RequiredPermissionSettings StartInstancePermission { get; set; }
    }
}
