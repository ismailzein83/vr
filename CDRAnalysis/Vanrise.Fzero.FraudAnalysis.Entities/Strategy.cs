using System;
using System.Collections.Generic;

namespace Vanrise.Fzero.FraudAnalysis.Entities
{
    public class Strategy : INumberProfileParameters
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int UserId { get; set; }
        public DateTime LastUpdatedOn { get; set; }
        public StrategySettings Settings { get; set; }

        #region INumberProfileParameters

        int INumberProfileParameters.GapBetweenConsecutiveCalls
        {
            get
            {
                return this.Settings.Parameters.GapBetweenConsecutiveCalls;
            }
            set
            {
                value = this.Settings.Parameters.GapBetweenConsecutiveCalls;
            }
        }
        int INumberProfileParameters.GapBetweenFailedConsecutiveCalls
        {
            get
            {
                return this.Settings.Parameters.GapBetweenFailedConsecutiveCalls;
            }
            set
            {
                value = this.Settings.Parameters.GapBetweenFailedConsecutiveCalls;
            }
        }
        int INumberProfileParameters.MaxLowDurationCall
        {
            get
            {
                return this.Settings.Parameters.MaxLowDurationCall;
            }
            set
            {
                value = this.Settings.Parameters.MaxLowDurationCall;
            }
        }
        int INumberProfileParameters.MinimumCountofCallsinActiveHour
        {
            get
            {
                return this.Settings.Parameters.MinimumCountofCallsinActiveHour;
            }
            set
            {
                value = this.Settings.Parameters.MinimumCountofCallsinActiveHour;
            }
        }
        HashSet<int> INumberProfileParameters.PeakHoursIds
        {
            get
            {
                return this.Settings.Parameters.PeakHoursIds;
            }
            set
            {
                value = this.Settings.Parameters.PeakHoursIds;
            }
        }

        #endregion
    }

    public class StrategySettings
    {
        public int PeriodId { get; set; }
        public bool IsDefault { get; set; }
        public bool IsEnabled { get; set; }
        public StrategyParameters Parameters { get; set; }
        public StrategySettingsCriteria StrategySettingsCriteria { get; set; }
    }
    public class StrategyParameters
    {
        public int GapBetweenConsecutiveCalls { get; set; }
        public int GapBetweenFailedConsecutiveCalls { get; set; }
        public int MaxLowDurationCall { get; set; }
        public int MinimumCountofCallsinActiveHour { get; set; }
        List<Hour> _peakHours;
        public List<Hour> PeakHours
        {
            get { return _peakHours; }

            set
            {
                _peakHours = value;
                this.PeakHoursIds = new HashSet<int>();
                foreach (var hour in value)
                    this.PeakHoursIds.Add(hour.Id);
            }
        }
        public HashSet<int> PeakHoursIds { get; set; }
    }
    public class Hour
    {
        public int Id { get; set; }

        public string Name { get; set; }
    }
   
    public abstract class StrategySettingsCriteria
    {
        public abstract Guid ConfigId { get;}
        public abstract void PrepareForExecution(IPrepareStrategySettingsCriteriaContext context);
        public abstract bool IsNumberSuspicious(IStrategySettingsCriteriaContext context);
    }
    public interface IPrepareStrategySettingsCriteriaContext
    {
        Object PreparedData { set; }
    }
    public interface IStrategySettingsCriteriaContext
    {
        Object PreparedData { get; }
        NumberProfile NumberProfile { get; }
        Decimal? GetCriteriaValue(int filterId);
        Filter GetFilter(int filterId);
        StrategyExecutionItem StrategyExecutionItem { set; }
    }

}