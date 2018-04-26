using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
