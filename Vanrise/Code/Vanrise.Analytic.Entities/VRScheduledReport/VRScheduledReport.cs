using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.Analytic.Entities
{
    public class VRScheduledReport
    {
        public Guid VRScheduledReportId { get; set; }
        public string Name { get; set; }
        public VRScheduledReportSettings Settings { get; set; }
    }
    public class VRScheduledReportSettings
    {
        public VRTimePeriod TimePeriod { get; set; }
        public Guid VRReportTypeId { get; set; }
        public VRScheduledReportAction ReportAction { get; set; }
        public VRScheduledReportHandler ReportHandler { get; set; }
        public List<VRScheduledReportFilterField> FilterFields { get; set; }
    }

    public class VRScheduledReportHandler
    {
        public List<VRScheduledReportHandlerField> Fields { get; set; }
        public VRAutomatedReportHandlerSettings Handler { get; set; }

    }
    public class VRScheduledReportHandlerField
    {
        public string FieldName { get; set; }
    }

    public class VRScheduledReportAction
    {
        public GenerateFilesActionType ActionType { get; set; }
        public List<VRScheduledReportAttachement> Attachements { get; set; }
    }

    public class VRScheduledReportAttachement
    {
        public Guid VRReportTypeAttachementId { get; set; }
    }
    public class VRScheduledReportFilterField
    {
        public string FieldName { get; set; }
        public List<Object> FieldValues { get; set; }
    }
}
