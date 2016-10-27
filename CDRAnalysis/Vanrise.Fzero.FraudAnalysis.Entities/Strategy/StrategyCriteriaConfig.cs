using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.Fzero.FraudAnalysis.Entities
{
    public class StrategyCriteriaConfig:ExtensionConfiguration
    {
        public const string EXTENSION_TYPE = "CDRAnalysis_FA_StrategyCriteriaConfigs";

        public string Editor { get; set; }
    }
}
