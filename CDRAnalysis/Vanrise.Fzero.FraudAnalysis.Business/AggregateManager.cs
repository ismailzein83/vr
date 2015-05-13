using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
                    return (cdr.CallType == (int)Enums.CallType.outgoingVoiceCall);
                })
            });


            AggregateDefinitions.Add(new AggregateDefinition()
            {
                Name = "CountInCalls",
                Aggregation = new CountAggregate((cdr) =>
                {
                    return (cdr.CallType == (int)Enums.CallType.incomingVoiceCall);
                })
            });


            AggregateDefinitions.Add(new AggregateDefinition()
            {
                Name = "TotalDataVolume",
                Aggregation = new SumAggregate((cdr) =>
                {
                    return cdr.UpVolume + cdr.DownVolume;
                }, null)
            });


            AggregateDefinitions.Add(new AggregateDefinition()
            {
                Name = "CountOutFails",
                Aggregation = new CountAggregate((cdr) =>
                {
                    return (cdr.CallType == (int)Enums.CallType.outgoingVoiceCall) && (cdr.DurationInSeconds == 0);
                })
            });


            AggregateDefinitions.Add(new AggregateDefinition()
            {
                Name = "CountInFails",
                Aggregation = new CountAggregate((cdr) =>
                {
                    return (cdr.CallType == (int)Enums.CallType.incomingVoiceCall) && (cdr.DurationInSeconds == 0);
                })
            });



            AggregateDefinitions.Add(new AggregateDefinition()
            {
                Name = "CountOutSMSs",
                Aggregation = new CountAggregate((cdr) =>
                {
                    return (cdr.CallType == (int)Enums.CallType.outgoingSms);
                })
            });


            AggregateDefinitions.Add(new AggregateDefinition()
            {
                Name = "CountOutOffNets",
                Aggregation = new CountAggregate((cdr) =>
                {
                    return (cdr.CallType == (int)Enums.CallType.outgoingVoiceCall && (cdr.CallClass == Enum.GetName(typeof(Enums.CallClass), (int)Enums.CallClass.ASIACELL) || cdr.CallClass == Enum.GetName(typeof(Enums.CallClass), (int)Enums.CallClass.KOREKTEL)));
                })
            });

            AggregateDefinitions.Add(new AggregateDefinition()
            {
                Name = "CountOutOnNets",
                Aggregation = new CountAggregate((cdr) =>
                {
                    return (cdr.CallType == (int)Enums.CallType.outgoingVoiceCall && ((cdr.CallClass == Enum.GetName(typeof(Enums.CallClass), (int)Enums.CallClass.ZAINIQ) || cdr.CallClass == Enum.GetName(typeof(Enums.CallClass), (int)Enums.CallClass.VAS) || cdr.CallClass == Enum.GetName(typeof(Enums.CallClass), (int)Enums.CallClass.INV))));
                })
            });


            AggregateDefinitions.Add(new AggregateDefinition()
            {
                Name = "CountOutInters",
                Aggregation = new CountAggregate((cdr) =>
                {
                    return (cdr.CallType == (int)Enums.CallType.outgoingVoiceCall && (cdr.CallClass == Enum.GetName(typeof(Enums.CallClass), (int)Enums.CallClass.INTL)));
                })
            });



            AggregateDefinitions.Add(new AggregateDefinition()
            {
                Name = "CountInOffNets",
                Aggregation = new CountAggregate((cdr) =>
                {
                    return (cdr.CallType == (int)Enums.CallType.incomingVoiceCall && (cdr.CallClass == Enum.GetName(typeof(Enums.CallClass), (int)Enums.CallClass.ASIACELL) || cdr.CallClass == Enum.GetName(typeof(Enums.CallClass), (int)Enums.CallClass.KOREKTEL)));
                })
            });



            AggregateDefinitions.Add(new AggregateDefinition()
            {
                Name = "CountInOnNets",
                Aggregation = new CountAggregate((cdr) =>
                {
                    return (cdr.CallType == (int)Enums.CallType.incomingVoiceCall && ((cdr.CallClass == Enum.GetName(typeof(Enums.CallClass), (int)Enums.CallClass.ZAINIQ) || cdr.CallClass == Enum.GetName(typeof(Enums.CallClass), (int)Enums.CallClass.VAS) || cdr.CallClass == Enum.GetName(typeof(Enums.CallClass), (int)Enums.CallClass.INV))));
                })
            });



            AggregateDefinitions.Add(new AggregateDefinition()
            {
                Name = "CountInInters",
                Aggregation = new CountAggregate((cdr) =>
                {
                    return (cdr.CallType == (int)Enums.CallType.incomingVoiceCall && (cdr.CallClass == Enum.GetName(typeof(Enums.CallClass), (int)Enums.CallClass.INTL)));
                })
            });



            AggregateDefinitions.Add(new AggregateDefinition()
            {
                Name = "CallOutDurs",
                Aggregation = new SumAggregate(
               (cdr) =>
               {
                   return cdr.DurationInSeconds / 60;
               },

               (cdr) =>
               {
                   return (cdr.DurationInSeconds != 0 && cdr.CallType == (int)Enums.CallType.outgoingVoiceCall);
               }
            )
            });


            AggregateDefinitions.Add(new AggregateDefinition()
            {
                Name = "TotalOutVolume",
                Aggregation = new SumAggregate(
                (cdr) =>
                {
                    return cdr.DurationInSeconds;
                },

                (cdr) =>
                {
                    return (cdr.CallType == (int)Enums.CallType.outgoingVoiceCall);
                }
            )
            });



            AggregateDefinitions.Add(new AggregateDefinition()
            {
                Name = "CallInDurs",
                Aggregation = new SumAggregate(
               (cdr) =>
               {
                   return cdr.DurationInSeconds / 60;
               },

               (cdr) =>
               {
                   return (cdr.DurationInSeconds != 0 && cdr.CallType == (int)Enums.CallType.incomingVoiceCall);
               }
            )
            });



            AggregateDefinitions.Add(new AggregateDefinition()
            {
                Name = "TotalInVolume",
                Aggregation = new SumAggregate(
                (cdr) =>
                {
                    return cdr.DurationInSeconds;
                },

                (cdr) =>
                {
                    return (cdr.CallType == (int)Enums.CallType.incomingVoiceCall);
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
                       return ((cdr.CallType == (int)Enums.CallType.incomingVoiceCall || cdr.CallType == (int)Enums.CallType.outgoingVoiceCall || cdr.CallType == (int)Enums.CallType.incomingSms || cdr.CallType == (int)Enums.CallType.outgoingSms));
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
                          return ((cdr.CallType == (int)Enums.CallType.incomingVoiceCall || cdr.CallType == (int)Enums.CallType.outgoingVoiceCall || cdr.CallType == (int)Enums.CallType.incomingSms || cdr.CallType == (int)Enums.CallType.outgoingSms));
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
                         return (cdr.CallType == (int)Enums.CallType.outgoingVoiceCall);
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
                         return (cdr.CallType == (int)Enums.CallType.incomingVoiceCall);
                     }
                 )
            });



            return AggregateDefinitions; 
        }

    }
}
