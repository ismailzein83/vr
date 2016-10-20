using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Integration.Entities
{
    public class DataSourceDetail
    {
        public DataSource Entity { get; set; }
        public string AdapterName { get; set; }
        public DataSourceAdapterType AdapterInfo { get; set; }
    }
}
