using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Analytic.Business;
using Vanrise.Analytic.Entities;
using Vanrise.Common.Business;
using Vanrise.Entities;

namespace Vanrise.Analytic.MainExtensions.AutomatedReport.Handlers
{
    public enum DateCounterType { Yearly = 0 }
    public class VRAutomatedReportSequenceSerialNumberPart : VRConcatenatedPartSettings<IVRAutomatedReportSerialNumberPartConcatenatedPartContext>
    {
        public override Guid ConfigId { get { return new Guid("9CC73443-2A1A-4405-A1ED-1DE27B9DCB42"); } }
        public DateCounterType? DateCounterType { get; set; }
        public int PaddingLeft { get; set; }
        public override string GetPartText(IVRAutomatedReportSerialNumberPartConcatenatedPartContext context)
        { 
            long initialSequenceValue = new Vanrise.Analytic.Business.ConfigManager().GetSerialNumberPartInitialSequence();
            if (context.TaskId.HasValue)
            {
                StringBuilder sequenceKey = new StringBuilder();
                StringBuilder sequenceGroup = new StringBuilder();
                sequenceGroup.Append("OVERALL");

                VRSequenceManager manager = new VRSequenceManager();
                if (this.DateCounterType.HasValue)
                {
                    if (sequenceKey.Length > 0)
                        sequenceKey.Append("_");
                    sequenceGroup.Append("_");
                    sequenceGroup.Append(Common.Utilities.GetEnumDescription(this.DateCounterType.Value));
                    switch (this.DateCounterType)
                    {
                        case Vanrise.Analytic.MainExtensions.AutomatedReport.Handlers.DateCounterType.Yearly:
                            sequenceKey.Append(string.Format("{0}_{1}", DateTime.Today.Year, DateTime.Today.Year + 1));
                            break;
                    }
                }
                var sequenceNumber = manager.GetNextSequenceValue(sequenceGroup.ToString(),context.TaskId.Value, sequenceKey.ToString(), initialSequenceValue);
                return sequenceNumber.ToString().PadLeft(this.PaddingLeft, '0');
            }
            else
            {
                return initialSequenceValue.ToString().PadLeft(this.PaddingLeft, '0');
            }
        }

    }
}
