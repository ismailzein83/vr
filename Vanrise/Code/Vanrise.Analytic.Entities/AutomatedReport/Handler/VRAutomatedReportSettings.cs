using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Business;
using Vanrise.Entities;

namespace Vanrise.Analytic.Entities.AutomatedReport.Handler
{
    public class VRAutomatedReportSettings : SettingData
    {
        public static string SETTING_TYPE = "VR_Analytic_VRAutomatedReportSettings";
        public List<VRAutomatedReportSerialNumberPart> SerialNumberParts { get; set; }

    }
    public class VRAutomatedReportSerialNumberPart
    {
        public string VariableName { get; set; }
        public string Description { get; set; }
        public VRConcatenatedPartSettings<IVRAutomatedReportSerialNumberPartConcatenatedPartContext> Settings { get; set; }

    }
    public interface IVRAutomatedReportSerialNumberPartConcatenatedPartContext
    {

    }
    public class VRAutomatedReportSerialNumberPartConcatenatedPartContext : IVRAutomatedReportSerialNumberPartConcatenatedPartContext
    {

    }
    public class VRAutomatedReportDateSerialNumberPart : VRConcatenatedPartSettings<IVRAutomatedReportSerialNumberPartConcatenatedPartContext>
    {
        public override Guid ConfigId { get { return new Guid("A194AAC8-0675-4100-8A8B-1FBE4105FE09"); } }
        public string DateFormat { get; set; }
        public override string GetPartText(IVRAutomatedReportSerialNumberPartConcatenatedPartContext context)
        {
            var date = DateTime.Now;
            if (!String.IsNullOrEmpty(this.DateFormat))
            {
                return date.ToString(this.DateFormat);
            }
            return date.ToString();
        }
    }

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
            return "";
        }
      
    }
}
