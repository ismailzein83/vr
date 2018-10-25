using System;
using System.Collections.Generic;
using System.Linq;
using Vanrise.Common;
using Vanrise.Fzero.Business;
using Vanrise.Fzero.CDRImport.Entities;
using Vanrise.Fzero.Entities;
using Vanrise.Fzero.FraudAnalysis.Aggregates;
using Vanrise.Fzero.FraudAnalysis.Data;
using Vanrise.Fzero.FraudAnalysis.Entities;

namespace Vanrise.Fzero.FraudAnalysis.Business
{
    public class AggregateManager
    {
        static Dictionary<int, string> s_aggregateDefinitionKeyNamesByIds;
        static AggregateManager()
        {
            s_aggregateDefinitionKeyNamesByIds = new Dictionary<int, string>();

            s_aggregateDefinitionKeyNamesByIds.Add(1, Constants._CountOutCalls);
            s_aggregateDefinitionKeyNamesByIds.Add(2, Constants._CountInCalls);
            s_aggregateDefinitionKeyNamesByIds.Add(3, Constants._TotalDataVolume);
            s_aggregateDefinitionKeyNamesByIds.Add(4, Constants._CountOutFails);
            s_aggregateDefinitionKeyNamesByIds.Add(5, Constants._CountInFails);
            s_aggregateDefinitionKeyNamesByIds.Add(6, Constants._CountOutSMSs);
            s_aggregateDefinitionKeyNamesByIds.Add(7, Constants._CountOutOffNets);
            s_aggregateDefinitionKeyNamesByIds.Add(8, Constants._CountOutOnNets);
            s_aggregateDefinitionKeyNamesByIds.Add(9, Constants._CountOutInters);
            s_aggregateDefinitionKeyNamesByIds.Add(10, Constants._CountInInters);
            s_aggregateDefinitionKeyNamesByIds.Add(11, Constants._CallOutDurAvg);
            s_aggregateDefinitionKeyNamesByIds.Add(12, Constants._CallInDurAvg);
            s_aggregateDefinitionKeyNamesByIds.Add(13, Constants._TotalOutVolume);
            s_aggregateDefinitionKeyNamesByIds.Add(14, Constants._TotalInVolume);
            s_aggregateDefinitionKeyNamesByIds.Add(15, Constants._TotalIMEI);
            s_aggregateDefinitionKeyNamesByIds.Add(16, Constants._TotalBTS);
            s_aggregateDefinitionKeyNamesByIds.Add(17, Constants._DiffOutputNumbers);
            s_aggregateDefinitionKeyNamesByIds.Add(18, Constants._DiffInputNumbers);
            s_aggregateDefinitionKeyNamesByIds.Add(19, Constants._CountInOffNets);
            s_aggregateDefinitionKeyNamesByIds.Add(20, Constants._CountInOnNets);
            s_aggregateDefinitionKeyNamesByIds.Add(21, Constants._DiffOutputNumbersNightCalls);
            s_aggregateDefinitionKeyNamesByIds.Add(22, Constants._CountOutCallsPeakHours);
            s_aggregateDefinitionKeyNamesByIds.Add(23, Constants._CountConsecutiveCalls);
            s_aggregateDefinitionKeyNamesByIds.Add(24, Constants._CountActiveHours);
            s_aggregateDefinitionKeyNamesByIds.Add(25, Constants._CountFailConsecutiveCalls);
            s_aggregateDefinitionKeyNamesByIds.Add(26, Constants._CountInLowDurationCalls);
            s_aggregateDefinitionKeyNamesByIds.Add(27, Constants._DiffDestZones);
            s_aggregateDefinitionKeyNamesByIds.Add(28, Constants._DiffSourcesZones);
        }

        private static Dictionary<int, AggregateDefinitionInfo> GetCachedAggregates()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetCachedAggregates",
               () =>
               {
                   IAggregateDataManager dataManager = FraudDataManagerFactory.GetDataManager<IAggregateDataManager>();
                   IEnumerable<AggregateDefinitionInfo> aggregates = dataManager.GetAggregates();
                   Dictionary<int, AggregateDefinitionInfo> dicAggregates = new Dictionary<int, AggregateDefinitionInfo>();
                   foreach (var aggregateDefinitionInfo in aggregates)
                   {
                       if (!(aggregateDefinitionInfo.OperatorTypeAllowed == GlobalConstants._DefaultOperatorType || aggregateDefinitionInfo.OperatorTypeAllowed == OperatorType.Both))
                           continue;
                       string keyName;
                       if (s_aggregateDefinitionKeyNamesByIds.TryGetValue(aggregateDefinitionInfo.Id, out keyName))
                           aggregateDefinitionInfo.KeyName = keyName;
                       else
                           throw new Exception(String.Format("Aggregate Id '{0}' not found in s_aggregateDefinitionKeyNamesByIds", aggregateDefinitionInfo.Id));
                       dicAggregates.Add(aggregateDefinitionInfo.Id, aggregateDefinitionInfo);
                   }
                   return dicAggregates;
               });
        }

        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IAggregateDataManager _dataManager = FraudDataManagerFactory.GetDataManager<IAggregateDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.AreAggregatesUpdated(ref _updateHandle);
            }
        }


        List<INumberProfileParameters> _parameters;
        HashSet<int> _nightCallHours = new HashSet<int>() { 0, 1, 2, 3, 4, 5 };



        public AggregateManager(List<INumberProfileParameters> parameters)
        {
            _parameters = parameters;
        }

        public AggregateManager()
        {

        }

        public List<AggregateDefinition> GetAggregateDefinitions(Dictionary<int, NetType> callClassNetTypes)
        {
            Func<CDR, NetType, bool> IsMatchNetType = (cdr, compareToNetType) =>
            {
                NetType netType;

                if (cdr.CallClassId.HasValue)
                {
                    if (!callClassNetTypes.TryGetValue(cdr.CallClassId.Value, out netType))
                        return false;
                    else
                        return netType == compareToNetType;
                }

                else
                    return false;


            };

            List<AggregateDefinition> aggregateDefinitions = new List<AggregateDefinition>();
            HashSet<int> aggregateIdsToUse = Vanrise.Common.ExtensionMethods.ToHashSet(GetCachedAggregates().Values.Select(itm => itm.Id));

            CheckAndAddAggregateDefinition(aggregateDefinitions, aggregateIdsToUse, new AggregateDefinition()
                {
                    Id = 1,
                    Aggregation = new CountAggregate((cdr) =>
                    {
                        return (cdr.CallType == CallType.OutgoingVoiceCall);
                    })
                     
                });

            CheckAndAddAggregateDefinition(aggregateDefinitions, aggregateIdsToUse, new AggregateDefinition()
            {
                Id = 2,
                Aggregation = new CountAggregate((cdr) =>
                {
                    return (cdr.CallType == CallType.IncomingVoiceCall);
                })
            });


            CheckAndAddAggregateDefinition(aggregateDefinitions, aggregateIdsToUse, new AggregateDefinition()
            {
                Id = 3,
                Aggregation = new SumAggregate((cdr) =>
                {
                    return (cdr.UpVolume.HasValue ? cdr.UpVolume.Value : 0) + (cdr.DownVolume.HasValue ? cdr.DownVolume.Value : 0);
                })
            });


            CheckAndAddAggregateDefinition(aggregateDefinitions, aggregateIdsToUse, new AggregateDefinition()
            {
                Id = 4,
                Aggregation = new CountAggregate((cdr) =>
                {
                    return (cdr.CallType == CallType.OutgoingVoiceCall) && (cdr.DurationInSeconds == 0);
                })
            });


            CheckAndAddAggregateDefinition(aggregateDefinitions, aggregateIdsToUse, new AggregateDefinition()
            {
                Id = 5,
                Aggregation = new CountAggregate((cdr) =>
                {
                    return (cdr.CallType == CallType.IncomingVoiceCall) && (cdr.DurationInSeconds == 0);
                })
            });



            CheckAndAddAggregateDefinition(aggregateDefinitions, aggregateIdsToUse, new AggregateDefinition()
            {
                Id = 6,
                Aggregation = new CountAggregate((cdr) =>
                {
                    return (cdr.CallType == CallType.OutgoingSms);
                })
            });



            CheckAndAddAggregateDefinition(aggregateDefinitions, aggregateIdsToUse, new AggregateDefinition()
            {
                Id = 7,
                Aggregation = new CountAggregate((cdr) =>
                {
                    return ((cdr.CallType == CallType.OutgoingVoiceCall) && (IsMatchNetType(cdr, NetType.Others)));
                })
            });

            CheckAndAddAggregateDefinition(aggregateDefinitions, aggregateIdsToUse, new AggregateDefinition()
            {
                Id = 8,
                Aggregation = new CountAggregate((cdr) =>
                {
                    return ((cdr.CallType == CallType.OutgoingVoiceCall) && (IsMatchNetType(cdr, NetType.Local)));
                })
            });


            CheckAndAddAggregateDefinition(aggregateDefinitions, aggregateIdsToUse, new AggregateDefinition()
            {
                Id = 9,
                Aggregation = new CountAggregate((cdr) =>
                {
                    return ((cdr.CallType == CallType.OutgoingVoiceCall) && (IsMatchNetType(cdr, NetType.International)));
                })
            });


            CheckAndAddAggregateDefinition(aggregateDefinitions, aggregateIdsToUse, new AggregateDefinition()
            {
                Id = 10,
                Aggregation = new CountAggregate((cdr) =>
                {
                    return ((cdr.CallType == CallType.IncomingVoiceCall) && (IsMatchNetType(cdr, NetType.International)));
                })
            });



            CheckAndAddAggregateDefinition(aggregateDefinitions, aggregateIdsToUse, new AggregateDefinition()
            {
                Id = 11,
                Aggregation = new AverageAggregate(
                    (cdr) =>
                    {
                        return (cdr.CallType == CallType.OutgoingVoiceCall && cdr.DurationInSeconds > 0);
                    }
                     ,

                   (cdr) =>
                   {
                       return cdr.DurationInSeconds / 60;
                   }
            )
            });

            CheckAndAddAggregateDefinition(aggregateDefinitions, aggregateIdsToUse, new AggregateDefinition()
            {
                Id = 12,
                Aggregation = new AverageAggregate(
                   (cdr) =>
                   {
                       return (cdr.CallType == CallType.IncomingVoiceCall && cdr.DurationInSeconds > 0);
                   }
                    ,
               (cdr) =>
               {
                   return cdr.DurationInSeconds / 60;
               }
            )
            });


            CheckAndAddAggregateDefinition(aggregateDefinitions, aggregateIdsToUse, new AggregateDefinition()
            {
                Id = 13,
                Aggregation = new SumAggregate(
                (cdr) =>
                {
                    return (cdr.CallType == CallType.OutgoingVoiceCall) ? cdr.DurationInSeconds / 60 : 0;
                }
            )
            });





            CheckAndAddAggregateDefinition(aggregateDefinitions, aggregateIdsToUse, new AggregateDefinition()
             {
                 Id = 14,
                 Aggregation = new SumAggregate(
                 (cdr) =>
                 {
                     return (cdr.CallType == CallType.IncomingVoiceCall) ? cdr.DurationInSeconds / 60 : 0;
                 }
             )
             });

            CheckAndAddAggregateDefinition(aggregateDefinitions, aggregateIdsToUse, new AggregateDefinition()
             {
                 Id = 15,
                 Aggregation = new DistinctCountAggregate(
                    (cdr) =>
                    {
                        return cdr.IMEI;
                    },

                    (cdr) =>
                    {
                        return (cdr.CallType == CallType.IncomingVoiceCall || cdr.CallType == CallType.OutgoingVoiceCall || cdr.CallType == CallType.IncomingSms || cdr.CallType == CallType.OutgoingSms);
                    }
                )
             });


            CheckAndAddAggregateDefinition(aggregateDefinitions, aggregateIdsToUse, new AggregateDefinition()
             {
                 Id = 16,
                 Aggregation = new DistinctCountAggregate(
                       (cdr) =>
                       {
                           return cdr.BTS;
                       },

                       (cdr) =>
                       {
                           return (!string.IsNullOrEmpty(cdr.BTS) && (cdr.CallType == CallType.IncomingVoiceCall || cdr.CallType == CallType.OutgoingVoiceCall || cdr.CallType == CallType.IncomingSms || cdr.CallType == CallType.OutgoingSms));
                       }
                   )
             });


            CheckAndAddAggregateDefinition(aggregateDefinitions, aggregateIdsToUse, new AggregateDefinition()
             {
                 Id = 17,
                 Aggregation = new DistinctCountAggregate(
                      (cdr) =>
                      {
                          return cdr.Destination;
                      },

                      (cdr) =>
                      {
                          return (cdr.CallType == CallType.OutgoingVoiceCall);
                      }
                  )
             });


            CheckAndAddAggregateDefinition(aggregateDefinitions, aggregateIdsToUse, new AggregateDefinition()
             {
                 Id = 18,
                 Aggregation = new DistinctCountAggregate(
                      (cdr) =>
                      {
                          return cdr.Destination;
                      },

                      (cdr) =>
                      {
                          return (cdr.CallType == CallType.IncomingVoiceCall);
                      }
                  )
             });


            CheckAndAddAggregateDefinition(aggregateDefinitions, aggregateIdsToUse, new AggregateDefinition()
             {
                 Id = 19,
                 Aggregation = new CountAggregate((cdr) =>
                 {
                     return ((cdr.CallType == CallType.IncomingVoiceCall) && (IsMatchNetType(cdr, NetType.Others)));
                 })
             });


            CheckAndAddAggregateDefinition(aggregateDefinitions, aggregateIdsToUse, new AggregateDefinition()
             {
                 Id = 20,
                 Aggregation = new CountAggregate((cdr) =>
                 {
                     return ((cdr.CallType == CallType.IncomingVoiceCall) && (IsMatchNetType(cdr, NetType.Local)));
                 })
             });


            CheckAndAddAggregateDefinition(aggregateDefinitions, aggregateIdsToUse, new AggregateDefinition()
             {
                 Id = 21,
                 Aggregation = new DistinctCountAggregate(
                      (cdr) =>
                      {
                          return cdr.Destination;
                      },

                      (cdr) =>
                      {
                          return (cdr.CallType == CallType.OutgoingVoiceCall && _nightCallHours.Contains(cdr.ConnectDateTime.Hour));
                      }
                  )
             });


            CheckAndAddAggregateDefinition(aggregateDefinitions, aggregateIdsToUse, new AggregateDefinition()
             {
                 Id = 22,
                 Aggregation = new CountAggregate((cdr, strategy) =>
                 {
                     return (cdr.CallType == CallType.OutgoingVoiceCall && strategy.PeakHoursIds.Contains(cdr.ConnectDateTime.Hour));
                 }, _parameters)
             });


            CheckAndAddAggregateDefinition(aggregateDefinitions, aggregateIdsToUse, new AggregateDefinition()
             {
                 Id = 23,
                 Aggregation = new ConsecutiveAggregate(
                       (cdr, strategy) =>
                       {
                           return (cdr.CallType == CallType.OutgoingVoiceCall && cdr.DurationInSeconds > 0);
                       }
                       , _parameters
                       ,(parameterSets) => parameterSets.GapBetweenConsecutiveCalls
                   )
             });




            CheckAndAddAggregateDefinition(aggregateDefinitions, aggregateIdsToUse, new AggregateDefinition()
             {
                 Id = 24,
                 Aggregation = new GroupCountAggregate(
                       (cdr, strategy) =>
                       {
                           return (cdr.CallType == CallType.OutgoingVoiceCall);
                       }
                       , _parameters
                   )
             });



            CheckAndAddAggregateDefinition(aggregateDefinitions, aggregateIdsToUse, new AggregateDefinition()
            {
                Id = 25,
                Aggregation = new ConsecutiveAggregate(
                      (cdr, strategy) =>
                      {
                          return (cdr.CallType == CallType.OutgoingVoiceCall && cdr.DurationInSeconds == 0);
                      }
                      , _parameters
                      , (parameterSets) => parameterSets.GapBetweenFailedConsecutiveCalls
                  )
            });


            CheckAndAddAggregateDefinition(aggregateDefinitions, aggregateIdsToUse, new AggregateDefinition()
            {
                Id = 26,
                Aggregation = new CountAggregate((cdr, strategy) =>
                {
                    return (cdr.CallType == CallType.IncomingVoiceCall && cdr.DurationInSeconds <= strategy.MaxLowDurationCall);
                }, _parameters)
            });




            CheckAndAddAggregateDefinition(aggregateDefinitions, aggregateIdsToUse, new AggregateDefinition()
            {
                Id = 27,
                Aggregation = new DistinctCountAggregate(
                      (cdr) =>
                      {
                          return cdr.DestinationAreaCode;
                      },

                      (cdr) =>
                      {
                          return cdr.CallType == CallType.OutgoingVoiceCall;
                      }
                  )
            });




            CheckAndAddAggregateDefinition(aggregateDefinitions, aggregateIdsToUse, new AggregateDefinition()
            {
                Id = 28,
                Aggregation = new DistinctCountAggregate(
                      (cdr) =>
                      {
                          return cdr.MSISDNAreaCode;
                      },

                      (cdr) =>
                      {
                          return cdr.CallType == CallType.IncomingVoiceCall;
                      }
                  )
            });

            return aggregateDefinitions;
        }

        public List<AggregateState> CreateAggregateStates(List<AggregateDefinition> aggregateDefinitions)
        {
            List<AggregateState> states = new List<AggregateState>();
            foreach(var aggDef in aggregateDefinitions)
            {
                states.Add(aggDef.Aggregation.CreateState());
            }
            return states;
        }

        private void CheckAndAddAggregateDefinition(List<AggregateDefinition> aggregateDefinitions, HashSet<int> aggregateIdsToUse, AggregateDefinition aggregateDefinition)
        {
            if (aggregateIdsToUse.Contains(aggregateDefinition.Id))
            {
                string keyName;
                if (s_aggregateDefinitionKeyNamesByIds.TryGetValue(aggregateDefinition.Id, out keyName))
                    aggregateDefinition.KeyName = keyName;
                else
                    throw new Exception(String.Format("Aggregate Id '{0}' not found in s_aggregateDefinitionKeyNamesByIds", aggregateDefinition.Id));
                aggregateDefinitions.Add(aggregateDefinition);
            }
        }

        public List<AggregateDefinitionInfo> GetAggregateDefinitionsInfo()
        {
            return GetCachedAggregates().Values.ToList();
        }

    }
}
