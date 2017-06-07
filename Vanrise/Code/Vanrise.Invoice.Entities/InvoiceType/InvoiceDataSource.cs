using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Invoice.Entities
{
    public enum CompareOperator { StartWith = 1, Equal = 2, Contains = 3, EndWith = 4 }
    public class InvoiceDataSource
    {
        public string DataSourceName { get; set; }
        public ItemsFilter ItemsFilter { get; set; }
        public InvoiceDataSourceSettings Settings { get; set; }
    }
}
