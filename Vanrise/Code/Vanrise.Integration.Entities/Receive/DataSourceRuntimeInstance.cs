using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Integration.Entities
{
    public class DataSourceRuntimeInstance
    {
        public Guid DataSourceRuntimeInstanceId { get; set; }

        public Guid DataSourceId { get; set; }

        public DateTime CreatedTime { get; set; }
    }
}