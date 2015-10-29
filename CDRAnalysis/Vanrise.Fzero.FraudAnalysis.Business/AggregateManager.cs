using System;
using System.Collections.Generic;
using System.Linq;
using Vanrise.Fzero.CDRImport.Entities;
using Vanrise.Fzero.FraudAnalysis.Aggregates;
using Vanrise.Fzero.FraudAnalysis.Entities;
using Vanrise.Fzero.Entities;
using Vanrise.Fzero.Business;
using Vanrise.Fzero.FraudAnalysis.Data;

namespace Vanrise.Fzero.FraudAnalysis.Business
{
    public class AggregateManager
    {

        private static Dictionary<int, AggregateDefinitionInfo> GetCachedAggregates()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetAggregates",
               () =>
               {
                   IAggregateDataManager dataManager = FraudDataManagerFactory.GetDataManager<IAggregateDataManager>();
                   IEnumerable<AggregateDefinitionInfo> aggregates = dataManager.GetAggregates();
                   return aggregates.ToDictionary(kvp => kvp.Id, kvp => kvp);
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


        IEnumerable<INumberProfileParameters> _parameters;
        HashSet<int> _nightCallHours = new HashSet<int>() { 0, 1, 2, 3, 4, 5 };



        public AggregateManager(IEnumerable<INumberProfileParameters> parameters)
        {
            _parameters = parameters;
        }

        public AggregateManager()
        {

        }

        public List<AggregateDefinition> GetAggregateDefinitions(List<CallClass> callClasses)
        {
            Dictionary<string, NetType> callClassNetTypes = new Dictionary<string, NetType>();
            foreach (CallClass callClass in callClasses)
            {
                callClassNetTypes.Add(callClass.Description, callClass.NetType);
            }

            Func<CDR, NetType, bool> IsMatchNetType = (cdr, compareToNetType) =>
            {
                NetType netType;
                if (String.IsNullOrEmpty(cdr.CallClass) || !callClassNetTypes.TryGetValue(cdr.CallClass, out netType))
                    return false;
                else
                    return netType == compareToNetType;

            };


            List<AggregateDefinition> AggregateDefinitions = new List<AggregateDefinition>();


            AggregateDefinitions.Add(new AggregateDefinition()
                {
                    Id = 1,
                    Aggregation = new CountAggregate((cdr) =>
                    {
                        return (cdr.CallType == CallType.OutgoingVoiceCall);
                    })
                });


            AggregateDefinitions.Add(new AggregateDefinition()
            {
                Id = 2,
                Aggregation = new CountAggregate((cdr) =>
                {
                    return (cdr.CallType == CallType.IncomingVoiceCall);
                })
            });


            AggregateDefinitions.Add(new AggregateDefinition()
            {
                Id = 3,
                Aggregation = new SumAggregate((cdr) =>
                {
                    return (cdr.UpVolume.HasValue ? cdr.UpVolume.Value : 0) + (cdr.DownVolume.HasValue ? cdr.DownVolume.Value : 0);
                })
            });


            AggregateDefinitions.Add(new AggregateDefinition()
            {
                Id = 4,
                Aggregation = new CountAggregate((cdr) =>
                {
                    return (cdr.CallType == CallType.OutgoingVoiceCall) && (cdr.DurationInSeconds == 0);
                })
            });


            AggregateDefinitions.Add(new AggregateDefinition()
            {
                Id = 5,
                Aggregation = new CountAggregate((cdr) =>
                {
                    return (cdr.CallType == CallType.IncomingVoiceCall) && (cdr.DurationInSeconds == 0);
                })
            });



            AggregateDefinitions.Add(new AggregateDefinition()
            {
                Id = 6,
                Aggregation = new CountAggregate((cdr) =>
                {
                    return (cdr.CallType == CallType.OutgoingSms);
                })
            });



            AggregateDefinitions.Add(new AggregateDefinition()
            {
                Id = 7,
                Aggregation = new CountAggregate((cdr) =>
                {
                    return ((cdr.CallType == CallType.OutgoingVoiceCall) && (IsMatchNetType(cdr, NetType.Others)));
                })
            });

            AggregateDefinitions.Add(new AggregateDefinition()
            {
                Id = 8,
                Aggregation = new CountAggregate((cdr) =>
                {
                    return ((cdr.CallType == CallType.OutgoingVoiceCall) && (IsMatchNetType(cdr, NetType.Local)));
                })
            });


            AggregateDefinitions.Add(new AggregateDefinition()
            {
                Id = 9,
                Aggregation = new CountAggregate((cdr) =>
                {
                    return ((cdr.CallType == CallType.OutgoingVoiceCall) && (IsMatchNetType(cdr, NetType.International)));
                })
            });


            AggregateDefinitions.Add(new AggregateDefinition()
            {
                Id = 10,
                Aggregation = new CountAggregate((cdr) =>
                {
                    return ((cdr.CallType == CallType.IncomingVoiceCall) && (IsMatchNetType(cdr, NetType.International)));
                })
            });



            AggregateDefinitions.Add(new AggregateDefinition()
            {
                Id = 11,
                Aggregation = new AverageAggregate(
                    (cdr) =>
                    {
                        return (cdr.CallType == CallType.OutgoingVoiceCall && cdr.DurationInSeconds.HasValue && cdr.DurationInSeconds > 0);
                    }
                     ,

                   (cdr) =>
                   {
                       return cdr.DurationInSeconds.Value / 60;
                   }
            )
            });

            AggregateDefinitions.Add(new AggregateDefinition()
            {
                Id = 12,
                Aggregation = new AverageAggregate(
                   (cdr) =>
                   {
                       return (cdr.CallType == CallType.IncomingVoiceCall && cdr.DurationInSeconds.HasValue && cdr.DurationInSeconds > 0);
                   }
                    ,
               (cdr) =>
               {
                   return cdr.DurationInSeconds.Value / 60;
               }
            )
            });


            AggregateDefinitions.Add(new AggregateDefinition()
            {
                Id = 13,
                Aggregation = new SumAggregate(
                (cdr) =>
                {
                    return (cdr.CallType == CallType.OutgoingVoiceCall && cdr.DurationInSeconds.HasValue) ? cdr.DurationInSeconds.Value / 60 : 0;
                }
            )
            });





            AggregateDefinitions.Add(new AggregateDefinition()
             {
                 Id = 14,
                 Aggregation = new SumAggregate(
                 (cdr) =>
                 {
                     return (cdr.CallType == CallType.IncomingVoiceCall && cdr.DurationInSeconds.HasValue) ? cdr.DurationInSeconds.Value : 0;
                 }
             )
             });

            AggregateDefinitions.Add(new AggregateDefinition()
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


            AggregateDefinitions.Add(new AggregateDefinition()
             {
                 Id = 16,
                 Aggregation = new DistinctCountAggregate(
                       (cdr) =>
                       {
                           return cdr.BTSId;
                       },

                       (cdr) =>
                       {
                           return (cdr.BTSId.HasValue && (cdr.CallType == CallType.IncomingVoiceCall || cdr.CallType == CallType.OutgoingVoiceCall || cdr.CallType == CallType.IncomingSms || cdr.CallType == CallType.OutgoingSms));
                       }
                   )
             });


            AggregateDefinitions.Add(new AggregateDefinition()
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


            AggregateDefinitions.Add(new AggregateDefinition()
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


            AggregateDefinitions.Add(new AggregateDefinition()
             {
                 Id = 19,
                 Aggregation = new CountAggregate((cdr) =>
                 {
                     return ((cdr.CallType == CallType.IncomingVoiceCall) && (IsMatchNetType(cdr, NetType.Others)));
                 })
             });


            AggregateDefinitions.Add(new AggregateDefinition()
             {
                 Id = 20,
                 Aggregation = new CountAggregate((cdr) =>
                 {
                     return ((cdr.CallType == CallType.IncomingVoiceCall) && (IsMatchNetType(cdr, NetType.Local)));
                 })
             });


            AggregateDefinitions.Add(new AggregateDefinition()
             {
                 Id = 21,
                 Aggregation = new DistinctCountAggregate(
                      (cdr) =>
                      {
                          return cdr.Destination;
                      },

                      (cdr) =>
                      {
                          return (cdr.CallType == CallType.OutgoingVoiceCall && cdr.ConnectDateTime.HasValue && _nightCallHours.Contains(cdr.ConnectDateTime.Value.Hour));
                      }
                  )
             });


            AggregateDefinitions.Add(new AggregateDefinition()
             {
                 Id = 22,
                 Aggregation = new CountAggregate((cdr, strategy) =>
                 {
                     return (cdr.CallType == CallType.OutgoingVoiceCall && cdr.ConnectDateTime.HasValue && strategy.PeakHoursIds.Contains(cdr.ConnectDateTime.Value.Hour));
                 }, _parameters)
             });


            AggregateDefinitions.Add(new AggregateDefinition()
             {
                 Id = 23,
                 Aggregation = new ConsecutiveAggregate(
                       (cdr, strategy) =>
                       {
                           return (cdr.CallType == CallType.OutgoingVoiceCall && cdr.DurationInSeconds > 0);
                       }
                       , _parameters
                   )
             });




            AggregateDefinitions.Add(new AggregateDefinition()
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



            AggregateDefinitions.Add(new AggregateDefinition()
            {
                Id = 25,
                Aggregation = new FailedConsecutiveAggregate(
                      (cdr, strategy) =>
                      {
                          return (cdr.CallType == CallType.OutgoingVoiceCall && cdr.DurationInSeconds == 0);
                      }
                      , _parameters
                  )
            });


            AggregateDefinitions.Add(new AggregateDefinition()
            {
                Id = 26,
                Aggregation = new CountAggregate((cdr, strategy) =>
                {
                    return (cdr.CallType == CallType.IncomingVoiceCall && cdr.DurationInSeconds <= strategy.MaxLowDurationCall);
                }, _parameters)
            });




            AggregateDefinitions.Add(new AggregateDefinition()
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




            AggregateDefinitions.Add(new AggregateDefinition()
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


            List<AggregateDefinitionInfo> aggregateDefinitionInfo = GetAggregateDefinitionsInfo();


            foreach (var j in aggregateDefinitionInfo)
            {
                foreach (var i in AggregateDefinitions)
                {
                    if (i.Id == j.Id)
                    {
                        i.KeyName = j.KeyName;
                        i.OperatorTypeAllowed = j.OperatorTypeAllowed;
                    }
                }
            }



            return AggregateDefinitions.Where(x => x.KeyName != null && (x.OperatorTypeAllowed == GlobalConstants._DefaultOperatorType || x.OperatorTypeAllowed == OperatorType.Both)).ToList();
        }

        public List<AggregateDefinitionInfo> GetAggregateDefinitionsInfo()
        {
            List<AggregateDefinitionInfo> AggregateDefinitionsInfo = new List<AggregateDefinitionInfo>();


            foreach (var i in GetCachedAggregates())
            {
                switch (i.Value.Id)
                {
                    case 1: i.Value.KeyName = Constants._CountOutCalls; break;
                    case 2: i.Value.KeyName = Constants._CountInCalls; break;
                    case 3: i.Value.KeyName = Constants._TotalDataVolume; break;
                    case 4: i.Value.KeyName = Constants._CountOutFails; break;
                    case 5: i.Value.KeyName = Constants._CountInFails; break;
                    case 6: i.Value.KeyName = Constants._CountOutSMSs; break;
                    case 7: i.Value.KeyName = Constants._CountOutOffNets; break;
                    case 8: i.Value.KeyName = Constants._CountOutOnNets; break;
                    case 9: i.Value.KeyName = Constants._CountOutInters; break;
                    case 10: i.Value.KeyName = Constants._CountInInters; break;
                    case 11: i.Value.KeyName = Constants._CallOutDurAvg; break;
                    case 12: i.Value.KeyName = Constants._CallInDurAvg; break;
                    case 13: i.Value.KeyName = Constants._TotalOutVolume; break;
                    case 14: i.Value.KeyName = Constants._TotalInVolume; break;
                    case 15: i.Value.KeyName = Constants._TotalIMEI; break;
                    case 16: i.Value.KeyName = Constants._TotalBTS; break;
                    case 17: i.Value.KeyName = Constants._DiffOutputNumbers; break;
                    case 18: i.Value.KeyName = Constants._DiffInputNumbers; break;
                    case 19: i.Value.KeyName = Constants._CountInOffNet; break;
                    case 20: i.Value.KeyName = Constants._CountInOnNets; break;
                    case 21: i.Value.KeyName = Constants._DiffOutputNumbersNightCalls; break;
                    case 22: i.Value.KeyName = Constants._CountOutCallsPeakHours; break;
                    case 23: i.Value.KeyName = Constants._CountConsecutiveCalls; break;
                    case 24: i.Value.KeyName = Constants._CountActiveHours; break;
                    case 25: i.Value.KeyName = Constants._CountFailConsecutiveCalls; break;
                    case 26: i.Value.KeyName = Constants._CountInLowDurationCalls; break;
                    case 27: i.Value.KeyName = Constants._DiffDestZones; break;
                    case 28: i.Value.KeyName = Constants._DiffSourcesZones; break;
                }
                AggregateDefinitionsInfo.Add(i.Value);
            }


            return AggregateDefinitionsInfo.Where(x => x.OperatorTypeAllowed == GlobalConstants._DefaultOperatorType || x.OperatorTypeAllowed == OperatorType.Both).ToList();
        }

    }
}
