using System;
using System.Collections.Generic;
using Vanrise.Fzero.FraudAnalysis.Entities;

namespace Vanrise.Fzero.FraudAnalysis.Business
{
    public class AggregateManager
    {

        List<Strategy> _strategies;
        HashSet<int> _nightCallHours = new HashSet<int>() { 0,1,2,3,4,5};
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


        public List<AggregateDefinition> GetAggregateDefinitions()
        {

            PredefinedManager predefinedDataManager = new PredefinedManager();
            List<CallClass> callClasses = predefinedDataManager.GetClasses();

            Dictionary<string, Enums.NetType> callClassNetTypes = new Dictionary<string, Enums.NetType>();
            foreach(CallClass callClass in callClasses)
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


            foreach ( var agg in GetAggregatesNames())
            {

                switch(agg.Name)
                {
                    case "CountOutCalls":
                        agg.Aggregation = new CountAggregate((cdr) =>
                            {
                                return (cdr.CallType == Enums.CallType.OutgoingVoiceCall);
                            });
                    break;



                case "CountInCalls":
                        agg.Aggregation = new CountAggregate((cdr) =>
                    {
                        return (cdr.CallType == Enums.CallType.IncomingVoiceCall);
                   });
                    break;


                case "TotalDataVolume":
                        agg.Aggregation = new SumAggregate((cdr) =>
                    {
                        return (cdr.UpVolume.HasValue ? cdr.UpVolume.Value : 0) + (cdr.DownVolume.HasValue ? cdr.DownVolume.Value : 0);
                   });
                    break;

                case "CountOutFails":
                        agg.Aggregation = new CountAggregate((cdr) =>
                    {
                        return (cdr.CallType == Enums.CallType.OutgoingVoiceCall) && (cdr.DurationInSeconds == 0);
                    });
                    break;


                case "CountInFails":
                        agg.Aggregation = new CountAggregate((cdr) =>
                    {
                        return (cdr.CallType == Enums.CallType.IncomingVoiceCall) && (cdr.DurationInSeconds == 0);
                   });
                    break;



                case "CountOutSMSs":
                        agg.Aggregation = new CountAggregate((cdr) =>
                    {
                        return (cdr.CallType == Enums.CallType.OutgoingSms);
                   });
                    break;



                case "CountOutOffNets":
                        agg.Aggregation = new CountAggregate((cdr) =>
                    {
                        return ((cdr.CallType == Enums.CallType.OutgoingVoiceCall) && (IsMatchNetType(cdr, Enums.NetType.Others)));
                    });
                    break;

                case "CountOutOnNets":
                        agg.Aggregation = new CountAggregate((cdr) =>
                    {
                        return ((cdr.CallType == Enums.CallType.OutgoingVoiceCall) && (IsMatchNetType(cdr, Enums.NetType.Local)));
                    });
                    break;


                case "CountOutInters":
                        agg.Aggregation = new CountAggregate((cdr) =>
                    {
                        return ((cdr.CallType == Enums.CallType.OutgoingVoiceCall) && (IsMatchNetType(cdr, Enums.NetType.International)));
                    });
                    break;


                case "CountInInters":
                        agg.Aggregation = new CountAggregate((cdr) =>
                    {
                        return ((cdr.CallType == Enums.CallType.IncomingVoiceCall) && (IsMatchNetType(cdr, Enums.NetType.International)));
                   });
                    break;


                case "CallOutDurAvg":
                        agg.Aggregation = new SumAggregate(
                   (cdr) =>
                   {
                       return (cdr.CallType == Enums.CallType.OutgoingVoiceCall && cdr.DurationInSeconds.HasValue) ? cdr.DurationInSeconds.Value / 60 : 0;
                   });
                    break;

                case "CallInDurAvg":
                        agg.Aggregation = new SumAggregate(
                   (cdr) =>
                   {
                       return (cdr.CallType == Enums.CallType.IncomingVoiceCall && cdr.DurationInSeconds.HasValue) ? cdr.DurationInSeconds.Value / 60 : 0;
                   });
                    break;


                case "TotalOutVolume":
                        agg.Aggregation = new SumAggregate(
                    (cdr) =>
                    {
                        return (cdr.CallType == Enums.CallType.OutgoingVoiceCall && cdr.DurationInSeconds.HasValue) ? cdr.DurationInSeconds.Value : 0;
                    });
                    break;





                case "TotalInVolume":
                        agg.Aggregation = new SumAggregate(
                    (cdr) =>
                    {
                        return (cdr.CallType == Enums.CallType.IncomingVoiceCall && cdr.DurationInSeconds.HasValue) ? cdr.DurationInSeconds.Value : 0;
                    });
                    break;

                case "TotalIMEI":
                        agg.Aggregation = new DistinctCountAggregate(
                       (cdr) =>
                       {
                           return cdr.IMEI;
                       },

                       (cdr) =>
                       {
                           return (cdr.CallType == Enums.CallType.IncomingVoiceCall || cdr.CallType == Enums.CallType.OutgoingVoiceCall || cdr.CallType == Enums.CallType.IncomingSms || cdr.CallType == Enums.CallType.OutgoingSms);
                       });
                    break;


                case "TotalBTS":
                        agg.Aggregation = new DistinctCountAggregate(
                          (cdr) =>
                          {
                              return cdr.BTSId;
                          },

                          (cdr) =>
                          {
                              return (cdr.BTSId.HasValue && (cdr.CallType == Enums.CallType.IncomingVoiceCall || cdr.CallType == Enums.CallType.OutgoingVoiceCall || cdr.CallType == Enums.CallType.IncomingSms || cdr.CallType == Enums.CallType.OutgoingSms));
                         });
                    break;


                case "DiffOutputNumb":
                        agg.Aggregation = new DistinctCountAggregate(
                         (cdr) =>
                         {
                             return cdr.Destination;
                         },

                         (cdr) =>
                         {
                             return (cdr.CallType == Enums.CallType.OutgoingVoiceCall);
                         });
                    break;


                case "DiffInputNumbers":
                        agg.Aggregation = new DistinctCountAggregate(
                         (cdr) =>
                         {
                             return cdr.Destination;
                         },

                         (cdr) =>
                         {
                             return (cdr.CallType == Enums.CallType.IncomingVoiceCall);
                         });
                    break;


                case "CountInOffNets":
                        agg.Aggregation = new CountAggregate((cdr) =>
                    {
                        return ((cdr.CallType == Enums.CallType.IncomingVoiceCall) && (IsMatchNetType(cdr, Enums.NetType.Others)));
                    });
                    break;


                case "CountInOnNets":
                        agg.Aggregation = new CountAggregate((cdr) =>
                    {
                        return ((cdr.CallType == Enums.CallType.IncomingVoiceCall) && (IsMatchNetType(cdr, Enums.NetType.Local)));
                   });
                    break;


                case "DiffOutputNumbNightCalls":
                        agg.Aggregation = new DistinctCountAggregate(
                         (cdr) =>
                         {
                             return cdr.Destination;
                         },

                         (cdr) =>
                         {
                             return (cdr.CallType == Enums.CallType.OutgoingVoiceCall && cdr.ConnectDateTime.HasValue && _nightCallHours.Contains(cdr.ConnectDateTime.Value.Hour));
                         });
                    break;


                case "CountOutCallsPeakHours":
                        agg.Aggregation = new CountAggregate((cdr, strategy) =>
                    {
                        return (cdr.CallType == Enums.CallType.OutgoingVoiceCall && cdr.ConnectDateTime.HasValue && strategy.PeakHoursIds.Contains(cdr.ConnectDateTime.Value.Hour));
                    }, _strategies);
              
                    break;


                case "CountConsecutiveCalls":
                        agg.Aggregation = new ConsecutiveAggregate(
                          (cdr, strategy) =>
                          {
                              return (cdr.CallType == Enums.CallType.OutgoingVoiceCall);
                          }
                          , _strategies);
                    break;




                case "CountActiveHours":
                        agg.Aggregation = new GroupCountAggregate(
                          (cdr, strategy) =>
                          {
                              return (cdr.CallType == Enums.CallType.OutgoingVoiceCall);
                          }
                          , _strategies
                      );
                    break;



                case "CountFailConsecutiveCalls":
                        agg.Aggregation = new ConsecutiveAggregate(
                          (cdr, strategy) =>
                          {
                              return (cdr.CallType == Enums.CallType.OutgoingVoiceCall && cdr.DurationInSeconds == 0);
                          }
                          , _strategies
                      );
                    break;


                case "CountInLowDurationCalls":
                        agg.Aggregation = new CountAggregate((cdr, strategy) =>
                    {
                        return (cdr.CallType == Enums.CallType.IncomingVoiceCall && cdr.DurationInSeconds <= strategy.MaxLowDurationCall);
                    }, _strategies);
                
                    break;
                }
               
            }


           

            return AggregateDefinitions; 
        }

        public List<AggregateDefinition> GetAggregatesNames()
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
                AggregateId = 11,
                Name = "CallOutDurAvg"
            });

            AggregateDefinitions.Add(new AggregateDefinition()
            {
                AggregateId = 12,
                Name = "CallInDurAvg"
            });


            AggregateDefinitions.Add(new AggregateDefinition()
            {
                AggregateId = 12,
                Name = "TotalOutVolume"
            });

            AggregateDefinitions.Add(new AggregateDefinition()
            {
                AggregateId = 14,
                Name = "TotalInVolume"
            });

            AggregateDefinitions.Add(new AggregateDefinition()
            {
                AggregateId = 15,
                Name = "TotalIMEI"
            });


            AggregateDefinitions.Add(new AggregateDefinition()
            {
                AggregateId = 16,
                Name = "TotalBTS"
            });


            AggregateDefinitions.Add(new AggregateDefinition()
            {
                AggregateId = 17,
                Name = "DiffOutputNumb"
            });


            AggregateDefinitions.Add(new AggregateDefinition()
            {
                AggregateId = 18,
                Name = "DiffInputNumbers"
            });


            AggregateDefinitions.Add(new AggregateDefinition()
            {
                AggregateId = 19,
                Name = "CountInOffNets"
            });


            AggregateDefinitions.Add(new AggregateDefinition()
            {
                AggregateId = 20,
                Name = "CountInOnNets"
            });


            AggregateDefinitions.Add(new AggregateDefinition()
            {
                AggregateId = 21,
                Name = "DiffOutputNumbNightCalls"
            });


            AggregateDefinitions.Add(new AggregateDefinition()
            {
                AggregateId =22,
                Name = "CountOutCallsPeakHours"
            });


            AggregateDefinitions.Add(new AggregateDefinition()
            {
                AggregateId =23,
                Name = "CountConsecutiveCalls"
            });




            AggregateDefinitions.Add(new AggregateDefinition()
            {
                AggregateId =24,
                Name = "CountActiveHours"
            });



            AggregateDefinitions.Add(new AggregateDefinition()
            {
                AggregateId = 25,
                Name = "CountFailConsecutiveCalls"
            });


            AggregateDefinitions.Add(new AggregateDefinition()
            {
                AggregateId = 26,
                Name = "CountInLowDurationCalls"
            });

            return AggregateDefinitions;
        }

    }
}
