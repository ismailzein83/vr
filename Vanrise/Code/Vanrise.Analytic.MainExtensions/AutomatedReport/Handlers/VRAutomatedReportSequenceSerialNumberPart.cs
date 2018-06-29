using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Analytic.Business;
using Vanrise.Analytic.Entities.AutomatedReport.Handler;
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
            StringBuilder sequenceKey = new StringBuilder();
            StringBuilder sequenceGroup = new StringBuilder();
            sequenceGroup.Append("OVERALL");

            long initialSequenceValue = new Vanrise.Analytic.Business.ConfigManager().GetSerialNumberPartInitialSequence();

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
            VRSequenceManager manager = new VRSequenceManager();
            Guid sequenceid = new Guid();
            //fix sequence id
            var sequenceNumber = manager.GetNextSequenceValue(sequenceGroup.ToString(), sequenceid, sequenceKey.ToString(), initialSequenceValue);
            return sequenceNumber.ToString().PadLeft(this.PaddingLeft, '0');
        }

    }
}
