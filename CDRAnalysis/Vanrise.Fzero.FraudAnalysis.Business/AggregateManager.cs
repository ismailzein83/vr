using System;
using System.Collections.Generic;
using System.Linq;
using Vanrise.Fzero.CDRImport.Entities;
using Vanrise.Fzero.FraudAnalysis.Aggregates;
using Vanrise.Fzero.FraudAnalysis.Entities;

namespace Vanrise.Fzero.FraudAnalysis.Business
{
    public class AggregateManager
    {
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


            foreach (var i in AggregateDefinitions)
            {
                foreach (var j in GetAggregateDefinitionsInfo())
                {
                    if (i.Id == j.Id)
                    {
                        i.Name = j.Name;
                        i.OperatorTypeAllowed = j.OperatorTypeAllowed;
                    }
                }
            }



            return AggregateDefinitions.Where(x => x.OperatorTypeAllowed == Constants._DefaultOperatorType || x.OperatorTypeAllowed == OperatorType.Mobile).ToList();
        }

        public List<AggregateDefinitionInfo> GetAggregateDefinitionsInfo()
        {
            List<AggregateDefinitionInfo> AggregateDefinitionsInfo = new List<AggregateDefinitionInfo>();

            AggregateDefinitionsInfo.Add(new AggregateDefinitionInfo() { Id = 1, Name = Constants._CountOutCalls, OperatorTypeAllowed = OperatorType.Both, NumberPrecision = Constants._NoDecimal });
            AggregateDefinitionsInfo.Add(new AggregateDefinitionInfo() { Id = 2, Name = Constants._CountInCalls, OperatorTypeAllowed = OperatorType.Both, NumberPrecision = Constants._NoDecimal });
            AggregateDefinitionsInfo.Add(new AggregateDefinitionInfo() { Id = 3, Name = Constants._TotalDataVolume, OperatorTypeAllowed = OperatorType.Mobile, NumberPrecision = Constants._LongPrecision });
            AggregateDefinitionsInfo.Add(new AggregateDefinitionInfo() { Id = 4, Name = Constants._CountOutFails, OperatorTypeAllowed = OperatorType.Both, NumberPrecision = Constants._NoDecimal });
            AggregateDefinitionsInfo.Add(new AggregateDefinitionInfo() { Id = 5, Name = Constants._CountInFails, OperatorTypeAllowed = OperatorType.Both, NumberPrecision = Constants._NoDecimal });
            AggregateDefinitionsInfo.Add(new AggregateDefinitionInfo() { Id = 6, Name = Constants._CountOutSMSs, OperatorTypeAllowed = OperatorType.Mobile, NumberPrecision = Constants._NoDecimal });
            AggregateDefinitionsInfo.Add(new AggregateDefinitionInfo() { Id = 7, Name = Constants._CountOutOffNets, OperatorTypeAllowed = OperatorType.Both, NumberPrecision = Constants._NoDecimal });
            AggregateDefinitionsInfo.Add(new AggregateDefinitionInfo() { Id = 8, Name = Constants._CountOutOnNets, OperatorTypeAllowed = OperatorType.Both, NumberPrecision = Constants._NoDecimal });
            AggregateDefinitionsInfo.Add(new AggregateDefinitionInfo() { Id = 9, Name = Constants._CountOutInters, OperatorTypeAllowed = OperatorType.Both, NumberPrecision = Constants._NoDecimal });
            AggregateDefinitionsInfo.Add(new AggregateDefinitionInfo() { Id = 10, Name = Constants._CountInInters, OperatorTypeAllowed = OperatorType.Both, NumberPrecision = Constants._NoDecimal });
            AggregateDefinitionsInfo.Add(new AggregateDefinitionInfo() { Id = 11, Name = Constants._CallOutDurAvg, OperatorTypeAllowed = OperatorType.Both, NumberPrecision = Constants._LongPrecision });
            AggregateDefinitionsInfo.Add(new AggregateDefinitionInfo() { Id = 12, Name = Constants._CallInDurAvg, OperatorTypeAllowed = OperatorType.Both, NumberPrecision = Constants._LongPrecision });
            AggregateDefinitionsInfo.Add(new AggregateDefinitionInfo() { Id = 13, Name = Constants._TotalOutVolume, OperatorTypeAllowed = OperatorType.Mobile, NumberPrecision = Constants._LongPrecision });
            AggregateDefinitionsInfo.Add(new AggregateDefinitionInfo() { Id = 14, Name = Constants._TotalInVolume, OperatorTypeAllowed = OperatorType.Mobile, NumberPrecision = Constants._LongPrecision });
            AggregateDefinitionsInfo.Add(new AggregateDefinitionInfo() { Id = 15, Name = Constants._TotalIMEI, OperatorTypeAllowed = OperatorType.Mobile, NumberPrecision = Constants._NoDecimal });
            AggregateDefinitionsInfo.Add(new AggregateDefinitionInfo() { Id = 16, Name = Constants._TotalBTS, OperatorTypeAllowed = OperatorType.Mobile, NumberPrecision = Constants._NoDecimal });
            AggregateDefinitionsInfo.Add(new AggregateDefinitionInfo() { Id = 17, Name = Constants._DiffOutputNumbers, OperatorTypeAllowed = OperatorType.Both, NumberPrecision = Constants._NoDecimal });
            AggregateDefinitionsInfo.Add(new AggregateDefinitionInfo() { Id = 18, Name = Constants._DiffInputNumbers, OperatorTypeAllowed = OperatorType.Both, NumberPrecision = Constants._NoDecimal });
            AggregateDefinitionsInfo.Add(new AggregateDefinitionInfo() { Id = 19, Name = Constants._CountInOffNet, OperatorTypeAllowed = OperatorType.Both, NumberPrecision = Constants._NoDecimal });
            AggregateDefinitionsInfo.Add(new AggregateDefinitionInfo() { Id = 20, Name = Constants._CountInOnNets, OperatorTypeAllowed = OperatorType.Both, NumberPrecision = Constants._NoDecimal });
            AggregateDefinitionsInfo.Add(new AggregateDefinitionInfo() { Id = 21, Name = Constants._DiffOutputNumbersNightCalls, OperatorTypeAllowed = OperatorType.Both, NumberPrecision = Constants._NoDecimal });
            AggregateDefinitionsInfo.Add(new AggregateDefinitionInfo() { Id = 22, Name = Constants._CountOutCallsPeakHours, OperatorTypeAllowed = OperatorType.Both, NumberPrecision = Constants._NoDecimal });
            AggregateDefinitionsInfo.Add(new AggregateDefinitionInfo() { Id = 23, Name = Constants._CountConsecutiveCalls, OperatorTypeAllowed = OperatorType.Both, NumberPrecision = Constants._NoDecimal });
            AggregateDefinitionsInfo.Add(new AggregateDefinitionInfo() { Id = 24, Name = Constants._CountActiveHours, OperatorTypeAllowed = OperatorType.Both, NumberPrecision = Constants._NoDecimal });
            AggregateDefinitionsInfo.Add(new AggregateDefinitionInfo() { Id = 25, Name = Constants._CountFailConsecutiveCalls, OperatorTypeAllowed = OperatorType.Both, NumberPrecision = Constants._NoDecimal });
            AggregateDefinitionsInfo.Add(new AggregateDefinitionInfo() { Id = 26, Name = Constants._CountInLowDurationCalls, OperatorTypeAllowed = OperatorType.Both, NumberPrecision = Constants._NoDecimal });
            AggregateDefinitionsInfo.Add(new AggregateDefinitionInfo() { Id = 27, Name = Constants._DiffDestZones, OperatorTypeAllowed = OperatorType.PSTN, NumberPrecision = Constants._NoDecimal });
            AggregateDefinitionsInfo.Add(new AggregateDefinitionInfo() { Id = 28, Name = Constants._DiffSourcesZones, OperatorTypeAllowed = OperatorType.PSTN, NumberPrecision = Constants._NoDecimal });


            return AggregateDefinitionsInfo.Where(x => x.OperatorTypeAllowed == Constants._DefaultOperatorType || x.OperatorTypeAllowed == OperatorType.Both).ToList();
        }

    }
}
