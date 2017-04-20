using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mediation.Generic.Entities;
using Vanrise.GenericData.Business;

namespace Mediation.Teles.Business
{
    public static class TelesMediatorManager
    {
        public static ProcessCDREntity ProcessSingleLegCDRs(List<dynamic> cdrLegs, string cookedCDRRecordTypeName, string prevTerminatorNumber = null, DateTime? disconnectDateTime = null, CentrixCallType? prevCallType = null)
        {
            ProcessCDREntity processCDREntity = new ProcessCDREntity();
            if (cdrLegs != null && cdrLegs.Count > 0)
            {
                dynamic startRecord = cdrLegs.Where(itm => itm.TC_LOGTYPE == "START").First();
                dynamic stopRecord = cdrLegs.Where(itm => itm.TC_LOGTYPE == "STOP").First();
                DateTime connectDateTime = startRecord.TC_TIMESTAMP;
                disconnectDateTime = disconnectDateTime == null ? stopRecord.TC_TIMESTAMP : disconnectDateTime;
                dynamic cookedCDR = null;
                string startCallProgressState = startRecord.TC_CALLPROGRESSSTATE;
                string stopCallProgressState = stopRecord.TC_CALLPROGRESSSTATE;
                if (string.IsNullOrEmpty(startRecord.TC_ORIGINATORNUMBER))// Routing Case
                {
                    processCDREntity.CookedCDRs.Add(GetCookedCDR(startRecord, cookedCDRRecordTypeName, startRecord.TC_ORIGINATORID, startRecord.TC_ORIGINALDIALEDNUMBER, disconnectDateTime, CentrixCallType.Routing));
                    processCDREntity.CookedCDRs.Add(GetCookedCDR(startRecord, cookedCDRRecordTypeName, startRecord.TC_ORIGINALDIALEDNUMBER, startRecord.TC_TERMINATORNUMBER, disconnectDateTime, CentrixCallType.Routing));
                    prevTerminatorNumber = startRecord.TC_TERMINATORNUMBER;
                }
                else
                {
                    CentrixCallType callType = CentrixCallType.Normal;
                    if (string.IsNullOrEmpty(startCallProgressState) && string.IsNullOrEmpty(stopCallProgressState))
                    {
                        if (prevCallType.HasValue)
                            callType = prevCallType.Value;
                    }
                    else
                    {
                        callType = stopCallProgressState.Contains("XFER") ? CentrixCallType.Transfer : GetCallType(startCallProgressState);
                    }

                    string[] terminatorNumbers = startRecord.TC_TERMINATORNUMBER.Split(';');
                    foreach (var terminatorNumber in terminatorNumbers)
                    {
                        string originatorNumber = string.IsNullOrEmpty(prevTerminatorNumber) ? startRecord.TC_ORIGINATORNUMBER : prevTerminatorNumber;
                        cookedCDR = GetCookedCDR(startRecord, cookedCDRRecordTypeName, originatorNumber, terminatorNumber, disconnectDateTime, callType);
                        prevTerminatorNumber = terminatorNumber;
                        processCDREntity.CookedCDRs.Add(cookedCDR);
                    }
                    processCDREntity.CallType = callType;
                }
                processCDREntity.PreviousTerminator = prevTerminatorNumber;

            }
            return processCDREntity;
        }

        private static CentrixCallType GetCallType(string callProgressState)
        {
            switch (callProgressState)
            {
                case "CFU":
                    return CentrixCallType.Forward;
                case "CFU;XFER":
                    return CentrixCallType.Transfer;
                case "REDIR":
                    return CentrixCallType.Redirect;
                case "CQ":
                    return CentrixCallType.Pickup;
            }
            return CentrixCallType.Normal;
        }
        public static List<dynamic> ProcessMultiLegCDRs(List<dynamic> cdrLegs, string cookedCDRRecordTypeName)
        {
            List<dynamic> records = new List<dynamic>();
            var groupedLegs = cdrLegs.GroupBy(itm => itm.TC_CALLID);
            string prevTerminationNumber = null;
            CentrixCallType? callType = null;
            DateTime disconnectDateTime = cdrLegs.OrderBy(itm => itm.TC_SEQUENCENUMBER).Where(itm => itm.TC_LOGTYPE == "STOP").Last().TC_TIMESTAMP;
            foreach (var group in groupedLegs.OrderBy(group => group.Min(record => record.TC_SEQUENCENUMBER)))
            {
                ProcessCDREntity ProcessCDREntity = ProcessSingleLegCDRs(group.ToList(), cookedCDRRecordTypeName, prevTerminationNumber, disconnectDateTime, callType);
                prevTerminationNumber = ProcessCDREntity.PreviousTerminator;
                callType = ProcessCDREntity.CallType;
                records.AddRange(ProcessCDREntity.CookedCDRs);
            }
            return records;
        }
        static dynamic GetCookedCDR(dynamic startCDR, string cookedCDRRecordTypeName, string originatorNumber, string terminatorNumber, DateTime? disconnectDateTime, CentrixCallType callType)
        {
            DataRecordTypeManager dataRecordTypeManager = new DataRecordTypeManager();
            dynamic cookedCDR = Activator.CreateInstance(dataRecordTypeManager.GetDataRecordRuntimeType(cookedCDRRecordTypeName));
            cookedCDR.AttemptDateTime = startCDR.TC_SESSIONINITIATIONTIME;
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
            cookedCDR.CallType = (int)callType;
            cookedCDR.DurationInSeconds = (disconnectDateTime.HasValue ? (decimal)(cookedCDR.DiconnectDateTime - cookedCDR.ConnectDateTime).TotalSeconds : 0);
            return cookedCDR;
        }
        static dynamic GenerateCDRFromLeg(dynamic cdr, dynamic cdrLeg, string cdpn = null, string cgpn = null)
        {
            if (cdrLeg.TC_LOGTYPE == "START")
            {
                cdr = new ExpandoObject();
                cdr.CallId = cdrLeg.TC_CALLID;
                cdr.AttemptDateTime = cdrLeg.TC_SESSIONINITIATIONTIME;
                cdr.ConnectDateTime = cdrLeg.TC_TIMESTAMP;
                cdr.OriginatorNumber = string.IsNullOrEmpty(cgpn) ? cdrLeg.TC_ORIGINATORNUMBER : cgpn;
                cdr.TerminatorNumber = string.IsNullOrEmpty(cdpn) ? cdrLeg.TC_TERMINATORNUMBER : cdpn;
                cdr.DisconnectReason = cdrLeg.TC_DISCONNECTREASON;
                cdr.OriginatorId = cdrLeg.TC_ORIGINATORID;
                cdr.OriginatorFromNumber = cdrLeg.TC_ORIGINALFROMNUMBER;
                cdr.OriginalDialedNumber = cdrLeg.TC_ORIGINALDIALEDNUMBER;
                cdr.TerminatorId = cdrLeg.TC_TERMINATORID;
                cdr.IncomingGwId = cdrLeg.TC_INCOMINGGWID;
                cdr.OutgoingGwId = cdrLeg.TC_OUTGOINGGWID;
                cdr.TransferredCallId = cdrLeg.TC_TRANSFERREDCALLID;
                cdr.OriginatorIp = cdrLeg.TC_ORIGINATORIP;
                cdr.TerminatorIp = cdrLeg.TC_TERMINATORIP;

            }
            else if (cdrLeg.IsMultiLegSessionEnd)
            {
                cdr.DisconnectDateTime = cdrLeg.TC_TIMESTAMP;
            }
            return cdr;
        }
    }

    public class ProcessCDREntity
    {
        public string PreviousTerminator { get; set; }
        public List<dynamic> CookedCDRs { get; set; }
        public CentrixCallType? CallType { get; set; }
        public ProcessCDREntity()
        {
            CookedCDRs = new List<dynamic>();
        }
    }
}
