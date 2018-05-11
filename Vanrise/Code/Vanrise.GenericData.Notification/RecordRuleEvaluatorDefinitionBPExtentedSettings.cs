using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess.Entities;
using Vanrise.GenericData.Notification.Arguments;
using Vanrise.Common;
using Vanrise.Security.Entities;
using Vanrise.BusinessProcess.Business;
namespace Vanrise.GenericData.Notification
{
    public class RecordRuleEvaluatorDefinitionBPExtentedSettings : DefaultBPDefinitionExtendedSettings
    {
        public override RequiredPermissionSettings GetViewInstanceRequiredPermissions(IBPDefinitionGetViewInstanceRequiredPermissionsContext context)
        {
            var dataRecordRuleEvaluatorProcessInput = context.InputArg.CastWithValidate<DataRecordRuleEvaluatorProcessInput>("context.InputArg");
            return new DataRecordRuleEvaluatorDefinitionManager().GetViewInstanceRequiredPermissions(dataRecordRuleEvaluatorProcessInput.DataRecordRuleEvaluatorDefinitionId);
        }
        public override bool DoesUserHaveViewAccess(IBPDefinitionDoesUserHaveViewAccessContext context)
        {
            return new DataRecordRuleEvaluatorDefinitionManager().DoesUserHaveViewAccess(context.UserId);
        }
        public override bool DoesUserHaveStartAccess(IBPDefinitionDoesUserHaveStartAccessContext context)
        {
            return new DataRecordRuleEvaluatorDefinitionManager().DoesUserHaveStartNewInstanceAccess(context.UserId);
        }
        public override bool DoesUserHaveScheduleTaskAccess(IBPDefinitionDoesUserHaveScheduleTaskContext context)
        {
            return new DataRecordRuleEvaluatorDefinitionManager().DoesUserHaveStartNewInstanceAccess(context.UserId);
        }
        public override bool DoesUserHaveStartSpecificInstanceAccess(IBPDefinitionDoesUserHaveStartSpecificInstanceAccessContext context)
        {
            var dataRecordRuleEvaluatorProcessInput = context.InputArg.CastWithValidate<DataRecordRuleEvaluatorProcessInput>("context.InputArg");
            return new DataRecordRuleEvaluatorDefinitionManager().DoesUserHaveStartSpecificInstanceAccess(context.DefinitionContext.UserId, dataRecordRuleEvaluatorProcessInput.DataRecordRuleEvaluatorDefinitionId);
        }
        public override bool DoesUserHaveScheduleSpecificTaskAccess(IBPDefinitionDoesUserHaveScheduleSpecificTaskAccessContext context)
        {
            var dataRecordRuleEvaluatorProcessInput = context.InputArg.CastWithValidate<DataRecordRuleEvaluatorProcessInput>("context.InputArg");
            return new DataRecordRuleEvaluatorDefinitionManager().DoesUserHaveStartSpecificInstanceAccess(context.DefinitionContext.UserId, dataRecordRuleEvaluatorProcessInput.DataRecordRuleEvaluatorDefinitionId);
        }
    }
}
