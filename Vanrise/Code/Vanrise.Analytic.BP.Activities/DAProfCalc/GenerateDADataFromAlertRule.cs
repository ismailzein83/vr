using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using Vanrise.Analytic.Entities;
using Vanrise.BusinessProcess;
using Vanrise.Entities;
using Vanrise.GenericData.Entities;
using Vanrise.Notification.Business;
using Vanrise.Notification.Entities;

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
            if (maxDAProfCalcAnalysisPeriod == null && minDAProfCalcAnalysisPeriod == null)
                throw new NullReferenceException("maxDAProfCalcAnalysisPeriod and minDAProfCalcAnalysisPeriod");

            int maxPeriodInMinutes = maxDAProfCalcAnalysisPeriod != null ? maxDAProfCalcAnalysisPeriod.GetPeriodInMinutes() : Int32.MaxValue;
            int minPeriodInMinutes = minDAProfCalcAnalysisPeriod != null ? minDAProfCalcAnalysisPeriod.GetPeriodInMinutes() : 0;

            Guid alertRuleTypeId = this.AlertRuleTypeId.Get(context);
            VRAlertRuleType alertRuleType = new VRAlertRuleTypeManager().GetVRAlertRuleType(alertRuleTypeId);

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

            List<VRAlertRule> alertRules = new VRAlertRuleManager().GetActiveRules(alertRuleTypeId);

            DateTime minFromTime = DateTime.MaxValue;
            DateTime maxToTime = DateTime.MinValue;

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

                    VRTimePeriodContext daProfCalcAlertRuleTimePeriodContext = new VRTimePeriodContext() { EffectiveDate = effectiveDate };

                    var daProfCalcAlertRuleTimePeriod = daProfCalcAlertRuleSettings.TimePeriod;
                    daProfCalcAlertRuleTimePeriod.GetTimePeriod(daProfCalcAlertRuleTimePeriodContext);

                    DateTime fromDate = daProfCalcAlertRuleTimePeriodContext.FromTime;
                    TimeSpan ts = effectiveDate.Subtract(fromDate);

                    if (ts.TotalMinutes > maxPeriodInMinutes || ts.TotalMinutes <= minPeriodInMinutes)
                        continue;

                    RecordFilterGroup alertRuleTypeRecordFilter = null;
                    if (daProfCalcAlertRuleSettings.DAProfCalcAlertRuleFilter != null)
                        alertRuleTypeRecordFilter = daProfCalcAlertRuleSettings.DAProfCalcAlertRuleFilter.GetRecordFilterGroup(new DAProfCalcGetRecordFilterGroupContext());

                    if (minFromTime > daProfCalcAlertRuleTimePeriodContext.FromTime)
                        minFromTime = daProfCalcAlertRuleTimePeriodContext.FromTime;

                    if (maxToTime < daProfCalcAlertRuleTimePeriodContext.ToTime)
                        maxToTime = daProfCalcAlertRuleTimePeriodContext.ToTime;

                    DAProfCalcExecInput daProfCalcExecInput = new DAProfCalcExecInput()
                    {
                        OutputItemDefinitionId = daProfCalcAlertRuleSettings.OutputItemDefinitionId,
                        DAProfCalcPayload = new DAProfCalcAlertRuleExecPayload()
                        {
                            AlertRuleIds = new List<long>() { alertRule.VRAlertRuleId },
                            AlertRuleTypeId = alertRuleTypeId
                        },
                        DataAnalysisRecordFilter = BuildDataAnalysisRecordFilter(alertRuleTypeRecordFilter, daProfCalcAlertRuleSettings.DataAnalysisFilterGroup),
                        GroupingFieldNames = daProfCalcAlertRuleSettings.GroupingFieldNames,
                        FromTime = daProfCalcAlertRuleTimePeriodContext.FromTime,
                        ToTime = daProfCalcAlertRuleTimePeriodContext.ToTime,
                        ParameterValues = daProfCalcAlertRuleSettings.ParameterValues
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

            FromTime.Set(context, minFromTime);
            ToTime.Set(context, maxToTime);

            DataAnalysisDefinitionId.Set(context, dataAnalysisDefinitionId);
            DAProfCalcExecInputs.Set(context, daProfCalcExecInputs);
            SourceRecordStorageIds.Set(context, sourceRecordStorageIds);

            context.GetSharedInstanceData().WriteTrackingMessage(Vanrise.Entities.LogEntryType.Information, "Data Analysis Profiling and Calculation Execution Inputs loaded.", null);
        }

        private RecordFilterGroup BuildDataAnalysisRecordFilter(RecordFilterGroup alertRuleTypeRecordFilter, RecordFilterGroup alertRuleRecordFilter)
        {
            if (alertRuleTypeRecordFilter == null)
                return alertRuleRecordFilter;

            if (alertRuleRecordFilter == null)
                return alertRuleTypeRecordFilter;

            RecordFilterGroup recordFilterGroup = new RecordFilterGroup()
            {
                Filters = new List<RecordFilter>() { alertRuleTypeRecordFilter, alertRuleRecordFilter },
                LogicalOperator = RecordQueryLogicalOperator.And
            };

            return recordFilterGroup;
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

            if (DateTime.Compare(firstDAProfCalcExexInput.FromTime, secondDAProfCalcExexInput.FromTime) != 0)
                return false;

            if (DateTime.Compare(firstDAProfCalcExexInput.ToTime, secondDAProfCalcExexInput.ToTime) != 0)
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