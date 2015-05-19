using System;
using System.Collections.Generic;
using Vanrise.Fzero.FraudAnalysis.Entities;

namespace Vanrise.Fzero.FraudAnalysis.Business
{
    public class AggregateManager
    {
        public List<AggregateDefinition> GetAggregateDefinitions()
        {
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
                    return decimal.Parse((!cdr.UpVolume.HasValue && !cdr.DownVolume.HasValue ? 0:((!cdr.UpVolume.HasValue && cdr.DownVolume.HasValue)? cdr.DownVolume.Value: ((cdr.UpVolume.HasValue && !cdr.DownVolume.HasValue)? cdr.DownVolume.Value:0))).ToString());
                }, null)
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
                    return (cdr.CallType == Enums.CallType.OutgoingVoiceCall && (cdr.CallClass == Enums.CallClass.ASIACELL) || cdr.CallClass == Enums.CallClass.KOREKTEL);
                })
            });

            AggregateDefinitions.Add(new AggregateDefinition()
            {
                Name = "CountOutOnNets",
                Aggregation = new CountAggregate((cdr) =>
                {
                    return (cdr.CallType == Enums.CallType.OutgoingVoiceCall && (cdr.CallClass == Enums.CallClass.ZAINIQ|| cdr.CallClass == Enums.CallClass.VAS || cdr.CallClass == Enums.CallClass.INV));
                })
            });


            AggregateDefinitions.Add(new AggregateDefinition()
            {
                Name = "CountOutInters",
                Aggregation = new CountAggregate((cdr) =>
                {
                    return (cdr.CallType == Enums.CallType.OutgoingVoiceCall && cdr.CallClass == Enums.CallClass.INTL);
                })
            });



            AggregateDefinitions.Add(new AggregateDefinition()
            {
                Name = "CountInOffNets",
                Aggregation = new CountAggregate((cdr) =>
                {
                    return (cdr.CallType == Enums.CallType.IncomingVoiceCall && (cdr.CallClass == Enums.CallClass.ASIACELL || cdr.CallClass == Enums.CallClass.KOREKTEL));
                })
            });



            AggregateDefinitions.Add(new AggregateDefinition()
            {
                Name = "CountInOnNets",
                Aggregation = new CountAggregate((cdr) =>
                {
                    return (cdr.CallType == Enums.CallType.IncomingVoiceCall && ((cdr.CallClass == Enums.CallClass.ZAINIQ ) || cdr.CallClass == Enums.CallClass.VAS) || cdr.CallClass == Enums.CallClass.INV);
                })
            });



            AggregateDefinitions.Add(new AggregateDefinition()
            {
                Name = "CountInInters",
                Aggregation = new CountAggregate((cdr) =>
                {
                    return (cdr.CallType == Enums.CallType.IncomingVoiceCall && (cdr.CallClass == Enums.CallClass.INTL));
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



            return AggregateDefinitions; 
        }

    }
}
