using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.GenericData.Notification
{
    public class LoadDataRecordRuleEvaluatorDefinition:CodeActivity
    {
        [RequiredArgument]
        public InArgument<Guid> DataRecordRuleEvaluatorDefinitionId { get; set; }

        [RequiredArgument]
        public OutArgument<Guid> AlertRuleTypeId { get; set; }

        [RequiredArgument]
        public OutArgument<List<Guid>> DataRecordStorageIds { get; set; }

        protected override void Execute(CodeActivityContext context)
        {

           /// context.SetValue(this.AlertRuleTypeId, value);
            throw new NotImplementedException();
        }
    }
}
