using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Data.Linq;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CallGeneratorLibrary.Utilities;
using System.Diagnostics;
using CallGeneratorLibrary;

namespace CallGeneratorLibrary.Repositories
{
    public class ChartCall
    {
        public int ChartId { get; set; }
        public int Total { get; set; }
    }

    public class CDRRepository
    {

        public static CDR Load(double CDRID)
        {
            CDR log = new CDR();

            try
            {
                using (CallGeneratorModelDataContext context = new CallGeneratorModelDataContext())
                {
                    log = context.CDRs.Where(l => l.CDRID == CDRID).FirstOrDefault<CDR>();
                }
            }
            catch (System.Exception ex)
            {
                Logger.LogException(ex);
            }

            return log;
        }

        public static List<ChartCall> GetChartCalls(int status)
        {
            List<ChartCall> LstChartCalls = new List<ChartCall>();
            List<ChartCalls> LstOp = new List<ChartCalls>();
            try
            {
                using (CallGeneratorModelDataContext context = new CallGeneratorModelDataContext())
                {
                    LstOp = context.GetChartTotals1(status).GetResult<ChartCalls>().ToList<ChartCalls>();
                    for (int i = 1; i <= 31; i++)
                    {
                        ChartCall c = new ChartCall();
                        c.ChartId = i;
                        c.Total = 0;
                        foreach (ChartCalls cc in LstOp)
                        {
                            if (cc.CreationDate.Value.Day == i)
                            {
                                c.Total = cc.TotalCalls.Value;
                                break;
                            }
                        }
                        LstChartCalls.Add(c);
                    }
                }
            }
            catch (System.Exception ex)
            {
                WriteToEventLogEx(ex.ToString());
                Logger.LogException(ex);
            }
            return LstChartCalls;
        }

        public static int GetTotalCallsITPC()
        {
            int Total = 0;
            try
            {
                using (CallGeneratorModelDataContext context = new CallGeneratorModelDataContext())
                {
                    Total = context.CDRs.Where(l => l.ConnectDateTime.Value.Month == DateTime.Now.Month && l.ClientId == 1).Count();
                }
            }
            catch (System.Exception ex)
            {
                WriteToEventLogEx(ex.ToString());
                Logger.LogException(ex);
            }
            return Total;
        }
        public static int GetTotalCallsZain()
        {
            int Total = 0;
            try
            {
                using (CallGeneratorModelDataContext context = new CallGeneratorModelDataContext())
                {
                    Total = context.CDRs.Where(l => l.ConnectDateTime.Value.Month == DateTime.Now.Month && l.ClientId == 2).Count();
                }
            }
            catch (System.Exception ex)
            {
                WriteToEventLogEx(ex.ToString());
                Logger.LogException(ex);
            }
            return Total;
        }
        public static int GetTotalCallsST()
        {
            int Total = 0;
            try
            {
                using (CallGeneratorModelDataContext context = new CallGeneratorModelDataContext())
                {
                    Total = context.CDRs.Where(l => l.ConnectDateTime.Value.Month == DateTime.Now.Month && l.ClientId == 3).Count();
                }
            }
            catch (System.Exception ex)
            {
                WriteToEventLogEx(ex.ToString());
                Logger.LogException(ex);
            }
            return Total;
        }

        public static List<CDRHistory> GetCDRHistory(DateTime? StartDate, DateTime? EndDate, string Number, int? ClientId, int? DisplayStart, int? DisplayLength)
        {
            List<CDRHistory> LstScheduleNumbers = new List<CDRHistory>();
            try
            {
                using (CallGeneratorModelDataContext context = new CallGeneratorModelDataContext())
                {
                    LstScheduleNumbers = context.GetCDR1(StartDate, EndDate, Number, ClientId, DisplayStart, DisplayLength).GetResult<CDRHistory>().ToList<CDRHistory>();
                }
            }
            catch (System.Exception ex)
            {
                Logger.LogException(ex);
            }
            return LstScheduleNumbers;
        }

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
            string cs = "VanCallGen";
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
