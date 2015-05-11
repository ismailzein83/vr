using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Fzero.FraudAnalysis.Entities
{
    public class AggregateDefinition
    {
        public string Name { get; set; }

        public IAggregate Aggregation { get; set; }

    }
}
