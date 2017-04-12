using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mediation.Teles.Business
{
    public static class TelesMediatorManager
    {
        public static ProcessCDREntity ProcessSingleLegCDRs(List<dynamic> cdrLegs, string prevTerminatorNumber = null, DateTime? disconnectDateTime = null)
        {
            ProcessCDREntity processCDREntity = new ProcessCDREntity();
            foreach (var cdrLeg in cdrLegs)
            {

            }

            return processCDREntity;
        }

        public static List<dynamic> ProcessMultiLegCDRs(List<dynamic> cdrLegs)
        {
            List<dynamic> cdrs = new List<dynamic>();
            var groupedLegs = cdrLegs.GroupBy(itm => itm.TC_CALLID);
            var prevTerminationNumber = "";
            for (int i = 0; i < groupedLegs.Count(); i++)
            {
                var group = groupedLegs.ElementAt(i).OrderBy(itm => itm.TC_SEQUENCENUMBER);
                ProcessCDREntity ProcessCDREntity = ProcessSingleLegCDRs(group.ToList(), prevTerminationNumber);
                prevTerminationNumber = ProcessCDREntity.PreviousTerminator;

            }
            return cdrs;
        }

        static dynamic GetSingleCDRFromLegs(List<dynamic> cdrLegs)
        {
            dynamic cdr = null;
            foreach (var cdrLeg in cdrLegs)
            {
                cdr = GenerateCDRFromLeg(cdr, cdrLeg);
            }
            return cdr;
        }

        static dynamic GenerateCDRFromLeg(dynamic cdr, dynamic cdrLeg, string cdpn = null, string cgpn = null)
        {
            if (cdrLeg.TC_LOGTYPE == "START")
            {
                cdr = default(dynamic);
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
        public DateTime? Disconnect { get; set; }
        public List<dynamic> CookedCDRs { get; set; }
        public ProcessCDREntity()
        {
            CookedCDRs = new List<dynamic>();
        }
    }
}
