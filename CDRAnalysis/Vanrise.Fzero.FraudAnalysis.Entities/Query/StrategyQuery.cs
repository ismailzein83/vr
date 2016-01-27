using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Fzero.FraudAnalysis.Entities
{
    public class StrategyQuery
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public IEnumerable<int> UserIds { get; set; }

        public IEnumerable<PeriodEnum> PeriodIds { get; set; }

        public IEnumerable<StrategyKindEnum> Kinds { get; set; }

        public IEnumerable<StrategyStatusEnum> Statuses { get; set; }

        public DateTime? FromDate { get; set; }

        public DateTime? ToDate { get; set; }

    }
}
