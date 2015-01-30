using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CallGeneratorLibrary.Repositories
{
    public class CDRRepository
    {
        public static bool Save(CDR CDR)
        {
            bool success = false;
            if (CDR.CDRID == default(int))
                success = Insert(CDR);
            else
                success = Update(CDR);
            return success;
        }

        public static bool Insert(CDR CDR)
        {
            bool success = false;
            try
            {
                using (CallGeneratorModelDataContext context = new CallGeneratorModelDataContext())
                {
                    context.CDRs.InsertOnSubmit(CDR);
                    context.SubmitChanges();
                    success = true;
                }
            }
            catch (System.Exception ex)
            {
                WriteToEventLogEx(ex.ToString());
                Logger.LogException(ex);
            }
            return success;
        }

        private static bool Update(CDR CDR)
        {
            bool success = false;
            CDR look = new CDR();

            try
            {
                using (CallGeneratorModelDataContext context = new CallGeneratorModelDataContext())
                {
                    look = context.CDRs.Single(l => l.CDRID == CDR.CDRID);

                    look.IDonSwitch = CDR.IDonSwitch;
                    look.AttemptDateTime = CDR.AttemptDateTime;
                    look.AlertDateTime = CDR.AlertDateTime;
                    look.ConnectDateTime = CDR.ConnectDateTime;
                    look.DisconnectDateTime = CDR.DisconnectDateTime;
                    look.DurationInSeconds = CDR.DurationInSeconds;
                    look.IN_TRUNK = CDR.IN_TRUNK;
                    look.IN_CIRCUIT = CDR.IN_CIRCUIT;
                    look.IN_CARRIER = CDR.IN_CARRIER;
                    look.IN_IP = CDR.IN_IP;
                    look.OUT_TRUNK = CDR.OUT_TRUNK;
                    look.OUT_CIRCUIT = CDR.OUT_CIRCUIT;
                    look.OUT_CARRIER = CDR.OUT_CARRIER;
                    look.OUT_IP = CDR.OUT_IP;
                    look.CGPN = CDR.CGPN;
                    look.CDPN = CDR.CDPN;
                    look.CAUSE_FROM_RELEASE_CODE = CDR.CAUSE_FROM_RELEASE_CODE;
                    look.CAUSE_FROM = CDR.CAUSE_FROM;
                    look.CAUSE_TO_RELEASE_CODE = CDR.CAUSE_TO_RELEASE_CODE;
                    look.CAUSE_TO = CDR.CAUSE_TO;
                    look.Extra_Fields = CDR.Extra_Fields;
                    look.IsRerouted = CDR.IsRerouted;
                    look.CDPNOut = CDR.CDPNOut;
                    look.SIP = CDR.SIP;
                    look.UserID = CDR.UserID;
                    look.TransactionId = CDR.TransactionId;
                    look.RecievedCLI = CDR.RecievedCLI;
                    context.SubmitChanges();
                    success = true;
                }
            }
            catch (System.Exception ex)
            {
                WriteToEventLogEx(ex.ToString());
                Logger.LogException(ex);
            }
            return success;
        }

        private static void WriteToEventLogEx(string message)
        {
            string cs = "Call Generator Service";
            EventLog elog = new EventLog();
            if (!EventLog.SourceExists(cs))
            {
                EventLog.CreateEventSource(cs, cs);
            }
            elog.Source = cs;
            elog.EnableRaisingEvents = true;
            elog.WriteEntry(message);
        }
    }
}
