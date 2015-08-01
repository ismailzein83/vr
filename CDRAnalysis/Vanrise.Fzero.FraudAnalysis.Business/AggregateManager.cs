using System;
using System.Collections.Generic;
using Vanrise.Fzero.FraudAnalysis.Entities;

namespace Vanrise.Fzero.FraudAnalysis.Business
{
    public class AggregateManager
    {

        List<Strategy> _strategies;
        HashSet<int> _nightCallHours = new HashSet<int>() { 0, 1, 2, 3, 4, 5 };
        //HashSet<int> _peakHours = new HashSet<int>();


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
                Name = "CountOutCalls",
                Aggregation = new CountAggregate((cdr) =>
                {
                    return (cdr.CallType == Enums.CallType.OutgoingVoiceCall);
                })
            });


            AggregateDefinitions.Add(new AggregateDefinition()
            {
                Name = "CountInCalls",
                Aggregation = new CountAggregate((cdr) =>
                {
                    return (cdr.CallType == Enums.CallType.IncomingVoiceCall);
                })
            });


            AggregateDefinitions.Add(new AggregateDefinition()
            {
                Name = "TotalDataVolume",
                Aggregation = new SumAggregate((cdr) =>
                {
                    return (cdr.UpVolume.HasValue ? cdr.UpVolume.Value : 0) + (cdr.DownVolume.HasValue ? cdr.DownVolume.Value : 0);
                })
            });


            AggregateDefinitions.Add(new AggregateDefinition()
            {
                Name = "CountOutFails",
                Aggregation = new CountAggregate((cdr) =>
                {
                    return (cdr.CallType == Enums.CallType.OutgoingVoiceCall) && (cdr.DurationInSeconds == 0);
                })
            });


            AggregateDefinitions.Add(new AggregateDefinition()
            {
                Name = "CountInFails",
                Aggregation = new CountAggregate((cdr) =>
                {
                    return (cdr.CallType == Enums.CallType.IncomingVoiceCall) && (cdr.DurationInSeconds == 0);
                })
            });



            AggregateDefinitions.Add(new AggregateDefinition()
            {
                Name = "CountOutSMSs",
                Aggregation = new CountAggregate((cdr) =>
                {
                    return (cdr.CallType == Enums.CallType.OutgoingSms);
                })
            });



            AggregateDefinitions.Add(new AggregateDefinition()
            {
                Name = "CountOutOffNets",
                Aggregation = new CountAggregate((cdr) =>
                {
                    return ((cdr.CallType == Enums.CallType.OutgoingVoiceCall) && (IsMatchNetType(cdr, Enums.NetType.Others)));
                })
            });

            AggregateDefinitions.Add(new AggregateDefinition()
            {
                Name = "CountOutOnNets",
                Aggregation = new CountAggregate((cdr) =>
                {
                    return ((cdr.CallType == Enums.CallType.OutgoingVoiceCall) && (IsMatchNetType(cdr, Enums.NetType.Local)));
                })
            });


            AggregateDefinitions.Add(new AggregateDefinition()
            {
                Name = "CountOutInters",
                Aggregation = new CountAggregate((cdr) =>
                {
                    return ((cdr.CallType == Enums.CallType.OutgoingVoiceCall) && (IsMatchNetType(cdr, Enums.NetType.International)));
                })
            });


            AggregateDefinitions.Add(new AggregateDefinition()
            {
                Name = "CountInInters",
                Aggregation = new CountAggregate((cdr) =>
                {
                    return ((cdr.CallType == Enums.CallType.IncomingVoiceCall) && (IsMatchNetType(cdr, Enums.NetType.International)));
                })
            });


            AggregateDefinitions.Add(new AggregateDefinition()
            {
                Name = "CallOutDurAvg",
                Aggregation = new SumAggregate(
               (cdr) =>
               {
                   return (cdr.CallType == Enums.CallType.OutgoingVoiceCall && cdr.DurationInSeconds.HasValue) ? cdr.DurationInSeconds.Value / 60 : 0;
               }
            )
            });

            AggregateDefinitions.Add(new AggregateDefinition()
            {
                Name = "CallInDurAvg",
                Aggregation = new SumAggregate(
               (cdr) =>
               {
                   return (cdr.CallType == Enums.CallType.IncomingVoiceCall && cdr.DurationInSeconds.HasValue) ? cdr.DurationInSeconds.Value / 60 : 0;
               }
            )
            });


            AggregateDefinitions.Add(new AggregateDefinition()
            {
                Name = "TotalOutVolume",
                Aggregation = new SumAggregate(
                (cdr) =>
                {
                    return (cdr.CallType == Enums.CallType.OutgoingVoiceCall && cdr.DurationInSeconds.HasValue) ? cdr.DurationInSeconds.Value : 0;
                }
            )
            });





            AggregateDefinitions.Add(new AggregateDefinition()
            {
                Name = "TotalInVolume",
                Aggregation = new SumAggregate(
                (cdr) =>
                {
                    return (cdr.CallType == Enums.CallType.IncomingVoiceCall && cdr.DurationInSeconds.HasValue) ? cdr.DurationInSeconds.Value : 0;
                }
            )
            });

            AggregateDefinitions.Add(new AggregateDefinition()
            {
                Name = "TotalIMEI",
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
                Name = "TotalBTS",
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
                Name = "DiffOutputNumb",
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
                Name = "DiffInputNumbers",
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
                Name = "CountInOffNets",
                Aggregation = new CountAggregate((cdr) =>
                {
                    return ((cdr.CallType == Enums.CallType.IncomingVoiceCall) && (IsMatchNetType(cdr, Enums.NetType.Others)));
                })
            });


            AggregateDefinitions.Add(new AggregateDefinition()
            {
                Name = "CountInOnNets",
                Aggregation = new CountAggregate((cdr) =>
                {
                    return ((cdr.CallType == Enums.CallType.IncomingVoiceCall) && (IsMatchNetType(cdr, Enums.NetType.Local)));
                })
            });


            AggregateDefinitions.Add(new AggregateDefinition()
            {
                Name = "DiffOutputNumbNightCalls",
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
                Name = "CountOutCallsPeakHours",
                Aggregation = new CountAggregate((cdr, strategy) =>
                {
                    return (cdr.CallType == Enums.CallType.OutgoingVoiceCall && cdr.ConnectDateTime.HasValue && strategy.PeakHoursIds.Contains(cdr.ConnectDateTime.Value.Hour));
                }, _strategies)
            });


            AggregateDefinitions.Add(new AggregateDefinition()
            {
                Name = "CountConsecutiveCalls",
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
                Name = "CountActiveHours",
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
                Name = "CountFailConsecutiveCalls",
                Aggregation = new ConsecutiveAggregate(
                      (cdr, strategy) =>
                      {
                          return (cdr.CallType == Enums.CallType.OutgoingVoiceCall && cdr.DurationInSeconds == 0);
                      }
                      , _strategies
                  )
            });


            AggregateDefinitions.Add(new AggregateDefinition()
            {
                Name = "CountInLowDurationCalls",
                Aggregation = new CountAggregate((cdr, strategy) =>
                {
                    return (cdr.CallType == Enums.CallType.IncomingVoiceCall && cdr.DurationInSeconds <= strategy.MaxLowDurationCall);
                }, _strategies)
            });

            return AggregateDefinitions;
        }

        public List<AggregateDefinition> GetAggregateNames()
        {
            List<AggregateDefinition> AggregateDefinitions = new List<AggregateDefinition>();

            AggregateDefinitions.Add(new AggregateDefinition()
            {
                AggregateId=1,
                Name = "CountOutCalls"
            });


            AggregateDefinitions.Add(new AggregateDefinition()
            {
                AggregateId = 2,
                Name = "CountInCalls"
            });


            AggregateDefinitions.Add(new AggregateDefinition()
            {
                AggregateId = 3,
                Name = "TotalDataVolume"
            });


            AggregateDefinitions.Add(new AggregateDefinition()
            {
                AggregateId = 4,
                Name = "CountOutFails"
            });


            AggregateDefinitions.Add(new AggregateDefinition()
            {
                AggregateId = 5,
                Name = "CountInFails"
            });



            AggregateDefinitions.Add(new AggregateDefinition()
            {
                AggregateId = 6,
                Name = "CountOutSMSs"
            });



            AggregateDefinitions.Add(new AggregateDefinition()
            {
                AggregateId = 7,
                Name = "CountOutOffNets"
            });

            AggregateDefinitions.Add(new AggregateDefinition()
            {
                AggregateId = 8,
                Name = "CountOutOnNets"
            });


            AggregateDefinitions.Add(new AggregateDefinition()
            {
                AggregateId = 9,
                Name = "CountOutInters"
            });


            AggregateDefinitions.Add(new AggregateDefinition()
            {
                AggregateId = 10,
                Name = "CountInInters"
            });


            AggregateDefinitions.Add(new AggregateDefinition()
             {
                AggregateId=11,
                Name = "CallOutDurAvg"
            });

            AggregateDefinitions.Add(new AggregateDefinition()
             {
                AggregateId=12,
                Name = "CallInDurAvg"
            });


            AggregateDefinitions.Add(new AggregateDefinition()
             {
                AggregateId=13,
                Name = "TotalOutVolume"
            });





            AggregateDefinitions.Add(new AggregateDefinition()
             {
                AggregateId=14,
                Name = "TotalInVolume"
            });

            AggregateDefinitions.Add(new AggregateDefinition()
             {
                AggregateId=15,
                Name = "TotalIMEI"
            });


            AggregateDefinitions.Add(new AggregateDefinition()
             {
                AggregateId=16,
                Name = "TotalBTS"
            });


            AggregateDefinitions.Add(new AggregateDefinition()
             {
                AggregateId=17,
                Name = "DiffOutputNumb"
            });


            AggregateDefinitions.Add(new AggregateDefinition()
             {
                AggregateId=18,
                Name = "DiffInputNumbers"
            });


            AggregateDefinitions.Add(new AggregateDefinition()
             {
                AggregateId=19,
                Name = "CountInOffNets"
            });


            AggregateDefinitions.Add(new AggregateDefinition()
             {
                AggregateId=20,
                Name = "CountInOnNets"
            });


            AggregateDefinitions.Add(new AggregateDefinition()
             {
                AggregateId=21,
                Name = "DiffOutputNumbNightCalls"
            });


            AggregateDefinitions.Add(new AggregateDefinition()
             {
                AggregateId=22,
                Name = "CountOutCallsPeakHours"
            });


            AggregateDefinitions.Add(new AggregateDefinition()
             {
                AggregateId=23,
                Name = "CountConsecutiveCalls"
            });




            AggregateDefinitions.Add(new AggregateDefinition()
             {
                AggregateId=24,
                Name = "CountActiveHours"
            });



            AggregateDefinitions.Add(new AggregateDefinition()
             {
                AggregateId=25,
                Name = "CountFailConsecutiveCalls"
            });


            AggregateDefinitions.Add(new AggregateDefinition()
             {
                AggregateId=26,
                Name = "CountInLowDurationCalls"
            });

            return AggregateDefinitions;
        }

    }
}
