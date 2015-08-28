using System;
using System.Collections.Generic;
using Vanrise.Fzero.FraudAnalysis.Entities;
using System.Linq;
using Vanrise.Fzero.Business;
using Vanrise.Fzero.Entities;
using Vanrise.Fzero.CDRImport.Entities;

namespace Vanrise.Fzero.FraudAnalysis.Business
{
    public class AggregateManager
    {

        List<Strategy> _strategies;
        HashSet<int> _nightCallHours = new HashSet<int>() { 0, 1, 2, 3, 4, 5 };


        public AggregateManager(List<Strategy> strategies)
        {
            _strategies = strategies;
        }

        public AggregateManager()
        {

        }


        public List<AggregateDefinition> GetAggregateDefinitions(List<CallClass> callClasses)
        {
            Dictionary<string, NetTypeEnum> callClassNetTypes = new Dictionary<string, NetTypeEnum>();
            foreach (CallClass callClass in callClasses)
            {
                callClassNetTypes.Add(callClass.Description, callClass.NetType);
            }

            Func<CDR, NetTypeEnum, bool> IsMatchNetType = (cdr, compareToNetType) =>
            {
                NetTypeEnum netType;
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
                        return (cdr.CallType == CallTypeEnum.OutgoingVoiceCall);
                    })
                });


            AggregateDefinitions.Add(new AggregateDefinition()
            {
                Id = 2,
                Aggregation = new CountAggregate((cdr) =>
                {
                    return (cdr.CallType == CallTypeEnum.IncomingVoiceCall);
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
                    return (cdr.CallType == CallTypeEnum.OutgoingVoiceCall) && (cdr.DurationInSeconds == 0);
                })
            });


            AggregateDefinitions.Add(new AggregateDefinition()
            {
                Id = 5,
                Aggregation = new CountAggregate((cdr) =>
                {
                    return (cdr.CallType == CallTypeEnum.IncomingVoiceCall) && (cdr.DurationInSeconds == 0);
                })
            });



            AggregateDefinitions.Add(new AggregateDefinition()
            {
                Id = 6,
                Aggregation = new CountAggregate((cdr) =>
                {
                    return (cdr.CallType == CallTypeEnum.OutgoingSms);
                })
            });



            AggregateDefinitions.Add(new AggregateDefinition()
            {
                Id = 7,
                Aggregation = new CountAggregate((cdr) =>
                {
                    return ((cdr.CallType == CallTypeEnum.OutgoingVoiceCall) && (IsMatchNetType(cdr, NetTypeEnum.Others)));
                })
            });

            AggregateDefinitions.Add(new AggregateDefinition()
            {
                Id = 8,
                Aggregation = new CountAggregate((cdr) =>
                {
                    return ((cdr.CallType == CallTypeEnum.OutgoingVoiceCall) && (IsMatchNetType(cdr, NetTypeEnum.Local)));
                })
            });


            AggregateDefinitions.Add(new AggregateDefinition()
            {
                Id = 9,
                Aggregation = new CountAggregate((cdr) =>
                {
                    return ((cdr.CallType == CallTypeEnum.OutgoingVoiceCall) && (IsMatchNetType(cdr, NetTypeEnum.International)));
                })
            });


            AggregateDefinitions.Add(new AggregateDefinition()
            {
                Id = 10,
                Aggregation = new CountAggregate((cdr) =>
                {
                    return ((cdr.CallType == CallTypeEnum.IncomingVoiceCall) && (IsMatchNetType(cdr, NetTypeEnum.International)));
                })
            });



            AggregateDefinitions.Add(new AggregateDefinition()
            {
                Id = 11,
                Aggregation = new AverageAggregate(
                    (cdr) =>
                    {
                        return (cdr.CallType == CallTypeEnum.OutgoingVoiceCall && cdr.DurationInSeconds.HasValue && cdr.DurationInSeconds > 0);
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
                       return (cdr.CallType == CallTypeEnum.IncomingVoiceCall && cdr.DurationInSeconds.HasValue && cdr.DurationInSeconds > 0);
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
                    return (cdr.CallType == CallTypeEnum.OutgoingVoiceCall && cdr.DurationInSeconds.HasValue) ? cdr.DurationInSeconds.Value/60 : 0;
                }
            )
            });





            AggregateDefinitions.Add(new AggregateDefinition()
             {
                    Id = 14,
                Aggregation = new SumAggregate(
                (cdr) =>
                {
                    return (cdr.CallType == CallTypeEnum.IncomingVoiceCall && cdr.DurationInSeconds.HasValue) ? cdr.DurationInSeconds.Value : 0;
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
                       return (cdr.CallType == CallTypeEnum.IncomingVoiceCall || cdr.CallType == CallTypeEnum.OutgoingVoiceCall || cdr.CallType == CallTypeEnum.IncomingSms || cdr.CallType == CallTypeEnum.OutgoingSms);
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
                          return (cdr.BTSId.HasValue && (cdr.CallType == CallTypeEnum.IncomingVoiceCall || cdr.CallType == CallTypeEnum.OutgoingVoiceCall || cdr.CallType == CallTypeEnum.IncomingSms || cdr.CallType == CallTypeEnum.OutgoingSms));
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
                         return (cdr.CallType == CallTypeEnum.OutgoingVoiceCall);
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
                         return (cdr.CallType == CallTypeEnum.IncomingVoiceCall);
                     }
                 )
            });


            AggregateDefinitions.Add(new AggregateDefinition()
             {
                    Id = 19,
                Aggregation = new CountAggregate((cdr) =>
                {
                    return ((cdr.CallType == CallTypeEnum.IncomingVoiceCall) && (IsMatchNetType(cdr, NetTypeEnum.Others)));
                })
            });


            AggregateDefinitions.Add(new AggregateDefinition()
             {
                    Id = 20,
                Aggregation = new CountAggregate((cdr) =>
                {
                    return ((cdr.CallType == CallTypeEnum.IncomingVoiceCall) && (IsMatchNetType(cdr, NetTypeEnum.Local)));
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
                         return (cdr.CallType == CallTypeEnum.OutgoingVoiceCall && cdr.ConnectDateTime.HasValue && _nightCallHours.Contains(cdr.ConnectDateTime.Value.Hour));
                     }
                 )
            });


            AggregateDefinitions.Add(new AggregateDefinition()
             {
                    Id = 22,
                Aggregation = new CountAggregate((cdr, strategy) =>
                {
                    return (cdr.CallType == CallTypeEnum.OutgoingVoiceCall && cdr.ConnectDateTime.HasValue && strategy.PeakHoursIds.Contains(cdr.ConnectDateTime.Value.Hour));
                }, _strategies)
            });

           
            AggregateDefinitions.Add(new AggregateDefinition()
             {
                    Id = 23,
                Aggregation = new ConsecutiveAggregate(
                      (cdr, strategy) =>
                      {
                          return (cdr.CallType == CallTypeEnum.OutgoingVoiceCall && cdr.DurationInSeconds > 0);
                      }
                      , _strategies
                  )
            });




            AggregateDefinitions.Add(new AggregateDefinition()
             {
                    Id = 24,
                Aggregation = new GroupCountAggregate(
                      (cdr, strategy) =>
                      {
                          return (cdr.CallType == CallTypeEnum.OutgoingVoiceCall);
                      }
                      , _strategies
                  )
            });



            AggregateDefinitions.Add(new AggregateDefinition()
            {
                    Id = 25,
                Aggregation = new FailedConsecutiveAggregate(
                      (cdr, strategy) =>
                      {
                          return (cdr.CallType == CallTypeEnum.OutgoingVoiceCall && cdr.DurationInSeconds == 0);
                      }
                      , _strategies
                  )
            });


            AggregateDefinitions.Add(new AggregateDefinition()
            {
                    Id = 26,
                Aggregation = new CountAggregate((cdr, strategy) =>
                {
                    return (cdr.CallType == CallTypeEnum.IncomingVoiceCall && cdr.DurationInSeconds <= strategy.MaxLowDurationCall);
                }, _strategies)
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



            return AggregateDefinitions.Where(x => x.OperatorTypeAllowed == ConfigParameterManager.GetOperatorType() || x.OperatorTypeAllowed == CommonEnums.OperatorType.Mobile).ToList(); 
        }


        public List<AggregateDefinitionInfo> GetAggregateDefinitionsInfo()
        {


            List<AggregateDefinitionInfo> AggregateDefinitionsInfo = new List<AggregateDefinitionInfo>();


            AggregateDefinitionsInfo.Add(new AggregateDefinitionInfo()
            {
                Id = 1, 
                Name = "Count Out Calls", 
                OperatorTypeAllowed = Fzero.Entities.CommonEnums.OperatorType.Mobile
            });


            AggregateDefinitionsInfo.Add(new AggregateDefinitionInfo()
            {
                Id = 2,
                Name = "Count In Calls",
                OperatorTypeAllowed = Fzero.Entities.CommonEnums.OperatorType.Mobile
            });


            AggregateDefinitionsInfo.Add(new AggregateDefinitionInfo()
            {
                Id = 3,
                Name = "Total Data Volume",
                OperatorTypeAllowed = Fzero.Entities.CommonEnums.OperatorType.Mobile
            });


            AggregateDefinitionsInfo.Add(new AggregateDefinitionInfo()
            {
                Id = 4,
                Name = "Count Out Fails",
                OperatorTypeAllowed = Fzero.Entities.CommonEnums.OperatorType.Mobile
            });


            AggregateDefinitionsInfo.Add(new AggregateDefinitionInfo()
            {
                Id = 5,
                Name = "Count In Fails",
                OperatorTypeAllowed = Fzero.Entities.CommonEnums.OperatorType.Mobile
            });



            AggregateDefinitionsInfo.Add(new AggregateDefinitionInfo()
            {
                Id = 6,
                Name = "Count Out SMSs",
                OperatorTypeAllowed = Fzero.Entities.CommonEnums.OperatorType.Mobile
            });



            AggregateDefinitionsInfo.Add(new AggregateDefinitionInfo()
            {
                Id = 7,
                Name = "Count Out OffNets",
                OperatorTypeAllowed = Fzero.Entities.CommonEnums.OperatorType.Mobile
            });

            AggregateDefinitionsInfo.Add(new AggregateDefinitionInfo()
            {
                Id = 8,
                Name = "Count Out OnNets",
                OperatorTypeAllowed = Fzero.Entities.CommonEnums.OperatorType.Mobile
            });


            AggregateDefinitionsInfo.Add(new AggregateDefinitionInfo()
            {
                Id = 9,
                Name = "Count Out Inters",
                OperatorTypeAllowed = Fzero.Entities.CommonEnums.OperatorType.Mobile
            });


            AggregateDefinitionsInfo.Add(new AggregateDefinitionInfo()
            {
                Id = 10,
                Name = "Count In Inters",
                OperatorTypeAllowed = Fzero.Entities.CommonEnums.OperatorType.Mobile
            });



            AggregateDefinitionsInfo.Add(new AggregateDefinitionInfo()
            {
                Id = 11,
                Name = "Call Out Dur Avg",
                OperatorTypeAllowed = Fzero.Entities.CommonEnums.OperatorType.Mobile
            });

            AggregateDefinitionsInfo.Add(new AggregateDefinitionInfo()
            {
                Id = 12,
                Name = "Call In Dur Avg",
                OperatorTypeAllowed = Fzero.Entities.CommonEnums.OperatorType.Mobile
            });


            AggregateDefinitionsInfo.Add(new AggregateDefinitionInfo()
            {
                Id = 13,
                Name = "Total Out Volume",
                OperatorTypeAllowed = Fzero.Entities.CommonEnums.OperatorType.Mobile
            });





            AggregateDefinitionsInfo.Add(new AggregateDefinitionInfo()
            {
                Id = 14,
                Name = "Total In Volume",
                OperatorTypeAllowed = Fzero.Entities.CommonEnums.OperatorType.Mobile
            });

            AggregateDefinitionsInfo.Add(new AggregateDefinitionInfo()
            {
                Id = 15,
                Name = "Total IMEI",
                OperatorTypeAllowed = Fzero.Entities.CommonEnums.OperatorType.Mobile
            });


            AggregateDefinitionsInfo.Add(new AggregateDefinitionInfo()
            {
                Id = 16,
                Name = "Total BTS",
                OperatorTypeAllowed = Fzero.Entities.CommonEnums.OperatorType.Mobile
            });


            AggregateDefinitionsInfo.Add(new AggregateDefinitionInfo()
            {
                Id = 17,
                Name = "Diff Output Numbers",
                OperatorTypeAllowed = Fzero.Entities.CommonEnums.OperatorType.Mobile
            });


            AggregateDefinitionsInfo.Add(new AggregateDefinitionInfo()
            {
                Id = 18,
                Name = "Diff Input Numbers",
                OperatorTypeAllowed = Fzero.Entities.CommonEnums.OperatorType.Mobile
            });


            AggregateDefinitionsInfo.Add(new AggregateDefinitionInfo()
            {
                Id = 19,
                Name = "Count In OffNets",
                OperatorTypeAllowed = Fzero.Entities.CommonEnums.OperatorType.Mobile
            });


            AggregateDefinitionsInfo.Add(new AggregateDefinitionInfo()
            {
                Id = 20,
                Name = "Count In OnNets",
                OperatorTypeAllowed = Fzero.Entities.CommonEnums.OperatorType.Mobile
            });


            AggregateDefinitionsInfo.Add(new AggregateDefinitionInfo()
            {
                Id = 21,
                Name = "Diff Output Numbers NightCalls",
                OperatorTypeAllowed = Fzero.Entities.CommonEnums.OperatorType.Mobile
            });


            AggregateDefinitionsInfo.Add(new AggregateDefinitionInfo()
            {
                Id = 22,
                Name = "Count Out Calls Peak Hours",
                OperatorTypeAllowed = Fzero.Entities.CommonEnums.OperatorType.Mobile
            });


            AggregateDefinitionsInfo.Add(new AggregateDefinitionInfo()
            {
                Id = 23,
                Name = "Count Consecutive Calls",
                OperatorTypeAllowed = Fzero.Entities.CommonEnums.OperatorType.Mobile
            });




            AggregateDefinitionsInfo.Add(new AggregateDefinitionInfo()
            {
                Id = 24,
                Name = "Count Active Hours",
                OperatorTypeAllowed = Fzero.Entities.CommonEnums.OperatorType.Mobile
            });



            AggregateDefinitionsInfo.Add(new AggregateDefinitionInfo()
            {
                Id = 25,
                Name = "Count Fail Consecutive Calls",
                OperatorTypeAllowed = Fzero.Entities.CommonEnums.OperatorType.Mobile
            });


            AggregateDefinitionsInfo.Add(new AggregateDefinitionInfo()
            {
                Id = 26,
                Name = "Count In Low Duration Calls",
                OperatorTypeAllowed = Fzero.Entities.CommonEnums.OperatorType.Mobile
            });



            return AggregateDefinitionsInfo.Where(x => x.OperatorTypeAllowed == ConfigParameterManager.GetOperatorType() || x.OperatorTypeAllowed == CommonEnums.OperatorType.Mobile).ToList(); 
        }


    }
}
