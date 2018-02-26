using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Invoice.Entities;

namespace Vanrise.Invoice.MainExtensions
{
    public class OpenRDLCReportAction : InvoiceActionSettings
    {
        public override string ActionTypeName { get { return "OpenRDLCReportAction"; } }
        public override Guid ConfigId { get { return new Guid("5B4BD540-832E-46E4-8C18-49073775D002"); } }
        public override InvoiceActionType Type
        {
            get { return InvoiceActionType.Download; }
        }
        public string ReportURL { get; set; }
        public string ReportRuntimeURL { get; set; }
        public List<RDLCReportParameter> MainReportParameters { get; set; }
        public List<InvoiceDataSource> MainReportDataSources { get; set; }
        public List<RDLCSubReport> SubReports { get; set; }
       
    }
    public class RDLCSubReport
    {
        public string ParentDataSourceName { get; set; }
        public GenericData.Entities.RecordFilterGroup FilterGroup { get; set; }
        public string SubReportName { get; set; }
        public bool RepeatedSubReport { get; set; }
        public List<Entities.InvoiceDataSource> SubReportDataSources { get; set; }
        public List<RDLCSubReport> SubReports { get; set; }


    }
    public class RDLCReportParameter
    {
        public string ParameterName { get; set; }
        public RDLCReportParameterValue Value { get; set; }
        public bool IsVisible { get; set; }
    }

}
