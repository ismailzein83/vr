using System;
using System.Activities;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Vanrise.BusinessProcess;
using Vanrise.Fzero.FraudAnalysis.Data;
using Vanrise.Fzero.FraudAnalysis.Entities;
using Vanrise.Queueing;

namespace Vanrise.Fzero.FraudAnalysis.BP.Activities
{

    #region Arguments Classes

    public class LoadNumberProfilesInput
    {
        public BaseQueue<NumberProfileBatch> OutputQueue { get; set; }

    }

    #endregion

    public class LoadNumberProfiles : BaseAsyncActivity<LoadNumberProfilesInput>
    {

        #region Arguments

        [RequiredArgument]
        public  InOutArgument<BaseQueue<NumberProfileBatch>> OutputQueue { get; set; }

        #endregion

        protected override void OnBeforeExecute(AsyncCodeActivityContext context, AsyncActivityHandle handle)
        {
            if (this.OutputQueue.Get(context) == null)
                this.OutputQueue.Set(context, new MemoryQueue<NumberProfileBatch>());
            base.OnBeforeExecute(context, handle);
        }

        //protected override void DoWork(LoadNumberProfilesInput inputArgument, AsyncActivityHandle handle)
        //{
        //   int? BatchSize = int.Parse( System.Configuration.ConfigurationManager.AppSettings["NumberProfileBatchSize"].ToString());
        //   handle.SharedInstanceData.WriteTrackingMessage(BusinessProcess.Entities.BPTrackingSeverity.Information, "LoadNumberProfiles.DoWork.Started ");


        //   List<NumberProfile> numberProfileBatch = new List<NumberProfile>();
        //   NumberProfile numberProfile = new NumberProfile();
        //   HashSet<string> DestinationsIn = new HashSet<string>();
        //   HashSet<string> DestinationsOut = new HashSet<string>();

        //   HashSet<string> MSISDNsIn = new HashSet<string>();
        //   HashSet<string> MSISDNsOut = new HashSet<string>();

        //   HashSet<int> BTSIds = new HashSet<int>();
        //   HashSet<string> IMEIs = new HashSet<string>();
        //   HashSet<decimal> callOutDurs = new HashSet<decimal>();
        //   HashSet<decimal> callInDurs = new HashSet<decimal>();
        //   int countOutCalls = 0;
        //   int countInCalls = 0;
        //   int countOutFails = 0;
        //   int countInFails = 0;
        //   int countInOffNets = 0;
        //   int countOutOffNets = 0;
        //   int countInOnNets = 0;
        //   int countOutOnNets = 0;
        //   int countInInters = 0;
        //   int countOutInters = 0;
        //   int countOutSMSs = 0;
        //   decimal totalDataVolume = 0;
        //   string _mSISDN = string.Empty;


        //   INumberProfileDataManager dataManager = FraudDataManagerFactory.GetDataManager<INumberProfileDataManager>();
        //   dataManager.LoadCDR(DateTime.Parse("2010-03-10 04:00:00"), DateTime.Parse("2019-03-20 06:00:00"), BatchSize, (normalCDR) =>
        //   {

               

        //       // CDR Facts
               
        //       string _destination = string.Empty;
        //       int _callType = 0;
        //       int _bTSId = 0;
        //       int _id = 0;
        //       string _iMSI = string.Empty;
        //       decimal _durationInSeconds = 0;
        //       DateTime _disconnectDateTime = new DateTime();
        //       string _callClass = string.Empty;
        //       short _isOnNet = 0;
        //       string _subType = string.Empty;
        //       string _iMEI = string.Empty;
        //       string _cellId = string.Empty;
        //       int _switchRecordId = 0;
        //       decimal _upVolume = 0;
        //       decimal _downVolume = 0;
        //       decimal _cellLatitude = 0;
        //       decimal _cellLongitude = 0;
        //       string _inTrunk = string.Empty;
        //       string _outTrunk = string.Empty;
        //       int _serviceType = 0;
        //       string _serviceVASName = string.Empty;
        //       DateTime _connectDateTime = new DateTime();




        //       // Agregates
              

             

        //           _callType = normalCDR.callType;
        //           _bTSId = normalCDR.bTSId;
        //           _connectDateTime = normalCDR.connectDateTime;
        //           _id = normalCDR.id;
        //           _iMSI = normalCDR.iMSI;
        //           _durationInSeconds = normalCDR.durationInSeconds;
        //           _disconnectDateTime = normalCDR.disconnectDateTime;
        //           _callClass = normalCDR.callClass;
        //           _isOnNet = normalCDR.isOnNet;
        //           _subType = normalCDR.subType;
        //           _iMEI = normalCDR.iMEI;
        //           _cellId = normalCDR.cellId;
        //           _switchRecordId = normalCDR.switchRecordId;
        //           _upVolume = normalCDR.upVolume;
        //           _downVolume = normalCDR.downVolume;
        //           _cellLatitude = normalCDR.cellLatitude;
        //           _cellLongitude = normalCDR.cellLongitude;
        //           _inTrunk = normalCDR.inTrunk;
        //           _outTrunk = normalCDR.outTrunk;
        //           _serviceType = normalCDR.serviceType;
        //           _serviceVASName = normalCDR.serviceVASName;
        //           _destination = normalCDR.destination;




        //           if (_mSISDN == string.Empty)
        //           {
        //               numberProfile = new NumberProfile();
        //               _mSISDN = normalCDR.mSISDN;
        //           }

        //           else if (_mSISDN != normalCDR.mSISDN)
        //           {
        //               numberProfileBatch.Add(numberProfile);
        //               if (BatchSize.HasValue && numberProfileBatch.Count == BatchSize)
        //               {
        //                   inputArgument.OutputQueue.Enqueue(new NumberProfileBatch()
        //                   {
        //                       numberProfiles = numberProfileBatch
        //                   });
        //                   handle.SharedInstanceData.WriteTrackingMessage(BusinessProcess.Entities.BPTrackingSeverity.Information, "LoadNumberProfiles.DoWork.Enqueued Count Items: {0} ", numberProfileBatch.Count);
        //                   numberProfileBatch = new List<NumberProfile>();
        //               }

        //               numberProfile = new NumberProfile();
        //               DestinationsIn = new HashSet<string>();
        //               DestinationsOut = new HashSet<string>();
        //               MSISDNsIn = new HashSet<string>();
        //               MSISDNsOut = new HashSet<string>();
        //               BTSIds = new HashSet<int>();
        //               IMEIs = new HashSet<string>();
        //               callOutDurs = new HashSet<decimal>();
        //               callInDurs = new HashSet<decimal>();
        //               countOutCalls = 0;
        //               countInCalls = 0;
        //               countOutFails = 0;
        //               countInFails = 0;
        //               countInOffNets = 0;
        //               countOutOffNets = 0;
        //               countInOnNets = 0;
        //               countOutOnNets = 0;
        //               countInInters = 0;
        //               countOutInters = 0;
        //               countOutSMSs = 0;
        //               totalDataVolume = 0;

        //               _mSISDN = normalCDR.mSISDN;
        //           }


        //           numberProfile.subscriberNumber = _mSISDN;

               

        

        //           // Filling Agregates

        //           numberProfile.periodId = (int)Enums.Period.Day;
        //           numberProfile.fromDate = _connectDateTime;
        //           numberProfile.isOnNet = 1;

        //           totalDataVolume += (_upVolume + _downVolume);
        //           numberProfile.totalDataVolume = totalDataVolume;




        //           if ((int)Enums.Period.Day == (int)Enums.Period.Day)
        //           {
        //               numberProfile.toDate = _connectDateTime.AddDays(1);
        //           }


        //           if (_callType == (int)Enums.CallType.outgoingVoiceCall)
        //           {
        //               numberProfile.countOutCalls = ++countOutCalls;


        //               if (!DestinationsOut.Contains(_destination))
        //               {
        //                   DestinationsOut.Add(_destination);
        //                   numberProfile.diffOutputNumb = DestinationsOut.Count();
        //               }

        //               if (!DestinationsOut.Contains(_destination))
        //               {
        //                   DestinationsOut.Add(_destination);
        //                   numberProfile.diffOutputNumb = DestinationsOut.Count();
        //               }



        //               if (_durationInSeconds == 0)
        //                   numberProfile.countOutFail = ++countOutFails;
        //               else
        //               {
        //                   callOutDurs.Add(_durationInSeconds / 60);
        //                   numberProfile.callOutDurAvg = callOutDurs.Average();
        //                   numberProfile.totalInVolume = callOutDurs.Sum();
        //               }

        //               if ((_callClass == Enum.GetName(typeof(Enums.CallClass), (int)Enums.CallClass.ASIACELL) || _callClass == Enum.GetName(typeof(Enums.CallClass), (int)Enums.CallClass.KOREKTEL)))
        //                   numberProfile.countOutOffNet = ++countOutOffNets;
        //               else if ((_callClass == Enum.GetName(typeof(Enums.CallClass), (int)Enums.CallClass.ZAINIQ) || _callClass == Enum.GetName(typeof(Enums.CallClass), (int)Enums.CallClass.VAS) || _callClass == Enum.GetName(typeof(Enums.CallClass), (int)Enums.CallClass.INV)))
        //                   numberProfile.countOutOnNet = ++countOutOnNets;
        //               else if ((_callClass == Enum.GetName(typeof(Enums.CallClass), (int)Enums.CallClass.INTL)))
        //                   numberProfile.countOutInter = ++countOutInters;
        //           }


        //           if (_callType == (int)Enums.CallType.incomingVoiceCall)
        //           {
        //               numberProfile.countOutCalls = ++countInCalls;

        //               if (!DestinationsIn.Contains(_destination))
        //               {
        //                   DestinationsIn.Add(_destination);
        //                   numberProfile.diffInputNumbers = DestinationsIn.Count();
        //               }


        //               if (_durationInSeconds == 0)
        //                   numberProfile.countInFail = ++countInFails;
        //               else
        //               {
        //                   callInDurs.Add(_durationInSeconds / 60);
        //                   numberProfile.callInDurAvg = callInDurs.Average();
        //                   numberProfile.totalInVolume = callInDurs.Sum();
        //               }

        //               if ((_callClass == Enum.GetName(typeof(Enums.CallClass), (int)Enums.CallClass.ASIACELL) || _callClass == Enum.GetName(typeof(Enums.CallClass), (int)Enums.CallClass.KOREKTEL)))
        //                   numberProfile.countInOffNet = ++countInOffNets;
        //               else if ((_callClass == Enum.GetName(typeof(Enums.CallClass), (int)Enums.CallClass.ZAINIQ) || _callClass == Enum.GetName(typeof(Enums.CallClass), (int)Enums.CallClass.VAS) || _callClass == Enum.GetName(typeof(Enums.CallClass), (int)Enums.CallClass.INV)))
        //                   numberProfile.countInOnNet = ++countInOnNets;
        //               else if ((_callClass == Enum.GetName(typeof(Enums.CallClass), (int)Enums.CallClass.INTL)))
        //                   numberProfile.countInInter = ++countInInters;
        //           }

        //           else if (_callType == (int)Enums.CallType.outgoingSms)
        //           {
        //               numberProfile.countOutSMS = ++countOutSMSs;
        //           }





        //           if ((_callType == (int)Enums.CallType.incomingVoiceCall || _callType == (int)Enums.CallType.outgoingVoiceCall || _callType == (int)Enums.CallType.incomingSms || _callType == (int)Enums.CallType.outgoingSms))
        //           {
        //               if (!IMEIs.Contains(_iMEI))
        //                   IMEIs.Add(_iMEI);
        //               numberProfile.totalIMEI = IMEIs.Count();



        //               if (!BTSIds.Contains(_bTSId))
        //                   BTSIds.Add(_bTSId);
        //               numberProfile.totalBTS = BTSIds.Count();
        //           }

             


        //   });

        //   if (numberProfileBatch.Count > 0)
        //   {
        //       inputArgument.OutputQueue.Enqueue(new NumberProfileBatch()
        //       {
        //           numberProfiles = numberProfileBatch


        //       });
        //       handle.SharedInstanceData.WriteTrackingMessage(BusinessProcess.Entities.BPTrackingSeverity.Information, "LoadNumberProfiles.DoWork.Enqueued Count Items: {0} ", numberProfileBatch.Count);
        //   }

        //   handle.SharedInstanceData.WriteTrackingMessage(BusinessProcess.Entities.BPTrackingSeverity.Information, "LoadNumberProfiles.DoWork.Ended");
          
        //}

        protected override void DoWork(LoadNumberProfilesInput inputArgument, AsyncActivityHandle handle)
        {

            int? BatchSize = int.Parse(System.Configuration.ConfigurationManager.AppSettings["NumberProfileBatchSize"].ToString());
            handle.SharedInstanceData.WriteTrackingMessage(BusinessProcess.Entities.BPTrackingSeverity.Information, "LoadNumberProfiles.DoWork.Started ");


            List<NumberProfile> numberProfileBatch = new List<NumberProfile>();


            IAggregate countOutCalls = new CountAggregate((cdr) =>
            {
                return (cdr.callType == (int)Enums.CallType.outgoingVoiceCall);
            });


            IAggregate countInCalls = new CountAggregate((cdr) =>
            {
                return (cdr.callType == (int)Enums.CallType.incomingVoiceCall);
            });


            IAggregate totalDataVolume = new SumAggregate((cdr) =>
            {
                return cdr.upVolume + cdr.downVolume;
            }, null);


            IAggregate countOutFails = new CountAggregate((cdr) =>
            {
                return (cdr.callType == (int)Enums.CallType.outgoingVoiceCall)  && (cdr.durationInSeconds==0) ;
            });


            IAggregate countInFails = new CountAggregate((cdr) =>
            {
                return (cdr.callType == (int)Enums.CallType.incomingVoiceCall) && (cdr.durationInSeconds == 0);
            });


            IAggregate countOutSMSs = new CountAggregate((cdr) =>
            {
                return (cdr.callType == (int)Enums.CallType.outgoingSms) ;
            });


            IAggregate countOutOffNets = new CountAggregate((cdr) =>
            {
                 return (cdr.callType == (int)Enums.CallType.outgoingVoiceCall && (cdr.callClass == Enum.GetName(typeof(Enums.CallClass), (int)Enums.CallClass.ASIACELL) || cdr.callClass == Enum.GetName(typeof(Enums.CallClass), (int)Enums.CallClass.KOREKTEL)));
            });


            IAggregate countOutOnNets = new CountAggregate((cdr) =>
            {
            return (cdr.callType == (int)Enums.CallType.outgoingVoiceCall && ((cdr.callClass == Enum.GetName(typeof(Enums.CallClass), (int)Enums.CallClass.ZAINIQ) || cdr.callClass == Enum.GetName(typeof(Enums.CallClass), (int)Enums.CallClass.VAS) || cdr.callClass == Enum.GetName(typeof(Enums.CallClass), (int)Enums.CallClass.INV))));
            });


            IAggregate countOutInters = new CountAggregate((cdr) =>
            {
            return (cdr.callType == (int)Enums.CallType.outgoingVoiceCall && (cdr.callClass == Enum.GetName(typeof(Enums.CallClass), (int)Enums.CallClass.INTL)));
            });


             IAggregate countInOffNets = new CountAggregate((cdr) =>
            {
                 return (cdr.callType == (int)Enums.CallType.incomingVoiceCall && (cdr.callClass == Enum.GetName(typeof(Enums.CallClass), (int)Enums.CallClass.ASIACELL) || cdr.callClass == Enum.GetName(typeof(Enums.CallClass), (int)Enums.CallClass.KOREKTEL)));
            });


            IAggregate countInOnNets = new CountAggregate((cdr) =>
            {
            return (cdr.callType == (int)Enums.CallType.incomingVoiceCall && ((cdr.callClass == Enum.GetName(typeof(Enums.CallClass), (int)Enums.CallClass.ZAINIQ) || cdr.callClass == Enum.GetName(typeof(Enums.CallClass), (int)Enums.CallClass.VAS) || cdr.callClass == Enum.GetName(typeof(Enums.CallClass), (int)Enums.CallClass.INV))));
            });


            IAggregate countInInters = new CountAggregate((cdr) =>
            {
            return (cdr.callType == (int)Enums.CallType.incomingVoiceCall && (cdr.callClass == Enum.GetName(typeof(Enums.CallClass), (int)Enums.CallClass.INTL)));
            });



            IAggregate callOutDurs = new SumAggregate(
               (cdr) =>
                {
                    return cdr.durationInSeconds / 60;
                },

               (cdr) =>
                {
                   return ( cdr.durationInSeconds!=0  && cdr.callType == (int)Enums.CallType.outgoingVoiceCall);
                }
            );



            IAggregate totalOutVolume = new SumAggregate(
                (cdr) =>
                    {
                        return cdr.durationInSeconds;
                    },

                (cdr) =>
                     {
                         return (cdr.callType == (int)Enums.CallType.outgoingVoiceCall);
                     }
            );





            IAggregate callInDurs = new SumAggregate(
               (cdr) =>
               {
                   return cdr.durationInSeconds / 60;
               },

               (cdr) =>
               {
                   return (cdr.durationInSeconds != 0 && cdr.callType == (int)Enums.CallType.incomingVoiceCall);
               }
            );



            IAggregate totalInVolume = new SumAggregate(
                (cdr) =>
                {
                    return cdr.durationInSeconds;
                },

                (cdr) =>
                {
                    return (cdr.callType == (int)Enums.CallType.incomingVoiceCall);
                }
            );



           
            NumberProfile numberProfile = new NumberProfile();
            HashSet<string> DestinationsIn = new HashSet<string>();
            HashSet<string> DestinationsOut = new HashSet<string>();

            HashSet<int> BTSIds = new HashSet<int>();
            HashSet<string> IMEIs = new HashSet<string>();
            
           
         
            string _mSISDN = string.Empty;


            INumberProfileDataManager dataManager = FraudDataManagerFactory.GetDataManager<INumberProfileDataManager>();
            dataManager.LoadCDR(DateTime.Parse("2010-03-10 04:00:00"), DateTime.Parse("2019-03-20 06:00:00"), BatchSize, (normalCDR) =>
            {
                

                // Agregates

                if (_mSISDN == string.Empty)
                {
                    numberProfile = new NumberProfile();
                    countInCalls.Reset();
                    countOutCalls.Reset();
                    totalDataVolume.Reset();
                    countOutFails.Reset();
                    countInFails.Reset();
                    countOutSMSs.Reset();
                    countInOffNets.Reset();
                    countOutOffNets.Reset();
                    countInOnNets.Reset();
                    countOutOnNets.Reset();
                    countInInters.Reset();
                    countOutInters.Reset();
                    countOutSMSs.Reset();
                    totalOutVolume.Reset();
                    totalInVolume.Reset();
                    callOutDurs.Reset();
                    callInDurs.Reset();


                    _mSISDN = normalCDR.mSISDN;
                }

                else if (_mSISDN != normalCDR.mSISDN)
                {
                    numberProfileBatch.Add(numberProfile);
                    if (BatchSize.HasValue && numberProfileBatch.Count == BatchSize)
                    {
                        inputArgument.OutputQueue.Enqueue(new NumberProfileBatch()
                        {
                            numberProfiles = numberProfileBatch
                        });
                        handle.SharedInstanceData.WriteTrackingMessage(BusinessProcess.Entities.BPTrackingSeverity.Information, "LoadNumberProfiles.DoWork.Enqueued Count Items: {0} ", numberProfileBatch.Count);
                        numberProfileBatch = new List<NumberProfile>();
                    }

                    numberProfile = new NumberProfile();
                    countInCalls.Reset();
                    countOutCalls.Reset();
                    totalDataVolume.Reset();
                    countOutFails.Reset();
                    countInFails.Reset();
                    countOutSMSs.Reset();
                    countInOffNets.Reset();
                    countOutOffNets.Reset();
                    countInOnNets.Reset();
                    countOutOnNets.Reset();
                    countInInters.Reset();
                    countOutInters.Reset();
                    countOutSMSs.Reset();
                    totalOutVolume.Reset();
                    totalInVolume.Reset();
                    callOutDurs.Reset();
                    callInDurs.Reset();

                    DestinationsIn = new HashSet<string>();
                    DestinationsOut = new HashSet<string>();
                    BTSIds = new HashSet<int>();
                    IMEIs = new HashSet<string>();
                    

                    _mSISDN = normalCDR.mSISDN;
                }


                numberProfile.subscriberNumber = _mSISDN;









                // New Filling Aggregates

                //IAggregate DistinctCountAggregate = new DistinctCountAggregate("", "");


                if ((int)Enums.Period.Day == (int)Enums.Period.Day)
                {
                    numberProfile.toDate = normalCDR.connectDateTime.AddDays(1);
                }

                numberProfile.periodId = (int)Enums.Period.Day;
                numberProfile.fromDate = normalCDR.connectDateTime;
                numberProfile.isOnNet = 1;

                countInCalls.EvaluateCDR(normalCDR);
                numberProfile.countInCalls = int.Parse(countInCalls.GetResult().ToString());


                countOutCalls.EvaluateCDR(normalCDR);
                numberProfile.countOutCalls = int.Parse(countOutCalls.GetResult().ToString());


                countInOffNets.EvaluateCDR(normalCDR);
                numberProfile.countInOffNet = int.Parse(countInOffNets.GetResult().ToString());


                countOutOffNets.EvaluateCDR(normalCDR);
                numberProfile.countOutOffNet = int.Parse(countOutOffNets.GetResult().ToString());


                countInOnNets.EvaluateCDR(normalCDR);
                numberProfile.countInOnNet = int.Parse(countInOnNets.GetResult().ToString());


                countOutOnNets.EvaluateCDR(normalCDR);
                numberProfile.countOutOnNet = int.Parse(countOutOnNets.GetResult().ToString());


                countInInters.EvaluateCDR(normalCDR);
                numberProfile.countInInter = int.Parse(countInInters.GetResult().ToString());


                countOutInters.EvaluateCDR(normalCDR);
                numberProfile.countOutInter = int.Parse(countOutInters.GetResult().ToString());


                countOutSMSs.EvaluateCDR(normalCDR);
                numberProfile.countOutSMS = int.Parse(countOutSMSs.GetResult().ToString());

                totalOutVolume.EvaluateCDR(normalCDR);
                numberProfile.totalOutVolume = int.Parse(totalOutVolume.GetResult().ToString());


                totalInVolume.EvaluateCDR(normalCDR);
                numberProfile.totalInVolume = int.Parse(totalInVolume.GetResult().ToString());


                callOutDurs.EvaluateCDR(normalCDR);
                numberProfile.callOutDurAvg = int.Parse(callOutDurs.GetResult().ToString());


                callInDurs.EvaluateCDR(normalCDR);
                numberProfile.callInDurAvg = int.Parse(callInDurs.GetResult().ToString());







                // Filling Agregates


                if (normalCDR.callType == (int)Enums.CallType.outgoingVoiceCall)
                {
                    if (!DestinationsOut.Contains(normalCDR.destination))
                    {
                        DestinationsOut.Add(normalCDR.destination);
                        numberProfile.diffOutputNumb = DestinationsOut.Count();
                    }
                  
                }


                if (normalCDR.callType == (int)Enums.CallType.incomingVoiceCall)
                {

                    if (!DestinationsIn.Contains(normalCDR.destination))
                    {
                        DestinationsIn.Add(normalCDR.destination);
                        numberProfile.diffInputNumbers = DestinationsIn.Count();
                    }
                 
                }




                if ((normalCDR.callType == (int)Enums.CallType.incomingVoiceCall || normalCDR.callType == (int)Enums.CallType.outgoingVoiceCall || normalCDR.callType == (int)Enums.CallType.incomingSms || normalCDR.callType == (int)Enums.CallType.outgoingSms))
                {
                    if (!IMEIs.Contains(normalCDR.iMEI))
                        IMEIs.Add(normalCDR.iMEI);
                    numberProfile.totalIMEI = IMEIs.Count();



                    if (!BTSIds.Contains(normalCDR.bTSId))
                        BTSIds.Add(normalCDR.bTSId);
                    numberProfile.totalBTS = BTSIds.Count();
                }




            });

            if (numberProfileBatch.Count > 0)
            {
                inputArgument.OutputQueue.Enqueue(new NumberProfileBatch()
                {
                    numberProfiles = numberProfileBatch


                });
                handle.SharedInstanceData.WriteTrackingMessage(BusinessProcess.Entities.BPTrackingSeverity.Information, "LoadNumberProfiles.DoWork.Enqueued Count Items: {0} ", numberProfileBatch.Count);
            }

            handle.SharedInstanceData.WriteTrackingMessage(BusinessProcess.Entities.BPTrackingSeverity.Information, "LoadNumberProfiles.DoWork.Ended");

        }

        protected override LoadNumberProfilesInput GetInputArgument(System.Activities.AsyncCodeActivityContext context)
        {
            return new LoadNumberProfilesInput
            {
                OutputQueue = this.OutputQueue.Get(context)
            };
        }

    }
}
