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

        static int _extensionMaxLength = 4;
        public static ProcessCDREntity ProcessSingleLegCDRs(List<dynamic> cdrLegs, string cookedCDRRecordTypeName, OptionalParametersEntity opEntity)
        {
            ProcessCDREntity processCDREntity = new ProcessCDREntity();
            try
            {
                if (opEntity == null)
                    opEntity = new OptionalParametersEntity();

                CentrixSendCallType sendCallType = opEntity.PrevRecieveCallType == null ? CentrixSendCallType.Normal : Utilities.GetEnumAttribute<CentrixReceiveCallType, ReceiveCallTypeAttribute>(opEntity.PrevRecieveCallType.Value).CorrespondingSendCallType;
                List<CentrixReceiveCallType> receiveCallTypes = GetReceiveCallTypes(cdrLegs);

                if (cdrLegs != null && cdrLegs.Count > 0)
                {
                    dynamic startRecord = cdrLegs.Where(itm => itm.TC_LOGTYPE == "START").FirstOrDefault();
                    dynamic stopRecord = cdrLegs.Where(itm => itm.TC_LOGTYPE == "STOP").FirstOrDefault();

                    if (IsInvalidRecords(cookedCDRRecordTypeName, processCDREntity, startRecord, stopRecord))
                        return processCDREntity;

                    DateTime connectDateTime = startRecord.TC_TIMESTAMP;
                    opEntity.DisconnectDateTime = opEntity.DisconnectDateTime == null ? stopRecord.TC_TIMESTAMP : opEntity.DisconnectDateTime;
                    dynamic cookedCDR = null;

                    if (string.IsNullOrEmpty(startRecord.TC_ORIGINATORNUMBER))// Routing Case
                    {
                        processCDREntity.CookedCDRs.Add(GetCookedCDR(startRecord, stopRecord, cookedCDRRecordTypeName, StripAndGetNumbers(startRecord.TC_ORIGINATORID as string).FirstOrDefault(), StripAndGetNumbers(startRecord.TC_ORIGINALDIALEDNUMBER as string).FirstOrDefault(), opEntity.DisconnectDateTime, CentrixSendCallType.Normal, CentrixReceiveCallType.Routing, false));
                        processCDREntity.CookedCDRs.Add(GetCookedCDR(startRecord, stopRecord, cookedCDRRecordTypeName, StripAndGetNumbers(startRecord.TC_ORIGINALDIALEDNUMBER as string).FirstOrDefault(), StripAndGetNumbers(startRecord.TC_TERMINATORNUMBER as string).FirstOrDefault(), opEntity.DisconnectDateTime, CentrixSendCallType.Routing, CentrixReceiveCallType.Normal, false));
                        opEntity.PrevTerminatorNumber = startRecord.TC_TERMINATORNUMBER;
                    }
                    else
                    {
                        //string terminatorId = startRecord.TC_TERMINATORNUMBER;
                        string[] terminatorIds = StripAndGetNumbers(startRecord.TC_TERMINATORID).ToArray();
                        string[] terminatorNumbers = StripAndGetNumbers(startRecord.TC_TERMINATORNUMBER).ToArray();
                        for (int i = 0; i < terminatorIds.Length; i++)
                        {
                            CentrixReceiveCallType receiveCallType = CentrixReceiveCallType.Normal;
                            var terminatorId = terminatorIds[i];
                            var terminatorNumber = terminatorNumbers[i];
                            if (!string.IsNullOrEmpty(terminatorId) && terminatorId.Length <= _extensionMaxLength)
                            {
                                terminatorNumber = terminatorNumber.Substring(0, terminatorNumber.Length - terminatorId.Length);
                            }
                            else
                            {
                                terminatorId = null;
                            }
                            string originatorNumber = string.IsNullOrEmpty(opEntity.PrevTerminatorNumber) ? StripAndGetNumbers(startRecord.TC_ORIGINATORNUMBER as string).FirstOrDefault() : opEntity.PrevTerminatorNumber;
                            receiveCallType = receiveCallTypes.Count >= terminatorNumbers.Length ? receiveCallTypes[i] : receiveCallTypes[0];
                            cookedCDR = GetCookedCDR(startRecord,
                                                        stopRecord,
                                                        cookedCDRRecordTypeName,
                                                        originatorNumber,
                                                        terminatorNumber,
                                                        opEntity.DisconnectDateTime,
                                                        sendCallType,
                                                        GetRecieveCallType(opEntity.IsLast, terminatorNumbers, i, receiveCallType),
                                                        false,
                                                        opEntity.PrevTerminatorExtension,
                                                        terminatorId);

                            sendCallType = Utilities.GetEnumAttribute<CentrixReceiveCallType, ReceiveCallTypeAttribute>(receiveCallType).CorrespondingSendCallType;

                            opEntity.PrevTerminatorNumber = terminatorNumber;
                            opEntity.PrevTerminatorExtension = terminatorId;
                            processCDREntity.CallType = receiveCallType;
                            processCDREntity.CookedCDRs.Add(cookedCDR);
                        }
                    }
                    processCDREntity.PreviousTerminator = opEntity.PrevTerminatorNumber;
                    processCDREntity.PrevTerminatorExtension = opEntity.PrevTerminatorExtension;
                }
            }
            catch (Exception ex)
            {
                //throw ex;
            }
            return processCDREntity;
        }
        public static List<dynamic> ProcessMultiLegCDRs(List<dynamic> cdrLegs, string cookedCDRRecordTypeName)
        {
            List<dynamic> records = new List<dynamic>();
            var groupedLegs = cdrLegs.GroupBy(itm => itm.TC_CALLID);
            string prevTerminationNumber = null;
            string prevTerminatorExtension = null;
            CentrixReceiveCallType? callType = null;
            DateTime disconnectDateTime = cdrLegs.OrderBy(itm => itm.TC_SEQUENCENUMBER).Where(itm => itm.TC_LOGTYPE == "STOP").Last().TC_TIMESTAMP;
            var i = 0;
            var count = groupedLegs.Count();
            foreach (var group in groupedLegs.OrderBy(group => group.Min(record => record.TC_SEQUENCENUMBER)))
            {
                OptionalParametersEntity opEntity = new OptionalParametersEntity
                {
                    DisconnectDateTime = disconnectDateTime,
                    IsLast = ++i == count,
                    PrevRecieveCallType = callType,
                    PrevTerminatorExtension = prevTerminatorExtension,
                    PrevTerminatorNumber = prevTerminationNumber
                };
                ProcessCDREntity ProcessCDREntity = ProcessSingleLegCDRs(group.ToList(), cookedCDRRecordTypeName, opEntity);
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
                processCDREntity.CookedCDRs.Add(GetCookedCDR(startRecord, stopRecord, cookedCDRRecordTypeName, stopRecord.TC_ORIGINATORNUMBER, stopRecord.TC_TERMINATORNUMBER, stopRecord.TC_TIMESTAMP, CentrixSendCallType.Cancel, CentrixReceiveCallType.Cancel, true));
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
            switch (callType.ToLower())
            {
                case "replaced":
                    return CentrixReceiveCallType.Replaced;
                case "cfto":
                case "cfb":
                case "cfu":
                    return CentrixReceiveCallType.Forward;
                case "xfer":
                case "cfnr;xfer":
                case "cfu;xfer":
                    return CentrixReceiveCallType.Transfer;
                case "redir":
                    return CentrixReceiveCallType.Redirect;
                case "cq":
                    return CentrixReceiveCallType.Pickup;
                case "cfnr":
                    return CentrixReceiveCallType.NoResponse;
                case "cmc-t":
                    switch (callIndicator)
                    {
                        case "conference_dedicated":
                            return CentrixReceiveCallType.Conference;
                        case "voice_mail_recording":
                            return CentrixReceiveCallType.VoiceMail;
                        default:
                            return CentrixReceiveCallType.Normal;
                    }
            }
            return CentrixReceiveCallType.Normal;
        }
        static dynamic GetCookedCDR(dynamic startRecord, dynamic stopRecord, string cookedCDRRecordTypeName, string originatorNumber, string terminatorNumber, DateTime? disconnectDateTime, CentrixSendCallType sendCallType, CentrixReceiveCallType receiveCallType, bool isZeroDuration, string originatorExtension = null, string terminatorExtension = null)
        {
            if (startRecord == null)
                startRecord = stopRecord;
            DataRecordTypeManager dataRecordTypeManager = new DataRecordTypeManager();
            dynamic cookedCDR = Activator.CreateInstance(dataRecordTypeManager.GetDataRecordRuntimeType(cookedCDRRecordTypeName));
            cookedCDR.AttemptDateTime = startRecord.TC_SESSIONINITIATIONTIME != null ? startRecord.TC_SESSIONINITIATIONTIME : startRecord.TC_TIMESTAMP;
            cookedCDR.ConnectDateTime = startRecord.TC_TIMESTAMP;
            cookedCDR.DiconnectDateTime = disconnectDateTime;
            cookedCDR.DisconnectReason = stopRecord.TC_DISCONNECTREASON;
            cookedCDR.OriginatorNumber = originatorNumber;
            cookedCDR.TerminatorNumber = terminatorNumber;
            cookedCDR.OriginatorId = stopRecord.TC_ORIGINATORID;
            cookedCDR.OriginatorFromNumber = stopRecord.TC_ORIGINALFROMNUMBER;
            cookedCDR.OriginalDialedNumber = stopRecord.TC_ORIGINALDIALEDNUMBER;
            cookedCDR.TerminatorId = stopRecord.TC_TERMINATORID;
            cookedCDR.IncomingGwId = stopRecord.TC_INCOMINGGWID;
            cookedCDR.OutgoingGwId = stopRecord.TC_OUTGOINGGWID;
            cookedCDR.TransferredCallId = stopRecord.TC_TRANSFERREDCALLID;
            cookedCDR.CallProgressState = stopRecord.TC_CALLPROGRESSSTATE;
            cookedCDR.SendCallType = (int)sendCallType;
            cookedCDR.ReceiveCallType = (int)receiveCallType;
            cookedCDR.CallId = stopRecord.TC_CALLID;
            cookedCDR.TransferredCallId = stopRecord.TC_TRANSFERREDCALLID;
            cookedCDR.ReplacedCallId = stopRecord.TC_REPLACECALLID;
            cookedCDR.FileName = stopRecord.FileName;
            cookedCDR.OriginatorExtension = originatorExtension;
            cookedCDR.TerminatorExtension = terminatorExtension;
            cookedCDR.OriginatorIp = stopRecord.TC_ORIGINATORIP;
            cookedCDR.TerminatorIp = stopRecord.TC_TERMINATORIP;
            cookedCDR.DurationInSeconds = (disconnectDateTime.HasValue && !isZeroDuration ? (decimal)(cookedCDR.DiconnectDateTime - cookedCDR.ConnectDateTime).TotalSeconds : 0);
            return cookedCDR;
        }

        static List<string> StripAndGetNumbers(string numberIds)
        {
            List<string> numbers = new List<string>();

            foreach (var numberId in numberIds.Split(';'))
            {
                string[] calls = numberId.Split(new char[] { ':', '@' });
                if (calls.Length > 1)
                {
                    numbers.Add(calls[1]);
                }
                else
                    numbers.Add(calls[0]);
            }

            return numbers;
        }

        #endregion

    }

    public class OptionalParametersEntity
    {
        public bool? IsLast { get; set; }
        public string PrevTerminatorNumber { get; set; }
        public string PrevTerminatorExtension { get; set; }
        public DateTime? DisconnectDateTime { get; set; }
        public CentrixReceiveCallType? PrevRecieveCallType { get; set; }
    }

    public class ProcessCDREntity
    {
        public string PreviousTerminator { get; set; }
        public List<dynamic> CookedCDRs { get; set; }
        public string PrevTerminatorExtension { get; set; }
        public CentrixReceiveCallType? CallType { get; set; }
        public ProcessCDREntity()
        {
            CookedCDRs = new List<dynamic>();
        }
    }
}
