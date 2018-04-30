using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;

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
            Guid dataRecordRuleEvaluatorDefinitionId = this.DataRecordRuleEvaluatorDefinitionId.Get(context);
           
            DataRecordRuleEvaluatorDefinitionManager dataRecordRuleEvaluatorDefinitionManager = new DataRecordRuleEvaluatorDefinitionManager();
            DataRecordRuleEvaluatorDefinitionSettings dataRecordRuleEvaluatorDefinitionSettings = dataRecordRuleEvaluatorDefinitionManager.GetDataRecordRuleEvaluatorDefinitionSettings(dataRecordRuleEvaluatorDefinitionId);
           
            this.AlertRuleTypeId.Set(context, dataRecordRuleEvaluatorDefinitionSettings.AlertRuleTypeId);
            this.DataRecordStorageIds.Set(context, dataRecordRuleEvaluatorDefinitionSettings.DataRecordStorageIds);
                        
        }
    }
}
