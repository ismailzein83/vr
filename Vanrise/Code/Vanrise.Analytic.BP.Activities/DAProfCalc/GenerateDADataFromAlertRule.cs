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
        public InArgument<DateTime> EffectiveDate { get; set; }

        [RequiredArgument]
        public InArgument<DAProfCalcAnalysisPeriod> MinDAProfCalcAnalysisPeriod { get; set; }

        [RequiredArgument]
        public InArgument<DAProfCalcAnalysisPeriod> MaxDAProfCalcAnalysisPeriod { get; set; }

        [RequiredArgument]
        public OutArgument<Guid> DataAnalysisDefinitionId { get; set; }

        [RequiredArgument]
        public OutArgument<List<DAProfCalcExecInput>> DAProfCalcExecInputs { get; set; }

        [RequiredArgument]
        public OutArgument<List<Guid>> SourceRecordStorageIds { get; set; }

        [RequiredArgument]
        public OutArgument<DateTime> FromTime { get; set; }

        [RequiredArgument]
        public OutArgument<DateTime> ToTime { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            DateTime effectiveDate = EffectiveDate.Get(context);

            DAProfCalcAnalysisPeriod maxDAProfCalcAnalysisPeriod = this.MaxDAProfCalcAnalysisPeriod.Get(context);
            DAProfCalcAnalysisPeriod minDAProfCalcAnalysisPeriod = this.MinDAProfCalcAnalysisPeriod.Get(context);
            if (maxDAProfCalcAnalysisPeriod == null)
                throw new NullReferenceException("maxDAProfCalcAnalysisPeriod");

            int maxPeriodInMinutes = maxDAProfCalcAnalysisPeriod.GetPeriodInMinutes();
            int minPeriodInMinutes = minDAProfCalcAnalysisPeriod != null ? minDAProfCalcAnalysisPeriod.GetPeriodInMinutes() : 0;

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

            int maxAlertRulePeriodInMinutes = 0;
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

                    int alertRulePeriodInMinutes = daProfCalcAlertRuleSettings.DAProfCalcAnalysisPeriod.GetPeriodInMinutes();
                    if (alertRulePeriodInMinutes > maxPeriodInMinutes || alertRulePeriodInMinutes <= minPeriodInMinutes)
                        continue;

                    maxAlertRulePeriodInMinutes = Math.Max(maxAlertRulePeriodInMinutes, alertRulePeriodInMinutes);
                    DAProfCalcExecInput daProfCalcExecInput = new DAProfCalcExecInput()
                        {
                            OutputItemDefinitionId = daProfCalcAlertRuleSettings.OutputItemDefinitionId,
                            DAProfCalcPayload = new DAProfCalcAlertRuleExecPayload()
                            {
                                AlertRuleIds = new List<long>() { alertRule.VRAlertRuleId },
                                AlertRuleTypeId = alertRuleTypeId
                            },
                            DataAnalysisRecordFilter = daProfCalcAlertRuleSettings.DataAnalysisFilterGroup,
                            GroupingFieldNames = daProfCalcAlertRuleSettings.GroupingFieldNames,
                            DAProfCalcAnalysisPeriod = daProfCalcAlertRuleSettings.DAProfCalcAnalysisPeriod
                        };

                    bool hasMatching = false;
                    foreach (DAProfCalcExecInput existingDAProfCalcExecInput in daProfCalcExecInputs)
                    {
                        if (AreDAProfCalcExecInputMatching(daProfCalcExecInput, existingDAProfCalcExecInput))
                        {
                            (existingDAProfCalcExecInput.DAProfCalcPayload as DAProfCalcAlertRuleExecPayload).AlertRuleIds.Add(alertRule.VRAlertRuleId);
                            hasMatching = true;
                            break;
                        }
                    }
                    if (!hasMatching)
                        daProfCalcExecInputs.Add(daProfCalcExecInput);
                }
            }

            DateTime fromDate = effectiveDate.AddMinutes(-1 * maxAlertRulePeriodInMinutes);
            FromTime.Set(context, fromDate);
            ToTime.Set(context, effectiveDate);

            DataAnalysisDefinitionId.Set(context, dataAnalysisDefinitionId);
            DAProfCalcExecInputs.Set(context, daProfCalcExecInputs);
            SourceRecordStorageIds.Set(context, sourceRecordStorageIds);

            context.GetSharedInstanceData().WriteTrackingMessage(Vanrise.Entities.LogEntryType.Information, "Data Analysis Profiling and Calculation Execution Inputs loaded.", null);
        }

        private bool AreDAProfCalcExecInputMatching(DAProfCalcExecInput firstDAProfCalcExexInput, DAProfCalcExecInput secondDAProfCalcExexInput)
        {
            if (firstDAProfCalcExexInput.OutputItemDefinitionId != secondDAProfCalcExexInput.OutputItemDefinitionId)
                return false;

            if (firstDAProfCalcExexInput.DataAnalysisRecordFilter != null || secondDAProfCalcExexInput.DataAnalysisRecordFilter != null)
                return false;

            if (firstDAProfCalcExexInput.GroupingFieldNames != null && secondDAProfCalcExexInput.GroupingFieldNames == null)
                return false;

            if (firstDAProfCalcExexInput.GroupingFieldNames == null && secondDAProfCalcExexInput.GroupingFieldNames != null)
                return false;

            if (firstDAProfCalcExexInput.DAProfCalcAnalysisPeriod.GetPeriodInMinutes() != secondDAProfCalcExexInput.DAProfCalcAnalysisPeriod.GetPeriodInMinutes())
                return false;

            var inFirstOnly = firstDAProfCalcExexInput.GroupingFieldNames.Except(secondDAProfCalcExexInput.GroupingFieldNames);
            if (inFirstOnly.Any())
                return false;

            var inSecondOnly = secondDAProfCalcExexInput.GroupingFieldNames.Except(firstDAProfCalcExexInput.GroupingFieldNames);
            if (inSecondOnly.Any())
                return false;

            return true;
        }
    }
}