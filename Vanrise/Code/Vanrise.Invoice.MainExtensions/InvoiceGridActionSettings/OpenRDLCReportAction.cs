using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Invoice.Entities;

namespace Vanrise.Invoice.MainExtensions
{
    public class OpenRDLCReportAction : InvoiceGridActionSettings
    {
        Guid _configId;
        public override Guid ConfigId { get { return _configId; } set { _configId = new Guid("5B4BD540-832E-46E4-8C18-49073775D002"); } }
        public string ReportURL { get; set; }
        public List<RDLCReportParameter> MainReportParameters { get; set; }
        public List<InvoiceDataSource> MainReportDataSources { get; set; }
        public List<RDLCSubReport> SubReports { get; set; }
      
    }
    public class RDLCSubReport
    {
        public InvoiceDataSource ParentSubreportDataSource { get; set; }
        public GenericData.Entities.RecordFilterGroup FilterGroup { get; set; }
        public string SubReportName { get; set; }
        public bool RepeatedSubReport { get; set; }
        public List<Entities.InvoiceDataSource> SubReportDataSources { get; set; }

    }
    public class RDLCReportParameter
    {
        public string ParameterName { get; set; }
        public RDLCReportParameterValue Value { get; set; }
        public bool IsVisible { get; set; }
    }

}
