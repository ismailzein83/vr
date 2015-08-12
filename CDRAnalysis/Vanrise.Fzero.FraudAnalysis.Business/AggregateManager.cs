using System;
using System.Collections.Generic;
using Vanrise.Fzero.FraudAnalysis.Entities;

namespace Vanrise.Fzero.FraudAnalysis.Business
{
    public class AggregateManager
    {

        List<Strategy> _strategies;
        HashSet<int> _nightCallHours = new HashSet<int>() { 0, 1, 2, 3, 4, 5 };
        private List<string> _AggregateNames = new List<string>(new string[] { "CountOutCalls",  "CountInCalls", "TotalDataVolume","CountOutFails","CountInFails","CountOutSMSs","CountOutOffNets","CountOutOnNets", 
            "CountOutInters","CountInInters","CallOutDurAvg", "CallInDurAvg","TotalOutVolume", "TotalInVolume", "TotalIMEI", "TotalBTS", "DiffOutputNumb", "DiffInputNumbers", "CountInOffNets","CountInOnNets"
            , "DiffOutputNumbNightCalls", "CountOutCallsPeakHours", "CountConsecutiveCalls", "CountActiveHours", "CountFailConsecutiveCalls", "CountInLowDurationCalls"});


        public AggregateManager(List<Strategy> strategies)
        {
            _strategies = strategies;

            //foreach (var i in _strategy.PeakHours)
            //{
            //    _peakHours.Add(i.Id);
            //}
        }

        public AggregateManager()
        {

        }


        public List<AggregateDefinition> GetAggregateDefinitions(List<CallClass> callClasses)
        {
            Dictionary<string, Enums.NetType> callClassNetTypes = new Dictionary<string, Enums.NetType>();
            foreach (CallClass callClass in callClasses)
            {
                callClassNetTypes.Add(callClass.Description, callClass.NetType);
            }

            Func<CDR, Enums.NetType, bool> IsMatchNetType = (cdr, compareToNetType) =>
            {
                Enums.NetType netType;
                if (String.IsNullOrEmpty(cdr.CallClass) || !callClassNetTypes.TryGetValue(cdr.CallClass, out netType))
                    return false;
                else
                    return netType == compareToNetType;

            };


            List<AggregateDefinition> AggregateDefinitions = new List<AggregateDefinition>();


            AggregateDefinitions.Add(new AggregateDefinition()
                {
                    Aggregation = new CountAggregate((cdr) =>
                    {
                        return (cdr.CallType == Enums.CallType.OutgoingVoiceCall);
                    })
                });


            AggregateDefinitions.Add(new AggregateDefinition()
            {
                Aggregation = new CountAggregate((cdr) =>
                {
                    return (cdr.CallType == Enums.CallType.IncomingVoiceCall);
                })
            });


            AggregateDefinitions.Add(new AggregateDefinition()
            {
                Aggregation = new SumAggregate((cdr) =>
                {
                    return (cdr.UpVolume.HasValue ? cdr.UpVolume.Value : 0) + (cdr.DownVolume.HasValue ? cdr.DownVolume.Value : 0);
                })
            });


            AggregateDefinitions.Add(new AggregateDefinition()
            {
                Aggregation = new CountAggregate((cdr) =>
                {
                    return (cdr.CallType == Enums.CallType.OutgoingVoiceCall) && (cdr.DurationInSeconds == 0);
                })
            });


            AggregateDefinitions.Add(new AggregateDefinition()
            {
                Aggregation = new CountAggregate((cdr) =>
                {
                    return (cdr.CallType == Enums.CallType.IncomingVoiceCall) && (cdr.DurationInSeconds == 0);
                })
            });



            AggregateDefinitions.Add(new AggregateDefinition()
            {
                Aggregation = new CountAggregate((cdr) =>
                {
                    return (cdr.CallType == Enums.CallType.OutgoingSms);
                })
            });



            AggregateDefinitions.Add(new AggregateDefinition()
            {
                Aggregation = new CountAggregate((cdr) =>
                {
                    return ((cdr.CallType == Enums.CallType.OutgoingVoiceCall) && (IsMatchNetType(cdr, Enums.NetType.Others)));
                })
            });

            AggregateDefinitions.Add(new AggregateDefinition()
            {
                Aggregation = new CountAggregate((cdr) =>
                {
                    return ((cdr.CallType == Enums.CallType.OutgoingVoiceCall) && (IsMatchNetType(cdr, Enums.NetType.Local)));
                })
            });


            AggregateDefinitions.Add(new AggregateDefinition()
            {
                Aggregation = new CountAggregate((cdr) =>
                {
                    return ((cdr.CallType == Enums.CallType.OutgoingVoiceCall) && (IsMatchNetType(cdr, Enums.NetType.International)));
                })
            });


            AggregateDefinitions.Add(new AggregateDefinition()
            {
                Aggregation = new CountAggregate((cdr) =>
                {
                    return ((cdr.CallType == Enums.CallType.IncomingVoiceCall) && (IsMatchNetType(cdr, Enums.NetType.International)));
                })
            });


            AggregateDefinitions.Add(new AggregateDefinition()
            {
                Aggregation = new SumAggregate(
               (cdr) =>
               {
                   return (cdr.CallType == Enums.CallType.OutgoingVoiceCall && cdr.DurationInSeconds.HasValue) ? cdr.DurationInSeconds.Value / 60 : 0;
               }
            )
            });

            AggregateDefinitions.Add(new AggregateDefinition()
            {
                Aggregation = new SumAggregate(
               (cdr) =>
               {
                   return (cdr.CallType == Enums.CallType.IncomingVoiceCall && cdr.DurationInSeconds.HasValue) ? cdr.DurationInSeconds.Value / 60 : 0;
               }
            )
            });


            AggregateDefinitions.Add(new AggregateDefinition()
            {
                Aggregation = new SumAggregate(
                (cdr) =>
                {
                    return (cdr.CallType == Enums.CallType.OutgoingVoiceCall && cdr.DurationInSeconds.HasValue) ? cdr.DurationInSeconds.Value : 0;
                }
            )
            });





            AggregateDefinitions.Add(new AggregateDefinition()
            {
                Aggregation = new SumAggregate(
                (cdr) =>
                {
                    return (cdr.CallType == Enums.CallType.IncomingVoiceCall && cdr.DurationInSeconds.HasValue) ? cdr.DurationInSeconds.Value : 0;
                }
            )
            });

            AggregateDefinitions.Add(new AggregateDefinition()
            {
                Aggregation = new DistinctCountAggregate(
                   (cdr) =>
                   {
                       return cdr.IMEI;
                   },

                   (cdr) =>
                   {
                       return (cdr.CallType == Enums.CallType.IncomingVoiceCall || cdr.CallType == Enums.CallType.OutgoingVoiceCall || cdr.CallType == Enums.CallType.IncomingSms || cdr.CallType == Enums.CallType.OutgoingSms);
                   }
               )
            });


            AggregateDefinitions.Add(new AggregateDefinition()
            {
                Aggregation = new DistinctCountAggregate(
                      (cdr) =>
                      {
                          return cdr.BTSId;
                      },

                      (cdr) =>
                      {
                          return (cdr.BTSId.HasValue && (cdr.CallType == Enums.CallType.IncomingVoiceCall || cdr.CallType == Enums.CallType.OutgoingVoiceCall || cdr.CallType == Enums.CallType.IncomingSms || cdr.CallType == Enums.CallType.OutgoingSms));
                      }
                  )
            });


            AggregateDefinitions.Add(new AggregateDefinition()
            {
                Aggregation = new DistinctCountAggregate(
                     (cdr) =>
                     {
                         return cdr.Destination;
                     },

                     (cdr) =>
                     {
                         return (cdr.CallType == Enums.CallType.OutgoingVoiceCall);
                     }
                 )
            });


            AggregateDefinitions.Add(new AggregateDefinition()
            {
                Aggregation = new DistinctCountAggregate(
                     (cdr) =>
                     {
                         return cdr.Destination;
                     },

                     (cdr) =>
                     {
                         return (cdr.CallType == Enums.CallType.IncomingVoiceCall);
                     }
                 )
            });


            AggregateDefinitions.Add(new AggregateDefinition()
            {
                Aggregation = new CountAggregate((cdr) =>
                {
                    return ((cdr.CallType == Enums.CallType.IncomingVoiceCall) && (IsMatchNetType(cdr, Enums.NetType.Others)));
                })
            });


            AggregateDefinitions.Add(new AggregateDefinition()
            {
                Aggregation = new CountAggregate((cdr) =>
                {
                    return ((cdr.CallType == Enums.CallType.IncomingVoiceCall) && (IsMatchNetType(cdr, Enums.NetType.Local)));
                })
            });


            AggregateDefinitions.Add(new AggregateDefinition()
            {
                Aggregation = new DistinctCountAggregate(
                     (cdr) =>
                     {
                         return cdr.Destination;
                     },

                     (cdr) =>
                     {
                         return (cdr.CallType == Enums.CallType.OutgoingVoiceCall && cdr.ConnectDateTime.HasValue && _nightCallHours.Contains(cdr.ConnectDateTime.Value.Hour));
                     }
                 )
            });


            AggregateDefinitions.Add(new AggregateDefinition()
            {
                Aggregation = new CountAggregate((cdr, strategy) =>
                {
                    return (cdr.CallType == Enums.CallType.OutgoingVoiceCall && cdr.ConnectDateTime.HasValue && strategy.PeakHoursIds.Contains(cdr.ConnectDateTime.Value.Hour));
                }, _strategies)
            });


            AggregateDefinitions.Add(new AggregateDefinition()
            {
                Aggregation = new ConsecutiveAggregate(
                      (cdr, strategy) =>
                      {
                          return (cdr.CallType == Enums.CallType.OutgoingVoiceCall);
                      }
                      , _strategies
                  )
            });




            AggregateDefinitions.Add(new AggregateDefinition()
            {
                Aggregation = new GroupCountAggregate(
                      (cdr, strategy) =>
                      {
                          return (cdr.CallType == Enums.CallType.OutgoingVoiceCall);
                      }
                      , _strategies
                  )
            });



            AggregateDefinitions.Add(new AggregateDefinition()
            {
                Aggregation = new FailedConsecutiveAggregate(
                      (cdr, strategy) =>
                      {
                          return (cdr.CallType == Enums.CallType.OutgoingVoiceCall && cdr.DurationInSeconds == 0);
                      }
                      , _strategies
                  )
            });


            AggregateDefinitions.Add(new AggregateDefinition()
            {
                Aggregation = new CountAggregate((cdr, strategy) =>
                {
                    return (cdr.CallType == Enums.CallType.IncomingVoiceCall && cdr.DurationInSeconds <= strategy.MaxLowDurationCall);
                }, _strategies)
            });



            for(int i=0; i<_AggregateNames.Count; i++)
            {
                AggregateDefinitions[i].Name = _AggregateNames[i];
            }


            return AggregateDefinitions;
        }

       

        public List<string> GetAggregateNames()
        {
            return _AggregateNames;
        }

    }
}
