using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Business;

namespace Mediation.Teles.Business
{
    public static class TelesMediatorManager
    {
        public static ProcessCDREntity ProcessSingleLegCDRs(List<dynamic> cdrLegs, string cookedCDRRecordTypeName, string prevTerminatorNumber = null, DateTime? disconnectDateTime = null)
        {
            ProcessCDREntity processCDREntity = new ProcessCDREntity();
            if (cdrLegs != null && cdrLegs.Count > 0)
            {
                dynamic startCDR = cdrLegs.Where(itm => itm.TC_LOGTYPE == "START").First();
                dynamic stopCDR = cdrLegs.Where(itm => itm.TC_LOGTYPE == "STOP").First();
                DateTime connectDateTime = startCDR.TC_TIMESTAMP;
                disconnectDateTime = disconnectDateTime == null ? stopCDR.TC_TIMESTAMP : disconnectDateTime;
                dynamic cookedCDR = null;
                if (string.IsNullOrEmpty(startCDR.TC_ORIGINATORNUMBER))// Routing Case
                {
                    processCDREntity.CookedCDRs.Add(GetCookedCDR(startCDR, cookedCDRRecordTypeName, startCDR.OriginatorId, startCDR.OriginalDialedNumber, disconnectDateTime));
                    processCDREntity.CookedCDRs.Add(GetCookedCDR(startCDR, cookedCDRRecordTypeName, startCDR.OriginalDialedNumber, startCDR.TC_TERMINATORNUMBER, disconnectDateTime));
                    prevTerminatorNumber = startCDR.TC_TERMINATORNUMBER;
                }
                else
                {
                    string[] terminatorNumbers = startCDR.TC_TERMINATORNUMBER.Split(';');
                    foreach (var terminatorNumber in terminatorNumbers)
                    {
                        string originatorNumber = string.IsNullOrEmpty(prevTerminatorNumber) ? startCDR.TC_ORIGINATORNUMBER : prevTerminatorNumber;
                        cookedCDR = GetCookedCDR(startCDR, cookedCDRRecordTypeName, prevTerminatorNumber, terminatorNumber, disconnectDateTime);
                        prevTerminatorNumber = terminatorNumber;
                        processCDREntity.CookedCDRs.Add(cookedCDR);
                    }
                }
                processCDREntity.PreviousTerminator = prevTerminatorNumber;
                processCDREntity.DisconnectDateTime = disconnectDateTime;
            }
            return processCDREntity;
        }
        public static List<dynamic> ProcessMultiLegCDRs(List<dynamic> cdrLegs, string cookedCDRRecordTypeName)
        {
            List<dynamic> cdrs = new List<dynamic>();
            var groupedLegs = cdrLegs.GroupBy(itm => itm.TC_CALLID);
            var prevTerminationNumber = "";
            DateTime disconnectDateTime = cdrLegs.OrderBy(itm => itm.TC_SEQUENCENUMBER).Where(itm => itm.TC_LOGTYPE == "STOP").Last().TC_TIMESTAMP;
            for (int i = 0; i < groupedLegs.Count(); i++)
            {
                var group = groupedLegs.ElementAt(i).OrderBy(itm => itm.TC_SEQUENCENUMBER);
                ProcessCDREntity ProcessCDREntity = ProcessSingleLegCDRs(group.ToList(), cookedCDRRecordTypeName, prevTerminationNumber, disconnectDateTime);
                prevTerminationNumber = ProcessCDREntity.PreviousTerminator;
                cdrs.AddRange(ProcessCDREntity.CookedCDRs);

            }
            return cdrs;
        }
        static dynamic GetCookedCDR(dynamic startCDR, string cookedCDRRecordTypeName, string originatorNumber, string terminatorNumber, DateTime? disconnectDateTime)
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
        public DateTime? DisconnectDateTime { get; set; }
        public List<dynamic> CookedCDRs { get; set; }
        public ProcessCDREntity()
        {
            CookedCDRs = new List<dynamic>();
        }
    }
}
