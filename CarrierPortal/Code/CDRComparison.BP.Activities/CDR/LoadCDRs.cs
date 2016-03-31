﻿using CDRComparison.Business;
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

        #region Fields

        NormalizeNumberSettings _cdpnNormalizationSettings;
        NormalizeNumberSettings _cgpnNormalizationSettings;
        
        #endregion

        protected override void DoWork(LoadCDRsInput inputArgument, AsyncActivityHandle handle)
        {
            SetNumberNormalizationSettings(inputArgument.CDRSource.NormalizationRules);

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
                        DurationInSec = cdr.DurationInSec,
                        IsPartnerCDR = inputArgument.IsPartnerCDRs
                    };

                    var cdpnNormalizationContext = new NormalizeRuleContext() { Value = cdr.CDPN };
                    _cdpnNormalizationSettings.ApplyNormalizationRule(cdpnNormalizationContext);
                    item.CDPN = cdpnNormalizationContext.NormalizedValue;

                    if (_cgpnNormalizationSettings != null)
                    {
                        var cgpnNormalizationContext = new NormalizeRuleContext() { Value = cdr.CGPN };
                        _cgpnNormalizationSettings.ApplyNormalizationRule(cgpnNormalizationContext);
                        item.CGPN = cgpnNormalizationContext.NormalizedValue;
                    }
                    
                    list.Add(item);
                }
                var cdrBatch = new CDRBatch() { CDRs = list };
                inputArgument.OutputQueue.Enqueue(cdrBatch);
            };

            var context = new ReadCDRsFromSourceContext(onCDRsReceived);
            inputArgument.CDRSource.ReadCDRs(context);
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

        void SetNumberNormalizationSettings(IEnumerable<NormalizationRule> normalizationRules)
        {
            if (normalizationRules == null)
                throw new ArgumentNullException("normalizationRules");
            
            NormalizationRule cdpnNormalizationRule = normalizationRules.FindRecord(itm => itm.FieldToNormalize == "CDPN");
            
            if (cdpnNormalizationRule == null)
                throw new NullReferenceException("cdpnNormalizationRule");
            if (cdpnNormalizationRule.NormalizationSettings == null)
                throw new NullReferenceException("cdpnNormalizationRule.NormalizationSettings");

            NormalizationRule cgpnNormalizationRule = normalizationRules.FindRecord(itm => itm.FieldToNormalize == "CGPN");

            _cdpnNormalizationSettings = cdpnNormalizationRule.NormalizationSettings;
            _cgpnNormalizationSettings = (cgpnNormalizationRule != null) ? cgpnNormalizationRule.NormalizationSettings : null;
        }
        
        #endregion
    }
}
