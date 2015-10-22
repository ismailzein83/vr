using System;
using System.Collections.Generic;
using System.Linq;
using Vanrise.Fzero.CDRImport.Entities;
using Vanrise.Fzero.FraudAnalysis.Entities;
using Vanrise.Fzero.Business;
using Vanrise.Fzero.Entities;
using Vanrise.Fzero.FraudAnalysis.Data;


namespace Vanrise.Fzero.FraudAnalysis.Business
{
    public class FilterManager
    {




        private static Dictionary<int, FilterDefinition> GetCachedFilters()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetFidddddlters",
               () =>
               {
                   IFilterDataManager dataManager = FraudDataManagerFactory.GetDataManager<IFilterDataManager>();
                   IEnumerable<FilterDefinition> filters = dataManager.GetFilters();
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



        public Decimal GetCriteriaValue(FilterDefinition criteria, NumberProfile numberProfile)
        {
            return s_criteriaDefinitions[criteria.FilterId].Expression(numberProfile);
        }

        static Dictionary<int, FilterDefinition> s_criteriaDefinitions = BuildAndGetCriteriaDefinitions();


        static Dictionary<int, FilterDefinition> BuildAndGetCriteriaDefinitions()
        {

            Dictionary<int, FilterDefinition> dictionary = new Dictionary<int, FilterDefinition>();

            foreach (var i in GetCachedFilters())
            {

                switch (i.Key)
                {
                    case 1: i.Value.Expression = CalculateRatioIncomingCallsvsOutgoingCalls; break;
                    case 2: i.Value.Expression = CalculateCountofDistinctDestinations; break;
                    case 3: i.Value.Expression = CalculateCountOutgoingCalls; break;
                    case 5: i.Value.Expression = CalculateTotalOriginatedVolume; break;
                    case 7: i.Value.Expression = CalculateRatioAverageIncomingDurationvsAverageOutgoingDuration; break;
                    case 9: i.Value.Expression = CalculateCountofDailyActiveHours; break;
                    case 10: i.Value.Expression = CalculateDistinctDestinationofNightCalls; break;
                    case 12: i.Value.Expression = CalculateRatioofDistinctDestinationvsTotalNumberofCalls; break;
                    case 14: i.Value.Expression = CalculateCountofOutgoingDuringPeakHours; break;
                    case 16: i.Value.Expression = CalculateConsecutiveCalls; break;
                    case 17: i.Value.Expression = CalculateFailConsecutiveCalls; break;
                    case 18: i.Value.Expression = CalculateRatioCountIncominglowdurationCallsVsCountIncomingCalls; break;
                    case 4: i.Value.Expression = CalculateCountofTotalBTSPerMSISDN; break;
                    case 6: i.Value.Expression = CalculateCountofTotalIMEIPerMSISDN; break;
                    case 8: i.Value.Expression = CalculateRatioOffNetOriginatedCallsvsOnNetOriginatedCalls; break;
                    case 11: i.Value.Expression = CalculateVoiceOnlyServiceUsage; break;
                    case 13: i.Value.Expression = CalculateRatioInternationalOriginatedvsOutgoingCalls; break;
                    case 15: i.Value.Expression = CalculateDataUsage; break;
                    case 19: i.Value.Expression = CalculateDifferentDestinationZones; break;
                    case 20: i.Value.Expression = CalculateDifferentSourceZones; break;

                }

                 dictionary.Add(i.Key, i.Value);
            }

            return dictionary.Where(x => x.Value.OperatorTypeAllowed == GlobalConstants._DefaultOperatorType || x.Value.OperatorTypeAllowed == OperatorType.Both).OrderBy(x => x.Value.FilterId).ToDictionary(i => i.Key, i => i.Value);
        }


        

        public Dictionary<int, FilterDefinition> GetCriteriaDefinitions()
        {
            return s_criteriaDefinitions;
        }

        public List<FilterDefinitionInfo> GetCriteriaNames()
        {
            List<FilterDefinitionInfo> names = new List<FilterDefinitionInfo>();
            foreach (var i in s_criteriaDefinitions)
            {
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
                names.Add(new FilterDefinitionInfo() { FilterId = i.Value.FilterId, OperatorTypeAllowed = i.Value.OperatorTypeAllowed, Description = i.Value.Description, Abbreviation = i.Value.Abbreviation, Label = i.Value.Label, MaxValue = i.Value.MaxValue, MinValue = i.Value.MinValue, DecimalPrecision = i.Value.DecimalPrecision, ExcludeHourly = i.Value.ExcludeHourly, ToolTip = i.Value.ToolTip, UpSign = upSign, DownSign = downSign });
            }
            return names;
        }

        // Funcs

        static decimal CalculateRatioCountIncominglowdurationCallsVsCountIncomingCalls(NumberProfile numberProfile)
        {
            Decimal countInCalls = numberProfile.AggregateValues[Constants._CountInCalls];
            if (countInCalls != 0)
                return (numberProfile.AggregateValues[Constants._CountInLowDurationCalls] / countInCalls);
            else
                return 0;
        }


        static decimal CalculateRatioIncomingCallsvsOutgoingCalls(NumberProfile numberProfile)
        {
            Decimal countOutCalls = numberProfile.AggregateValues[Constants._CountOutCalls];
            if (countOutCalls != 0)
                return (numberProfile.AggregateValues[Constants._CountInCalls] / countOutCalls);
            else
                return 0;
        }

        static decimal CalculateCountofDistinctDestinations(NumberProfile numberProfile)
        {
            return numberProfile.AggregateValues[Constants._DiffOutputNumbers];
        }

        static decimal CalculateCountOutgoingCalls(NumberProfile numberProfile)
        {
            return numberProfile.AggregateValues[Constants._CountOutCalls];
        }

        static decimal CalculateCountofTotalBTSPerMSISDN(NumberProfile numberProfile)
        {
            return numberProfile.AggregateValues[Constants._TotalBTS];
        }

        static decimal CalculateTotalOriginatedVolume(NumberProfile numberProfile)
        {
            return numberProfile.AggregateValues[Constants._TotalOutVolume];
        }

        static decimal CalculateCountofTotalIMEIPerMSISDN(NumberProfile numberProfile)
        {
            return numberProfile.AggregateValues["TotalIMEI"];
        }

        static decimal CalculateRatioAverageIncomingDurationvsAverageOutgoingDuration(NumberProfile numberProfile)
        {
            decimal callOutDurAvg = numberProfile.AggregateValues[Constants._CallOutDurAvg];
            if (callOutDurAvg != 0)
                return (numberProfile.AggregateValues[Constants._CallInDurAvg] / callOutDurAvg);
            else
                return 0;
        }

        static decimal CalculateRatioOffNetOriginatedCallsvsOnNetOriginatedCalls(NumberProfile numberProfile)
        {
            decimal countOutOnNets = numberProfile.AggregateValues[Constants._CountOutOnNets];
            if (countOutOnNets != 0)
                return (numberProfile.AggregateValues[Constants._CountOutOffNets] / countOutOnNets);
            else
                return 0;
        }

        static decimal CalculateCountofDailyActiveHours(NumberProfile numberProfile)
        {

            return numberProfile.AggregateValues[Constants._CountActiveHours];

        }

        static decimal CalculateDistinctDestinationofNightCalls(NumberProfile numberProfile)
        {

            return numberProfile.AggregateValues[Constants._DiffOutputNumbersNightCalls];

        }

        static decimal CalculateVoiceOnlyServiceUsage(NumberProfile numberProfile)
        {
            return numberProfile.AggregateValues[Constants._CountOutSMSs];
        }

        static decimal CalculateRatioofDistinctDestinationvsTotalNumberofCalls(NumberProfile numberProfile)
        {
            decimal countOutCalls = numberProfile.AggregateValues[Constants._CountOutCalls];
            if (countOutCalls != 0)
                return (numberProfile.AggregateValues[Constants._DiffOutputNumbers] / countOutCalls);
            else
                return 0;
        }

        static decimal CalculateRatioInternationalOriginatedvsOutgoingCalls(NumberProfile numberProfile)
        {
            decimal countOutCalls = numberProfile.AggregateValues[Constants._CountOutCalls];
            if (countOutCalls != 0)
                return (numberProfile.AggregateValues[Constants._CountOutInters] / countOutCalls);
            else
                return 0;
        }

        static decimal CalculateCountofOutgoingDuringPeakHours(NumberProfile numberProfile)
        {

            return numberProfile.AggregateValues[Constants._CountOutCallsPeakHours];

        }

        static decimal CalculateDataUsage(NumberProfile numberProfile)
        {
            return numberProfile.AggregateValues[Constants._TotalDataVolume];
        }

        static decimal CalculateDifferentDestinationZones(NumberProfile numberProfile)
        {
            return numberProfile.AggregateValues[Constants._DiffDestZones];
        }

        static decimal CalculateDifferentSourceZones(NumberProfile numberProfile)
        {
            return numberProfile.AggregateValues[Constants._DiffSourcesZones];
        }

        static decimal CalculateConsecutiveCalls(NumberProfile numberProfile)
        {
            return numberProfile.AggregateValues[Constants._CountConsecutiveCalls];
        }

        static decimal CalculateFailConsecutiveCalls(NumberProfile numberProfile)
        {
            return numberProfile.AggregateValues[Constants._CountFailConsecutiveCalls];
        }

    }
}
