using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Integration.Entities
{
    public class DataSourceDetail
    {
        public int DataSourceId { get; set; }

        public string Name { get; set; }

        public int AdapterTypeId { get; set; }

        public string AdapterName { get; set; }

        public AdapterTypeInfo AdapterInfo { get; set; }

        public BaseAdapterState AdapterState { get; set; }

        public int TaskId { get; set; }

        public bool IsEnabled { get; set; }

        public DataSourceSettings Settings { get; set; }
    }
}
