using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Integration.Entities
{
    public class DataSource
    {
        public int DataSourceId { get; set; }

        public string Name { get; set; }

        public int AdapterTypeId { get; set; }

        public AdapterTypeInfo AdapterInfo { get; set; }

        public BaseAdapterState AdapterState { get; set; }

        public int TaskId { get; set; }

        public DataSourceSettings Settings { get; set; }
    }

    public class DataSourceSettings
    {
        public BaseAdapterArgument AdapterArgument { get; set; }

        public string MapperCustomCode { get; set; }

        public int ExecutionFlowId { get; set; }
    }
}
