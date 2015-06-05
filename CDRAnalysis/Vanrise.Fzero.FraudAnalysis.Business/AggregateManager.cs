using System;
using System.Collections.Generic;
using Vanrise.Fzero.FraudAnalysis.Entities;

namespace Vanrise.Fzero.FraudAnalysis.Business
{
    public class AggregateManager
    {


        IList<int> NightCallHours = new List<int>() { 0,1,2,3,4,5};
        IList<int> PeakHours = new List<int>() { 11,12,13,14 };
        int LowDurationMaxValue = 20;


        public List<AggregateDefinition> GetAggregateDefinitions(List<CallClass> CallClasses)
        {
            List<AggregateDefinition> AggregateDefinitions = new List<AggregateDefinition>();



            AggregateDefinitions.Add(new AggregateDefinition()
            {
                Name = "CountOutLowDurationCalls",
                Aggregation = new CountAggregate((cdr) =>
                {
                    return (cdr.CallType == Enums.CallType.OutgoingVoiceCall  &&  cdr.DurationInSeconds <=LowDurationMaxValue);
                })
            });


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
                }, (cdr) =>
                {
                    return (cdr.UpVolume.HasValue || cdr.DownVolume.HasValue );
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
                    return ((cdr.CallType == Enums.CallType.OutgoingVoiceCall) && (CallClasses.FindAll(x=>x.NetType==Enums.NetType.Others &&  x.Description==cdr.CallClass).Count>0));
                })
            });

            AggregateDefinitions.Add(new AggregateDefinition()
            {
                Name = "CountOutOnNets",
                Aggregation = new CountAggregate((cdr) =>
                {
                    return ((cdr.CallType == Enums.CallType.OutgoingVoiceCall) && (CallClasses.FindAll(x => x.NetType == Enums.NetType.Local && x.Description == cdr.CallClass).Count > 0));
                })
            });


            AggregateDefinitions.Add(new AggregateDefinition()
            {
                Name = "CountOutInters",
                Aggregation = new CountAggregate((cdr) =>
                {
                    return ((cdr.CallType == Enums.CallType.OutgoingVoiceCall) && (CallClasses.FindAll(x => x.NetType == Enums.NetType.International && x.Description == cdr.CallClass).Count > 0));
                })
            });



            AggregateDefinitions.Add(new AggregateDefinition()
            {
                Name = "CountInOffNets",
                Aggregation = new CountAggregate((cdr) =>
                {
                    return ((cdr.CallType == Enums.CallType.IncomingVoiceCall) && (CallClasses.FindAll(x => x.NetType == Enums.NetType.Others && x.Description == cdr.CallClass).Count > 0));
                })
            });



            AggregateDefinitions.Add(new AggregateDefinition()
            {
                Name = "CountInOnNets",
                Aggregation = new CountAggregate((cdr) =>
                {
                    return ((cdr.CallType == Enums.CallType.IncomingVoiceCall) && (CallClasses.FindAll(x => x.NetType == Enums.NetType.Local && x.Description == cdr.CallClass).Count > 0));
                })
            });



            AggregateDefinitions.Add(new AggregateDefinition()
            {
                Name = "CountInInters",
                Aggregation = new CountAggregate((cdr) =>
                {
                    return ((cdr.CallType == Enums.CallType.IncomingVoiceCall) && (CallClasses.FindAll(x => x.NetType == Enums.NetType.International && x.Description == cdr.CallClass).Count > 0));
                })
            });



            AggregateDefinitions.Add(new AggregateDefinition()
            {
                Name = "CallOutDurs",
                Aggregation = new SumAggregate(
               (cdr) =>
               {
                   return cdr.DurationInSeconds.Value / 60;
               },

               (cdr) =>
               {
                   return (cdr.DurationInSeconds.HasValue && cdr.DurationInSeconds != 0 && cdr.CallType == Enums.CallType.OutgoingVoiceCall);
               }
            )
            });


            AggregateDefinitions.Add(new AggregateDefinition()
            {
                Name = "TotalOutVolume",
                Aggregation = new SumAggregate(
                (cdr) =>
                {
                    return cdr.DurationInSeconds.Value;
                },

                (cdr) =>
                {
                    return (cdr.DurationInSeconds.HasValue && cdr.CallType == Enums.CallType.OutgoingVoiceCall);
                }
            )
            });



            AggregateDefinitions.Add(new AggregateDefinition()
            {
                Name = "CallInDurs",
                Aggregation = new SumAggregate(
               (cdr) =>
               {
                   return decimal.Parse(( cdr.DurationInSeconds.Value / 60).ToString());
               },

               (cdr) =>
               {
                   return (cdr.DurationInSeconds.HasValue && cdr.DurationInSeconds != 0 && cdr.CallType == Enums.CallType.IncomingVoiceCall);
               }
            )
            });



            AggregateDefinitions.Add(new AggregateDefinition()
            {
                Name = "TotalInVolume",
                Aggregation = new SumAggregate(
                (cdr) =>
                {
                    return cdr.DurationInSeconds.Value;
                },

                (cdr) =>
                {
                    return (cdr.DurationInSeconds.HasValue && cdr.CallType == Enums.CallType.IncomingVoiceCall);
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
                       return ((cdr.CallType == Enums.CallType.IncomingVoiceCall || cdr.CallType == Enums.CallType.OutgoingVoiceCall || cdr.CallType == Enums.CallType.IncomingSms || cdr.CallType == Enums.CallType.OutgoingSms));
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
                Name = "DiffOutputNumbNightCalls",
                Aggregation = new DistinctCountAggregate(
                     (cdr) =>
                     {
                         return cdr.Destination;
                     },

                     (cdr) =>
                     {
                         return (cdr.CallType == Enums.CallType.OutgoingVoiceCall && cdr.ConnectDateTime.HasValue && NightCallHours.Contains(cdr.ConnectDateTime.Value.Hour));
                     }
                 )
            });




            AggregateDefinitions.Add(new AggregateDefinition()
            {
                Name = "CountOutCallsPeakHours",
                Aggregation = new CountAggregate((cdr) =>
                {
                    return (cdr.CallType == Enums.CallType.OutgoingVoiceCall && cdr.ConnectDateTime.HasValue && PeakHours.Contains(cdr.ConnectDateTime.Value.Hour));
                })
            });


            AggregateDefinitions.Add(new AggregateDefinition()
            {
                Name = "CountConsecutiveCalls",
                Aggregation = new ConsecutiveAggregate(
                      (cdr) =>
                      {
                          return (cdr.CallType == Enums.CallType.OutgoingVoiceCall);
                      }
                  )
            });


          




            AggregateDefinitions.Add(new AggregateDefinition()
            {
                Name = "CountActiveHours",
                Aggregation = new GroupCountAggregate(
                      (cdr) =>
                      {
                          return (cdr.CallType == Enums.CallType.OutgoingVoiceCall);
                      }
                  )
            });



            AggregateDefinitions.Add(new AggregateDefinition()
            {
                Name = "CountFailConsecutiveCalls",
                Aggregation = new ConsecutiveAggregate(
                      (cdr) =>
                      {
                          return (cdr.CallType == Enums.CallType.OutgoingVoiceCall && cdr.DurationInSeconds==0);
                      }
                  )
            });




            return AggregateDefinitions; 
        }

    }
}
