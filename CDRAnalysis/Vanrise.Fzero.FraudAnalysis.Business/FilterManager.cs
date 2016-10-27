using System;
using System.Collections.Generic;
using System.Linq;
using Vanrise.Fzero.Business;
using Vanrise.Fzero.Entities;
using Vanrise.Fzero.FraudAnalysis.Data;
using Vanrise.Fzero.FraudAnalysis.Entities;


namespace Vanrise.Fzero.FraudAnalysis.Business
{
    public class FilterManager
    {
        private static Dictionary<int, Filter> GetCachedFilters()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetFilters",
               () =>
               {
                   IFilterDataManager dataManager = FraudDataManagerFactory.GetDataManager<IFilterDataManager>();
                   IEnumerable<Filter> filters = dataManager.GetFilters();
                   return filters.ToDictionary(kvp => kvp.FilterId, kvp => kvp);
               });
        }
        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IFilterDataManager _dataManager = FraudDataManagerFactory.GetDataManager<IFilterDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.AreFiltersUpdated(ref _updateHandle);
            }
        }
        public Decimal? GetCriteriaValue(Filter criteria, NumberProfile numberProfile)
        {
            return s_criteriaDefinitions[criteria.FilterId].Expression(numberProfile);
        }
        static Dictionary<int, Filter> s_criteriaDefinitions = BuildAndGetCriteriaDefinitions();
        static Dictionary<int, Filter> BuildAndGetCriteriaDefinitions()
        {




            Dictionary<int, Filter> dictionary = new Dictionary<int, Filter>();

            foreach (var i in GetCachedFilters())
            {

                switch (i.Key)
                {
                    case 1: i.Value.Expression = CalculateRatioIncomingCallsvsOutgoingCalls; break;
                    case 2: i.Value.Expression = CalculateCountofDistinctDestinations; break;
                    case 3: i.Value.Expression = CalculateCountOutgoingCalls; break;
                    case 4: i.Value.Expression = CalculateCountofTotalBTSPerMSISDN; break;
                    case 5: i.Value.Expression = CalculateTotalOriginatedVolume; break;
                    case 6: i.Value.Expression = CalculateCountofTotalIMEIPerMSISDN; break;
                    case 7: i.Value.Expression = CalculateRatioAverageIncomingDurationvsAverageOutgoingDuration; break;
                    case 8: i.Value.Expression = CalculateRatioOffNetOriginatedCallsvsOnNetOriginatedCalls; break;
                    case 9: i.Value.Expression = CalculateCountofDailyActiveHours; break;
                    case 10: i.Value.Expression = CalculateDistinctDestinationofNightCalls; break;
                    case 11: i.Value.Expression = CalculateVoiceOnlyServiceUsage; break;
                    case 12: i.Value.Expression = CalculateRatioofDistinctDestinationvsTotalNumberofCalls; break;
                    case 13: i.Value.Expression = CalculateRatioInternationalOriginatedvsOutgoingCalls; break;
                    case 14: i.Value.Expression = CalculateCountofOutgoingDuringPeakHours; break;
                    case 15: i.Value.Expression = CalculateDataUsage; break;
                    case 16: i.Value.Expression = CalculateConsecutiveCalls; break;
                    case 17: i.Value.Expression = CalculateFailConsecutiveCalls; break;
                    case 18: i.Value.Expression = CalculateRatioCountIncominglowdurationCallsVsCountIncomingCalls; break;
                    case 19: i.Value.Expression = CalculateDifferentDestinationZones; break;
                    case 20: i.Value.Expression = CalculateDifferentSourceZones; break;
                }

                dictionary.Add(i.Key, i.Value);
            }

            return dictionary.Where(x => x.Value.OperatorTypeAllowed == GlobalConstants._DefaultOperatorType || x.Value.OperatorTypeAllowed == OperatorType.Both).OrderBy(x => x.Value.FilterId).ToDictionary(i => i.Key, i => i.Value);
        }
        public Dictionary<int, Filter> GetCriteriaDefinitions()
        {
            return s_criteriaDefinitions;
        }
        public List<FilterInfo> GetCriteriaNames(FilterForFilter filter)
        {
            List<FilterInfo> names = new List<FilterInfo>();
            foreach (var i in s_criteriaDefinitions)
            {
                if(filter != null)
                {
                    if (i.Value.ExcludeHourly == filter.ExcludeHourly && filter.ExcludeHourly == true)
                  {
                      continue;
                  }
                }
                string upSign = "";
                string downSign = "";

                if (i.Value.CompareOperator == CriteriaCompareOperator.GreaterThanorEqual)
                {
                    upSign = Constants._Critical;
                    downSign = Constants._Safe;
                }
                else
                {
                    upSign = Constants._Safe;
                    downSign = Constants._Critical;
                }

                FilterInfo filterDef = new FilterInfo()
                {
                    FilterId = i.Value.FilterId,
                    OperatorTypeAllowed = i.Value.OperatorTypeAllowed,
                    Description = i.Value.Description,
                    Abbreviation = i.Value.Abbreviation,
                    Label = i.Value.Label,
                    MaxValue = i.Value.MaxValue,
                    MinValue = i.Value.MinValue,
                    DecimalPrecision = i.Value.DecimalPrecision,
                    ExcludeHourly = i.Value.ExcludeHourly,
                    ToolTip = i.Value.ToolTip,
                    UpSign = upSign,
                    DownSign = downSign
                };


                filterDef.Parameters = new List<string>();


                switch (i.Value.FilterId)
                {
                    case 14:
                        filterDef.Parameters.Add("Peak Hours");
                        break;

                    case 16:
                        filterDef.Parameters.Add("Gap between Consecutive Calls in Seconds");
                        break;

                    case 17:
                        filterDef.Parameters.Add("Gap between Failed Consecutive Calls in Seconds");
                        break;

                    case 18:
                        filterDef.Parameters.Add("Maximum Low Duration Call (s)");
                        break;

                    case 9:
                        filterDef.Parameters.Add("Minimum Count of Calls per Hour");
                        break;
                }

                names.Add(filterDef);
            }
            return names;
        }

        // Funcs
        static decimal? CalculateRatioCountIncominglowdurationCallsVsCountIncomingCalls(NumberProfile numberProfile)
        {
            Decimal countInCalls = numberProfile.AggregateValues[Constants._CountInCalls];
            if (countInCalls != 0)
                return (numberProfile.AggregateValues[Constants._CountInLowDurationCalls] / countInCalls);
            else
                return null;
        }
        static decimal? CalculateRatioIncomingCallsvsOutgoingCalls(NumberProfile numberProfile)
        {
            Decimal countOutCalls = numberProfile.AggregateValues[Constants._CountOutCalls];
            if (countOutCalls != 0)
                return (numberProfile.AggregateValues[Constants._CountInCalls] / countOutCalls);
            else
                return null;
        }
        static decimal? CalculateCountofDistinctDestinations(NumberProfile numberProfile)
        {
            return numberProfile.AggregateValues[Constants._DiffOutputNumbers];
        }
        static decimal? CalculateCountOutgoingCalls(NumberProfile numberProfile)
        {
            return numberProfile.AggregateValues[Constants._CountOutCalls];
        }
        static decimal? CalculateCountofTotalBTSPerMSISDN(NumberProfile numberProfile)
        {
            return numberProfile.AggregateValues[Constants._TotalBTS];
        }
        static decimal? CalculateTotalOriginatedVolume(NumberProfile numberProfile)
        {
            return numberProfile.AggregateValues[Constants._TotalOutVolume];
        }
        static decimal? CalculateCountofTotalIMEIPerMSISDN(NumberProfile numberProfile)
        {
            return numberProfile.AggregateValues[Constants._TotalIMEI];
        }
        static decimal? CalculateRatioAverageIncomingDurationvsAverageOutgoingDuration(NumberProfile numberProfile)
        {
            decimal callOutDurAvg = numberProfile.AggregateValues[Constants._CallOutDurAvg];
            if (callOutDurAvg != 0)
                return (numberProfile.AggregateValues[Constants._CallInDurAvg] / callOutDurAvg);
            else
                return null;
        }
        static decimal? CalculateRatioOffNetOriginatedCallsvsOnNetOriginatedCalls(NumberProfile numberProfile)
        {
            decimal countOutOnNets = numberProfile.AggregateValues[Constants._CountOutOnNets];
            if (countOutOnNets != 0)
                return (numberProfile.AggregateValues[Constants._CountOutOffNets] / countOutOnNets);
            else
                return null;
        }
        static decimal? CalculateCountofDailyActiveHours(NumberProfile numberProfile)
        {

            return numberProfile.AggregateValues[Constants._CountActiveHours];

        }
        static decimal? CalculateDistinctDestinationofNightCalls(NumberProfile numberProfile)
        {

            return numberProfile.AggregateValues[Constants._DiffOutputNumbersNightCalls];

        }
        static decimal? CalculateVoiceOnlyServiceUsage(NumberProfile numberProfile)
        {
            return numberProfile.AggregateValues[Constants._CountOutSMSs];
        }
        static decimal? CalculateRatioofDistinctDestinationvsTotalNumberofCalls(NumberProfile numberProfile)
        {
            decimal countOutCalls = numberProfile.AggregateValues[Constants._CountOutCalls];
            if (countOutCalls != 0)
                return (numberProfile.AggregateValues[Constants._DiffOutputNumbers] / countOutCalls);
            else
                return null;
        }
        static decimal? CalculateRatioInternationalOriginatedvsOutgoingCalls(NumberProfile numberProfile)
        {
            decimal countOutCalls = numberProfile.AggregateValues[Constants._CountOutCalls];
            if (countOutCalls != 0)
                return (numberProfile.AggregateValues[Constants._CountOutInters] / countOutCalls);
            else
                return null;
        }
        static decimal? CalculateCountofOutgoingDuringPeakHours(NumberProfile numberProfile)
        {
            return numberProfile.AggregateValues[Constants._CountOutCallsPeakHours];
        }
        static decimal? CalculateDataUsage(NumberProfile numberProfile)
        {
            return numberProfile.AggregateValues[Constants._TotalDataVolume];
        }
        static decimal? CalculateDifferentDestinationZones(NumberProfile numberProfile)
        {
            return numberProfile.AggregateValues[Constants._DiffDestZones];
        }
        static decimal? CalculateDifferentSourceZones(NumberProfile numberProfile)
        {
            return numberProfile.AggregateValues[Constants._DiffSourcesZones];
        }
        static decimal? CalculateConsecutiveCalls(NumberProfile numberProfile)
        {
            return numberProfile.AggregateValues[Constants._CountConsecutiveCalls];
        }
        static decimal? CalculateFailConsecutiveCalls(NumberProfile numberProfile)
        {
            return numberProfile.AggregateValues[Constants._CountFailConsecutiveCalls];
        }

    }
}
