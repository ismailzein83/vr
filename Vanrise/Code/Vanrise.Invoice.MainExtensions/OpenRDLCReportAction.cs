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
        public string ReportURL { get; set; }
        public List<RDLCReportDataSource> DataSources { get; set; }
    }
    public class RDLCReportDataSource
    {
        public string DataSourceName { get; set; }
        public RDLCReportDataSourceSettings  Settings { get; set; }
    }
    
}
