using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Business;
using Vanrise.Entities;

namespace Vanrise.Analytic.Entities
{
    public class VRAutomatedReportSettings : SettingData
    {
        public static string SETTING_TYPE = "VR_Analytic_AutomatedReportSettings";
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
        Guid? TaskId { get; set; }

    }
    public class VRAutomatedReportSerialNumberPartConcatenatedPartContext : IVRAutomatedReportSerialNumberPartConcatenatedPartContext
    {
        public Guid? TaskId { get; set; }
    }
}
