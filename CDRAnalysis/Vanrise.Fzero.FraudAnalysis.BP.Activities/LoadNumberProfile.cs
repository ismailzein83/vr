using System;
using System.Activities;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
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

        protected override void DoWork(LoadNumberProfilesInput inputArgument, AsyncActivityHandle handle)
        {
           int? BatchSize = int.Parse( System.Configuration.ConfigurationManager.AppSettings["NumberProfileBatchSize"].ToString());
           handle.SharedInstanceData.WriteTrackingMessage(BusinessProcess.Entities.BPTrackingSeverity.Information, "LoadNumberProfiles.DoWork.Started ");

           INumberProfileDataManager dataManager = FraudDataManagerFactory.GetDataManager<INumberProfileDataManager>();
           dataManager.LoadCDR(DateTime.Parse("2010-03-10 04:00:00"), DateTime.Parse("2019-03-20 06:00:00"), BatchSize, (normalCDR) =>
           {

               List<NumberProfile> numberProfileBatch = new List<NumberProfile>();

               // CDR Facts
               string _mSISDN = string.Empty;
               string _destination = string.Empty;
               int _callType = 0;
               int _bTSId = 0;
               int _id = 0;
               string _iMSI = string.Empty;
               decimal _durationInSeconds = 0;
               DateTime _disconnectDateTime = new DateTime();
               string _callClass = string.Empty;
               short _isOnNet = 0;
               string _subType = string.Empty;
               string _iMEI = string.Empty;
               string _cellId = string.Empty;
               int _switchRecordId = 0;
               decimal _upVolume = 0;
               decimal _downVolume = 0;
               decimal _cellLatitude = 0;
               decimal _cellLongitude = 0;
               string _inTrunk = string.Empty;
               string _outTrunk = string.Empty;
               int _serviceType = 0;
               string _serviceVASName = string.Empty;
               DateTime _connectDateTime = new DateTime();




               // Agregates
               NumberProfile numberProfie = new NumberProfile();
               HashSet<string> DestinationsIn = new HashSet<string>();
               HashSet<string> DestinationsOut = new HashSet<string>();

               HashSet<string> MSISDNsIn = new HashSet<string>();
               HashSet<string> MSISDNsOut = new HashSet<string>();

               HashSet<int> BTSIds = new HashSet<int>();
               HashSet<string> IMEIs = new HashSet<string>();
               HashSet<decimal> callOutDurs = new HashSet<decimal>();
               HashSet<decimal> callInDurs = new HashSet<decimal>();
               int countOutCalls = 0;
               int countInCalls = 0;
               int countOutFails = 0;
               int countInFails = 0;
               int countInOffNets = 0;
               int countOutOffNets = 0;
               int countInOnNets = 0;
               int countOutOnNets = 0;
               int countInInters = 0;
               int countOutInters = 0;
               int countOutSMSs = 0;
               decimal totalDataVolume = 0;

               int count = 0;
               int currentIndex = 0;
              
                   currentIndex++;
                   if (currentIndex == 10000)
                   {
                       count += currentIndex;
                       currentIndex = 0;
                       Console.WriteLine("{0} rows read", count);
                   }

                   _callType = normalCDR.callType;
                   _bTSId = normalCDR.bTSId;
                   _connectDateTime = normalCDR.connectDateTime;
                   _id = normalCDR.id;
                   _iMSI = normalCDR.iMSI;
                   _durationInSeconds = normalCDR.durationInSeconds;
                   _disconnectDateTime = normalCDR.disconnectDateTime;
                   _callClass = normalCDR.callClass;
                   _isOnNet = normalCDR.isOnNet;
                   _subType = normalCDR.subType;
                   _iMEI = normalCDR.iMEI;
                   _cellId = normalCDR.cellId;
                   _switchRecordId = normalCDR.switchRecordId;
                   _upVolume = normalCDR.upVolume;
                   _downVolume = normalCDR.downVolume;
                   _cellLatitude = normalCDR.cellLatitude;
                   _cellLongitude = normalCDR.cellLongitude;
                   _inTrunk = normalCDR.inTrunk;
                   _outTrunk = normalCDR.outTrunk;
                   _serviceType = normalCDR.serviceType;
                   _serviceVASName = normalCDR.serviceVASName;
                   _destination = normalCDR.destination;




                   if (_mSISDN == string.Empty)
                   {
                       numberProfie = new NumberProfile();
                       _mSISDN = normalCDR.mSISDN;
                   }

                   else if (_mSISDN != normalCDR.mSISDN)
                   {
                       numberProfileBatch.Add(numberProfie);
                       if (BatchSize.HasValue && numberProfileBatch.Count == BatchSize)
                       {
                           inputArgument.OutputQueue.Enqueue(new NumberProfileBatch()
                           {
                               numberProfiles = numberProfileBatch
                           });
                           handle.SharedInstanceData.WriteTrackingMessage(BusinessProcess.Entities.BPTrackingSeverity.Information, "LoadNumberProfiles.DoWork.Enqueued Count Items: {0} ", numberProfileBatch.Count);
                           numberProfileBatch = new List<NumberProfile>();
                       }

                       numberProfie = new NumberProfile();
                       DestinationsIn = new HashSet<string>();
                       DestinationsOut = new HashSet<string>();
                       MSISDNsIn = new HashSet<string>();
                       MSISDNsOut = new HashSet<string>();
                       BTSIds = new HashSet<int>();
                       IMEIs = new HashSet<string>();
                       callOutDurs = new HashSet<decimal>();
                       callInDurs = new HashSet<decimal>();
                       countOutCalls = 0;
                       countInCalls = 0;
                       countOutFails = 0;
                       countInFails = 0;
                       countInOffNets = 0;
                       countOutOffNets = 0;
                       countInOnNets = 0;
                       countOutOnNets = 0;
                       countInInters = 0;
                       countOutInters = 0;
                       countOutSMSs = 0;
                       totalDataVolume = 0;

                       _mSISDN = normalCDR.mSISDN;
                   }


                   numberProfie.subscriberNumber = _mSISDN;










                   // Filling Agregates

                   numberProfie.periodId = (int) Enums.Period.Day;
                   numberProfie.fromDate = _connectDateTime;
                   numberProfie.isOnNet = 1;

                   totalDataVolume += (_upVolume + _downVolume);
                   numberProfie.totalDataVolume = totalDataVolume;




                   if ((int)Enums.Period.Day == (int)Enums.Period.Day)
                   {
                       numberProfie.toDate = _connectDateTime.AddDays(1);
                   }


                   if (_callType == (int)Enums.CallType.outgoingVoiceCall)
                   {
                       numberProfie.countOutCalls = ++countOutCalls;


                       if (!DestinationsOut.Contains(_destination))
                       {
                           DestinationsOut.Add(_destination);
                           numberProfie.diffOutputNumb = DestinationsOut.Count();
                       }

                       if (!DestinationsOut.Contains(_destination))
                       {
                           DestinationsOut.Add(_destination);
                           numberProfie.diffOutputNumb = DestinationsOut.Count();
                       }



                       if (_durationInSeconds == 0)
                           numberProfie.countOutFail = ++countOutFails;
                       else
                       {
                           callOutDurs.Add(_durationInSeconds / 60);
                           numberProfie.callOutDurAvg = callOutDurs.Average();
                           numberProfie.totalInVolume = callOutDurs.Sum();
                       }

                       if ((_callClass == Enum.GetName(typeof(Enums.CallClass), (int)Enums.CallClass.ASIACELL) || _callClass == Enum.GetName(typeof(Enums.CallClass), (int)Enums.CallClass.KOREKTEL)))
                           numberProfie.countOutOffNet = ++countOutOffNets;
                       else if ((_callClass == Enum.GetName(typeof(Enums.CallClass), (int)Enums.CallClass.ZAINIQ) || _callClass == Enum.GetName(typeof(Enums.CallClass), (int)Enums.CallClass.VAS) || _callClass == Enum.GetName(typeof(Enums.CallClass), (int)Enums.CallClass.INV)))
                           numberProfie.countOutOnNet = ++countOutOnNets;
                       else if ((_callClass == Enum.GetName(typeof(Enums.CallClass), (int)Enums.CallClass.INTL)))
                           numberProfie.countOutInter = ++countOutInters;
                   }


                   if (_callType == (int)Enums.CallType.incomingVoiceCall)
                   {
                       numberProfie.countOutCalls = ++countInCalls;

                       if (!DestinationsIn.Contains(_destination))
                       {
                           DestinationsIn.Add(_destination);
                           numberProfie.diffInputNumbers = DestinationsIn.Count();
                       }


                       if (_durationInSeconds == 0)
                           numberProfie.countInFail = ++countInFails;
                       else
                       {
                           callInDurs.Add(_durationInSeconds / 60);
                           numberProfie.callInDurAvg = callInDurs.Average();
                           numberProfie.totalInVolume = callInDurs.Sum();
                       }

                       if ((_callClass == Enum.GetName(typeof(Enums.CallClass), (int)Enums.CallClass.ASIACELL) || _callClass == Enum.GetName(typeof(Enums.CallClass), (int)Enums.CallClass.KOREKTEL)))
                           numberProfie.countInOffNet = ++countInOffNets;
                       else if ((_callClass == Enum.GetName(typeof(Enums.CallClass), (int)Enums.CallClass.ZAINIQ) || _callClass == Enum.GetName(typeof(Enums.CallClass), (int)Enums.CallClass.VAS) || _callClass == Enum.GetName(typeof(Enums.CallClass), (int)Enums.CallClass.INV)))
                           numberProfie.countInOnNet = ++countInOnNets;
                       else if ((_callClass == Enum.GetName(typeof(Enums.CallClass), (int)Enums.CallClass.INTL)))
                           numberProfie.countInInter = ++countInInters;
                   }

                   else if (_callType == (int)Enums.CallType.outgoingSms)
                   {
                       numberProfie.countOutSMS = ++countOutSMSs;
                   }





                   if ((_callType == (int)Enums.CallType.incomingVoiceCall || _callType == (int)Enums.CallType.outgoingVoiceCall || _callType == (int)Enums.CallType.incomingSms || _callType == (int)Enums.CallType.outgoingSms))
                   {
                       if (!IMEIs.Contains(_iMEI))
                           IMEIs.Add(_iMEI);
                       numberProfie.totalIMEI = IMEIs.Count();



                       if (!BTSIds.Contains(_bTSId))
                           BTSIds.Add(_bTSId);
                       numberProfie.totalBTS = BTSIds.Count();
                   }

               if (numberProfileBatch.Count > 0)
               {
                   inputArgument.OutputQueue.Enqueue(new NumberProfileBatch()
                   {
                       numberProfiles = numberProfileBatch
                       

                   });
                   handle.SharedInstanceData.WriteTrackingMessage(BusinessProcess.Entities.BPTrackingSeverity.Information, "LoadNumberProfiles.DoWork.Enqueued Count Items: {0} ", numberProfileBatch.Count);
               }


           });
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
