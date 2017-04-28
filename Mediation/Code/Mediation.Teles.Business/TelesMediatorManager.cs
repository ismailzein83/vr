using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mediation.Generic.Entities;
using Vanrise.GenericData.Business;
using Vanrise.Common;

namespace Mediation.Teles.Business
{
    public static class TelesMediatorManager
    {
        public static ProcessCDREntity ProcessSingleLegCDRs(List<dynamic> cdrLegs, string cookedCDRRecordTypeName, string prevTerminatorNumber = null, DateTime? disconnectDateTime = null, CentrixReceiveCallType? prevRecieveCallType = null, bool? isLast = default(bool?))
        {
            ProcessCDREntity processCDREntity = new ProcessCDREntity();
            try
            {
                CentrixSendCallType sendCallType = prevRecieveCallType == null ? CentrixSendCallType.Normal : Utilities.GetEnumAttribute<CentrixReceiveCallType, ReceiveCallTypeAttribute>(prevRecieveCallType.Value).CorrespondingSendCallType;
                List<CentrixReceiveCallType> receiveCallTypes = GetReceiveCallTypes(cdrLegs);

                if (cdrLegs != null && cdrLegs.Count > 0)
                {
                    dynamic startRecord = cdrLegs.Where(itm => itm.TC_LOGTYPE == "START").FirstOrDefault();
                    dynamic stopRecord = cdrLegs.Where(itm => itm.TC_LOGTYPE == "STOP").FirstOrDefault();

                    if (IsInvalidRecords(cookedCDRRecordTypeName, processCDREntity, startRecord, stopRecord))
                        return processCDREntity;

                    DateTime connectDateTime = startRecord.TC_TIMESTAMP;
                    disconnectDateTime = disconnectDateTime == null ? stopRecord.TC_TIMESTAMP : disconnectDateTime;
                    dynamic cookedCDR = null;

                    if (string.IsNullOrEmpty(startRecord.TC_ORIGINATORNUMBER))// Routing Case
                    {
                        processCDREntity.CookedCDRs.Add(GetCookedCDR(startRecord, cookedCDRRecordTypeName, startRecord.TC_ORIGINATORID, startRecord.TC_ORIGINALDIALEDNUMBER, disconnectDateTime, CentrixSendCallType.Normal, CentrixReceiveCallType.Routing, false));
                        processCDREntity.CookedCDRs.Add(GetCookedCDR(startRecord, cookedCDRRecordTypeName, startRecord.TC_ORIGINALDIALEDNUMBER, startRecord.TC_TERMINATORNUMBER, disconnectDateTime, CentrixSendCallType.Routing, CentrixReceiveCallType.Normal, false));
                        prevTerminatorNumber = startRecord.TC_TERMINATORNUMBER;
                    }
                    else
                    {
                        string[] terminatorNumbers = startRecord.TC_TERMINATORNUMBER.Split(';');
                        for (int i = 0; i < terminatorNumbers.Length; i++)
                        {
                            CentrixReceiveCallType receiveCallType = CentrixReceiveCallType.Normal;
                            var terminatorNumber = terminatorNumbers[i];
                            string originatorNumber = string.IsNullOrEmpty(prevTerminatorNumber) ? startRecord.TC_ORIGINATORNUMBER : prevTerminatorNumber;
                            receiveCallType = receiveCallTypes.Count >= terminatorNumbers.Length ? receiveCallTypes[i] : receiveCallTypes[0];
                            cookedCDR = GetCookedCDR(startRecord, cookedCDRRecordTypeName, originatorNumber, terminatorNumber, disconnectDateTime, sendCallType, GetRecieveCallType(isLast, terminatorNumbers, i, receiveCallType), false);

                            sendCallType = Utilities.GetEnumAttribute<CentrixReceiveCallType, ReceiveCallTypeAttribute>(receiveCallType).CorrespondingSendCallType;

                            prevTerminatorNumber = terminatorNumber;
                            processCDREntity.CallType = receiveCallType;
                            processCDREntity.CookedCDRs.Add(cookedCDR);
                        }
                    }
                    processCDREntity.PreviousTerminator = prevTerminatorNumber;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return processCDREntity;
        }
        public static List<dynamic> ProcessMultiLegCDRs(List<dynamic> cdrLegs, string cookedCDRRecordTypeName)
        {
            List<dynamic> records = new List<dynamic>();
            var groupedLegs = cdrLegs.GroupBy(itm => itm.TC_CALLID);
            string prevTerminationNumber = null;
            CentrixReceiveCallType? callType = null;
            DateTime disconnectDateTime = cdrLegs.OrderBy(itm => itm.TC_SEQUENCENUMBER).Where(itm => itm.TC_LOGTYPE == "STOP").Last().TC_TIMESTAMP;
            var i = 0;
            var count = groupedLegs.Count();
            foreach (var group in groupedLegs.OrderBy(group => group.Min(record => record.TC_SEQUENCENUMBER)))
            {
                ProcessCDREntity ProcessCDREntity = ProcessSingleLegCDRs(group.ToList(), cookedCDRRecordTypeName, prevTerminationNumber, disconnectDateTime, callType, ++i == count);
                prevTerminationNumber = ProcessCDREntity.PreviousTerminator;
                callType = ProcessCDREntity.CallType;
                records.AddRange(ProcessCDREntity.CookedCDRs);
            }
            return records;
        }

        #region Private Functions
        static bool IsInvalidRecords(string cookedCDRRecordTypeName, ProcessCDREntity processCDREntity, dynamic startRecord, dynamic stopRecord)
        {
            if (stopRecord != null && startRecord == null && !string.IsNullOrEmpty(stopRecord.TC_DISCONNECTREASON) && stopRecord.TC_DISCONNECTREASON != "BYE")
            {
                processCDREntity.CookedCDRs.Add(GetCookedCDR(stopRecord, cookedCDRRecordTypeName, stopRecord.TC_ORIGINATORNUMBER, stopRecord.TC_TERMINATORNUMBER, stopRecord.TC_TIMESTAMP, CentrixSendCallType.Cancel, CentrixReceiveCallType.Cancel, true));
                return true;
            }
            else if (startRecord == null || stopRecord == null)
                return true;
            return false;
        }
        static CentrixReceiveCallType GetRecieveCallType(bool? isLast, string[] terminatorNumbers, int i, CentrixReceiveCallType receiveCallType)
        {
            if (receiveCallType == CentrixReceiveCallType.VoiceMail || receiveCallType == CentrixReceiveCallType.Conference || (i != terminatorNumbers.Length - 1))
                return receiveCallType;
            return isLast.HasValue && !isLast.Value ? receiveCallType : CentrixReceiveCallType.Normal;
        }
        static List<CentrixReceiveCallType> GetReceiveCallTypes(List<dynamic> cdrLegs)
        {
            List<CentrixReceiveCallType> receiveCallTypes = new List<CentrixReceiveCallType>();// CentrixCallType.Normal;
            foreach (var record in cdrLegs)
            {
                if (!string.IsNullOrEmpty(record.TC_CALLPROGRESSSTATE))
                {
                    CentrixReceiveCallType receiveCallType = GetCallType(record);
                    receiveCallTypes.Add(receiveCallType);
                    if (record.TC_CALLINDICATOR == "cmc-t")
                        break;
                }
            }
            if (receiveCallTypes.Count == 0)
                receiveCallTypes.Add(CentrixReceiveCallType.Normal);
            return receiveCallTypes;
        }
        static CentrixReceiveCallType GetCallType(dynamic cdrLeg)
        {
            string callType = cdrLeg.TC_CALLPROGRESSSTATE;
            string callIndicator = cdrLeg.TC_CALLINDICATOR;
            switch (callType)
            {
                case "CFU":
                    return CentrixReceiveCallType.Forward;
                case "XFER":
                case "CFU;XFER":
                    return CentrixReceiveCallType.Transfer;
                case "REDIR":
                    return CentrixReceiveCallType.Redirect;
                case "CQ":
                    return CentrixReceiveCallType.Pickup;
                case "cmc-t":
                    switch (callIndicator)
                    {
                        case "CONFERENCE_DEDICATED":
                            return CentrixReceiveCallType.Conference;
                        case "VOICE_MAIL_RECORDING":
                            return CentrixReceiveCallType.VoiceMail;

                        default:
                            return CentrixReceiveCallType.Normal;
                    }
            }
            return CentrixReceiveCallType.Normal;
        }
        static dynamic GetCookedCDR(dynamic startCDR, string cookedCDRRecordTypeName, string originatorNumber, string terminatorNumber, DateTime? disconnectDateTime, CentrixSendCallType sendCallType, CentrixReceiveCallType receiveCallType, bool isZeroDuration)
        {
            DataRecordTypeManager dataRecordTypeManager = new DataRecordTypeManager();
            dynamic cookedCDR = Activator.CreateInstance(dataRecordTypeManager.GetDataRecordRuntimeType(cookedCDRRecordTypeName));
            cookedCDR.AttemptDateTime = startCDR.TC_SESSIONINITIATIONTIME != null ? startCDR.TC_SESSIONINITIATIONTIME : startCDR.TC_TIMESTAMP;
            cookedCDR.ConnectDateTime = startCDR.TC_TIMESTAMP;
            cookedCDR.DiconnectDateTime = disconnectDateTime;
            cookedCDR.DisconnectReason = startCDR.TC_DISCONNECTREASON;
            cookedCDR.OriginatorNumber = originatorNumber;
            cookedCDR.TerminatorNumber = terminatorNumber;
            cookedCDR.OriginatorId = startCDR.TC_ORIGINATORID;
            cookedCDR.OriginatorFromNumber = startCDR.TC_ORIGINALFROMNUMBER;
            cookedCDR.OriginalDialedNumber = startCDR.TC_ORIGINALDIALEDNUMBER;
            cookedCDR.TerminatorId = startCDR.TC_TERMINATORID;
            cookedCDR.IncomingGwId = startCDR.TC_INCOMINGGWID;
            cookedCDR.OutgoingGwId = startCDR.TC_OUTGOINGGWID;
            cookedCDR.TransferredCallId = startCDR.TC_TRANSFERREDCALLID;
            cookedCDR.CallProgressState = startCDR.TC_CALLPROGRESSSTATE;
            cookedCDR.SendCallType = (int)sendCallType;
            cookedCDR.ReceiveCallType = (int)receiveCallType;
            cookedCDR.DurationInSeconds = (disconnectDateTime.HasValue && !isZeroDuration ? (decimal)(cookedCDR.DiconnectDateTime - cookedCDR.ConnectDateTime).TotalSeconds : 0);
            return cookedCDR;
        }

        #endregion

    }

    public class ProcessCDREntity
    {
        public string PreviousTerminator { get; set; }
        public List<dynamic> CookedCDRs { get; set; }
        public CentrixReceiveCallType? CallType { get; set; }
        public ProcessCDREntity()
        {
            CookedCDRs = new List<dynamic>();
        }
    }
}
