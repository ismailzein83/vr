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
        public List<VRAutomatedReportFileNamePart> FileNameParts { get; set; }
         
    }
    public class VRAutomatedReportFileNamePart
    {
        public string VariableName { get; set; }
        public string Description { get; set; }
        public VRConcatenatedPartSettings<IVRAutomatedReportFileNamePartConcatenatedPartContext> Settings { get; set; }

    }
    public interface IVRAutomatedReportFileNamePartConcatenatedPartContext
    {
        Guid? TaskId { get; set; }

    }
    public class VRAutomatedReportFileNamePartConcatenatedPartContext : IVRAutomatedReportFileNamePartConcatenatedPartContext
    {
        public Guid? TaskId { get; set; }
    }
}
