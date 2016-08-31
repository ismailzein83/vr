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
        public List<RDLCReportURL> RDLCReportsURLs { get; set; }
        public string ReportURL { get; set; }
        public List<RDLCReportParameter> MainReportParameters { get; set; }
        public List<RDLCReportDataSource> MainReportDataSources { get; set; }
        public List<RDLCSubReport> SubReports { get; set; }
      
    }
    public class RDLCReportURL
    {
        public string ReportURL { get; set; }
        public GenericData.Entities.RecordFilterGroup FilterGroup { get; set; }
    }
    public class RDLCSubReport
    {
        public string SubReportName { get; set; }
        public List<RDLCReportDataSource> SubReportDataSources { get; set; }

    }
    public class RDLCReportDataSource
    {
        public string DataSourceName { get; set; }
        public RDLCReportDataSourceSettings  Settings { get; set; }
    }
    public class RDLCReportParameter
    {
        public string ParameterName { get; set; }
        public RDLCReportParameterValue Value { get; set; }
        public bool IsVisible { get; set; }
    }
}
