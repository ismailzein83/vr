using System;
using System.Collections.Generic;
using System.Activities;
using Vanrise.Analytic.Entities;
using Vanrise.BusinessProcess;
using Vanrise.Notification.Business;
using Vanrise.Notification.Entities;
using System.Linq;

namespace Vanrise.Analytic.BP.Activities.DAProfCalc
{
    public sealed class GenerateDADataFromAlertRule : CodeActivity
    {
        [RequiredArgument]
        public InArgument<Guid> AlertRuleTypeId { get; set; }

        [RequiredArgument]
        public OutArgument<Guid> DataAnalysisDefinitionId { get; set; }

        [RequiredArgument]
        public OutArgument<List<DAProfCalcExecInput>> DAProfCalcExecInputs { get; set; }

        [RequiredArgument]
        public OutArgument<List<Guid>> SourceRecordStorageIds { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            Guid alertRuleTypeId = this.AlertRuleTypeId.Get(context);

            VRAlertRuleTypeManager alertRuleTypeManager = new VRAlertRuleTypeManager();
            VRAlertRuleType alertRuleType = alertRuleTypeManager.GetVRAlertRuleType(alertRuleTypeId);

            if (alertRuleType.Settings == null)
                throw new NullReferenceException("alertRuleType.Settings");

            DAProfCalcAlertRuleTypeSettings alertRuleSettings = alertRuleType.Settings as DAProfCalcAlertRuleTypeSettings;
            if (alertRuleSettings == null)
                throw new Exception(String.Format("alertRuleType.Settings is not of type DAProfCalcAlertRuleTypeSettings. it is of type '0'", alertRuleSettings.GetType()));

            Guid dataAnalysisDefinitionId = alertRuleSettings.DataAnalysisDefinitionId;

            if (alertRuleSettings.SourceRecordStorages == null)
                throw new NullReferenceException("alertRuleSettings.SourceRecordStorages");
            List<Guid> sourceRecordStorageIds = alertRuleSettings.SourceRecordStorages.Select(itm => itm.DataRecordStorageId).ToList();

            List<DAProfCalcExecInput> daProfCalcExecInputs = new List<DAProfCalcExecInput>();

            VRAlertRuleManager alertRuleManager = new VRAlertRuleManager();
            List<VRAlertRule> alertRules = alertRuleManager.GetActiveRules(alertRuleTypeId);
            if (alertRules != null && alertRules.Count > 0)
            {
                foreach (VRAlertRule alertRule in alertRules)
                {
                    if (alertRule.Settings == null)
                        throw new NullReferenceException("alertRule.Settings");

                    if (alertRule.Settings.ExtendedSettings == null)
                        throw new NullReferenceException("alertRule.Settings.ExtendedSettings");

                    DAProfCalcAlertRuleSettings daProfCalcAlertRuleSettings = alertRule.Settings.ExtendedSettings as DAProfCalcAlertRuleSettings;
                    if (daProfCalcAlertRuleSettings == null)
                        throw new Exception(String.Format("alertRule.Settings.ExtendedSettings is not of type DAProfCalcAlertRuleSettings. it is of type '0'", alertRule.Settings.ExtendedSettings.GetType()));

                    DAProfCalcExecInput input = new DAProfCalcExecInput()
                    {
                        OutputItemDefinitionId = daProfCalcAlertRuleSettings.OutputItemDefinitionId,
                        DAProfCalcPayload = new DAProfCalcAlertRuleExecPayload() { AlertRuleId = alertRule.VRAlertRuleId }
                    };
                    daProfCalcExecInputs.Add(input);
                }
            }

            DataAnalysisDefinitionId.Set(context, dataAnalysisDefinitionId);
            DAProfCalcExecInputs.Set(context, daProfCalcExecInputs);
            SourceRecordStorageIds.Set(context, sourceRecordStorageIds);
            context.GetSharedInstanceData().WriteTrackingMessage(Vanrise.Entities.LogEntryType.Information, "Data Analysis Profiling and Calculation Execution Inputs loaded.", null);
        }
    }
}