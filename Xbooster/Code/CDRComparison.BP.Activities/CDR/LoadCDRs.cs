using CDRComparison.Business;
using CDRComparison.Entities;
using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess;
using Vanrise.Common;
using Vanrise.Queueing;
using Vanrise.Rules.Normalization;

namespace CDRComparison.BP.Activities
{
    #region Argument Classes

    public class LoadCDRsInput
    {
        public CDRSource CDRSource { get; set; }

        public bool IsPartnerCDRs { get; set; }

        public BaseQueue<CDRBatch> OutputQueue { get; set; }
    }

    #endregion

    public sealed class LoadCDRs : BaseAsyncActivity<LoadCDRsInput>
    {
        #region Arguments

        [RequiredArgument]
        public InArgument<CDRSource> CDRSource { get; set; }

        [RequiredArgument]
        public InArgument<bool> IsPartnerCDRs { get; set; }

        [RequiredArgument]
        public InOutArgument<BaseQueue<CDRBatch>> OutputQueue { get; set; }

        #endregion

        protected override void DoWork(LoadCDRsInput inputArgument, AsyncActivityHandle handle)
        {
            handle.SharedInstanceData.WriteTrackingMessage(Vanrise.Entities.LogEntryType.Information, "Started Loading CDRs from {0} Source...", !inputArgument.IsPartnerCDRs ? "System" : "Partner");

            NormalizeNumberSettings cdpnNormalizationSettings;
            NormalizeNumberSettings cgpnNormalizationSettings;
            SetNumberNormalizationSettings(inputArgument.CDRSource.NormalizationRules, out cdpnNormalizationSettings, out cgpnNormalizationSettings);

            long totalCount = 0;

            Action<IEnumerable<CDR>> onCDRsReceived = (cdrs) =>
            {
                var list = new List<CDR>();
                foreach (CDR cdr in cdrs)
                {
                    var item = new CDR()
                    {
                        OriginalCDPN = cdr.CDPN,
                        OriginalCGPN = cdr.CGPN,
                        Time = cdr.Time,
                        IsPartnerCDR = inputArgument.IsPartnerCDRs
                    };

                    item.DurationInSec = (inputArgument.CDRSource.DurationTimeUnit == CDRSourceTimeUnitEnum.Minutes) ? (cdr.DurationInSec * 60) : cdr.DurationInSec;
                    NormalizeNumbers(item, cdpnNormalizationSettings, cgpnNormalizationSettings);

                    list.Add(item);
                }
                var cdrBatch = new CDRBatch() { CDRs = list };
                totalCount += cdrs.Count();
                handle.SharedInstanceData.WriteTrackingMessage(Vanrise.Entities.LogEntryType.Information, "{0} CDRs loaded from {1} Source", totalCount, !inputArgument.IsPartnerCDRs ? "System" : "Partner");
                inputArgument.OutputQueue.Enqueue(cdrBatch);
            };

            var context = new ReadCDRsFromSourceContext(onCDRsReceived);
            inputArgument.CDRSource.ReadCDRs(context);
            handle.SharedInstanceData.WriteTrackingMessage(Vanrise.Entities.LogEntryType.Information, "Finished Loading CDRs from {0} Source", !inputArgument.IsPartnerCDRs ? "System" : "Partner");
        }

        protected override void OnBeforeExecute(AsyncCodeActivityContext context, AsyncActivityHandle handle)
        {
            if (this.OutputQueue.Get(context) == null)
                this.OutputQueue.Set(context, new MemoryQueue<CDRBatch>());
            base.OnBeforeExecute(context, handle);
        }

        protected override LoadCDRsInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new LoadCDRsInput()
            {
                CDRSource = this.CDRSource.Get(context),
                IsPartnerCDRs = this.IsPartnerCDRs.Get(context),
                OutputQueue = this.OutputQueue.Get(context)
            };
        }

        #region Private Methods

        void SetNumberNormalizationSettings(IEnumerable<NormalizationRule> normalizationRules, out NormalizeNumberSettings cdpnNormalizationSettings, out NormalizeNumberSettings cgpnNormalizationSettings)
        {
            if (normalizationRules == null)
            {
                cdpnNormalizationSettings = null;
                cgpnNormalizationSettings = null;
                return;
            }

            NormalizationRule cdpnNormalizationRule = normalizationRules.FindRecord(itm => itm.FieldToNormalize == "CDPN");
            NormalizationRule cgpnNormalizationRule = normalizationRules.FindRecord(itm => itm.FieldToNormalize == "CGPN");

            cdpnNormalizationSettings = (cdpnNormalizationRule != null) ? cdpnNormalizationRule.NormalizationSettings : null;
            cgpnNormalizationSettings = (cgpnNormalizationRule != null) ? cgpnNormalizationRule.NormalizationSettings : null;
        }

        void NormalizeNumbers(CDR cdr, NormalizeNumberSettings cdpnNormalizationSettings, NormalizeNumberSettings cgpnNormalizationSettings)
        {
            if (cdpnNormalizationSettings != null)
            {
                var cdpnNormalizationContext = new NormalizeRuleContext() { Value = cdr.OriginalCDPN };
                cdpnNormalizationSettings.ApplyNormalizationRule(cdpnNormalizationContext);
                cdr.CDPN = cdpnNormalizationContext.NormalizedValue;
            }
            else
                cdr.CDPN = cdr.OriginalCDPN;

            if (cgpnNormalizationSettings != null)
            {
                var cgpnNormalizationContext = new NormalizeRuleContext() { Value = cdr.OriginalCGPN };
                cgpnNormalizationSettings.ApplyNormalizationRule(cgpnNormalizationContext);
                cdr.CGPN = cgpnNormalizationContext.NormalizedValue;
            }
            else
                cdr.CGPN = cdr.OriginalCGPN;
        }

        #endregion
    }
}
