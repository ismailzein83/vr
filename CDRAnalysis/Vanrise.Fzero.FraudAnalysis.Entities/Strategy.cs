using System;
using System.Collections.Generic;

namespace Vanrise.Fzero.FraudAnalysis.Entities
{
    public class Strategy
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public bool IsDefault { get; set; }

        public string StrategyContent { get; set;}

        public int GapBetweenConsecutiveCalls { get; set; }

        public int MaxLowDurationCall { get; set; }

        public int MinimumCountofCallsinActiveHour { get; set; }

        List<Hour> _peakHours;
        public List<Hour> PeakHours
        {
            get
            {
                return _peakHours;
            }
            set
            {
                _peakHours = value;
                this.PeakHoursIds = new HashSet<int>();
                foreach (var hour in value)
                    this.PeakHoursIds.Add(hour.Id);
            }
        }

        public HashSet<int> PeakHoursIds { get; set; }

        public List<StrategyLevel> StrategyLevels { get; set; }

        public List<StrategyFilter> StrategyFilters { get; set; }
    }



    public class StrategyFilter
    {
        public int FilterId { get; set; }

        public string Description { get; set; }

        public Decimal Threshold { get; set; }

        public int? PeriodId { get; set; }
        //public bool IsSelected { get; set; }

    }



    public class StrategyLevel
    {
        public int SuspicionLevelId { get; set; }

        public List<StrategyLevelCriteria> StrategyLevelCriterias { get; set; }
    }

    public class StrategyLevelCriteria
    {
        public int FilterId { get; set; }

        public decimal Percentage { get; set; }
    }


    public class Hour
    {
        public int Id { get; set; }

        public string Name { get; set; }
    }

}