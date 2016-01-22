﻿
using Vanrise.Fzero.CDRImport.Entities;
using Vanrise.Fzero.Entities;

namespace Vanrise.Fzero.FraudAnalysis.Aggregates
{
    public class AggregateDefinition
    {
        public int Id { get; set; }

        public string KeyName { get; set; }

        public BaseAggregate Aggregation { get; set; }

        public OperatorType OperatorTypeAllowed { get; set; }
    }
}
