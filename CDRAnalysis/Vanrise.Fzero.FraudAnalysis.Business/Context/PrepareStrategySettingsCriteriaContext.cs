using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Fzero.FraudAnalysis.Entities;

namespace Vanrise.Fzero.FraudAnalysis.Business
{
    public class PrepareStrategySettingsCriteriaContext : IPrepareStrategySettingsCriteriaContext
    {
        public object PreparedData { get; set; }
    }
}
