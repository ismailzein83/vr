using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.Entities
{
    public enum ConfigParameterName
    {
        [ConfigParameterName(DefaultValue="03:00:00")]
        DropOldRoutingDatabasesInterval = 1,
        [ConfigParameterName(DefaultValue = "500000")]
        BCPBatchSize = 2,
        [ConfigParameterName(DefaultValue = "50000")]
        LoadZoneBatchSize = 3,
        [ConfigParameterName(DefaultValue = "50000")]
        LoadCalculatedRates = 4
    }

    public class ConfigParameterNameAttribute : Attribute
    {
        public string DefaultValue { get; set; }
    }
}
