using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.Entities
{
    public enum ConfigParameterName
    {
        [ConfigParameterName(DefaultValue = "03:00:00")]
        DropOldRoutingDatabasesInterval = 1,
        [ConfigParameterName(DefaultValue = "1000000")]
        BCPBatchSize = 2,
        [ConfigParameterName(DefaultValue = "100000")]
        LoadZoneBatchSize = 3,
        [ConfigParameterName(DefaultValue = "100000")]
        LoadCalculatedRatesBatchSize = 4,
        [ConfigParameterName(DefaultValue = "2")]
        RoutingCodePrefixLength = 5,
        [ConfigParameterName(DefaultValue = "1000000")]
        RouteBCPBatchSize = 6,
        [ConfigParameterName(DefaultValue = "5")]
        RepricingParallelThreads = 7,
        [ConfigParameterName(DefaultValue = "false")]
        RebuildZoneRates = 8


    }

    public class ConfigParameterNameAttribute : Attribute
    {
        public string DefaultValue { get; set; }
    }
}
