﻿using System;
using System.Collections.Generic;

namespace Vanrise.Fzero.FraudAnalysis.Entities
{
    public class Strategy
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public List<StrategyCriteria> StrategyCriterias { get; set; }

        public List<StrategyPeriod> StrategyPeriods { get; set; }

        public List<StrategyLevel> StrategyLevels { get; set; }
    }

    public class StrategyCriteria
    {
        public int CriteriaId { get; set; }

        public Decimal Threshold { get; set; }
    }

    public class StrategyPeriod
    {
        public int CriteriaId { get; set; }
        public int Value { get; set; }

        public Enums.Period? Period { get; set; }
    }

    public class StrategyLevel
    {
        public int SuspectionLevelId { get; set; }

        public List<StrategyLevelCriteria> StrategyLevelCriterias { get; set; }
    }

    public class StrategyLevelCriteria
    {
        public int CriteriaId { get; set; }

        public decimal Percentage { get; set; }
    }
}