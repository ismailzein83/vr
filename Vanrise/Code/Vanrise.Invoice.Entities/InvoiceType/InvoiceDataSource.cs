using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Invoice.Entities
{
    public class InvoiceDataSource
    {
        public string DataSourceName { get; set; }
        public ItemsFilter ItemsFilter { get; set; }
        public InvoiceDataSourceSettings Settings { get; set; }
    }
}
