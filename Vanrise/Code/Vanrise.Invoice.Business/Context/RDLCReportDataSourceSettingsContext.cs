using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Invoice.Entities;

namespace Vanrise.Invoice.Business.Context
{
    public class RDLCReportDataSourceSettingsContext : IInvoiceDataSourceSettingsContext
    {
        public Func<string,string, IEnumerable<dynamic>> DataSourceItemsFunc { get; set; }
        public IInvoiceActionContext InvoiceActionContext { get; set; }
        public IEnumerable<dynamic> GetDataSourceItems(string dataSourceName,string reportName)
        {
            return DataSourceItemsFunc(dataSourceName, reportName);
        }



      
    }
}
