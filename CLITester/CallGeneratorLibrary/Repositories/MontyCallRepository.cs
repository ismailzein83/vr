using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CallGeneratorLibrary.Repositories
{
    public class MontyCallRepository
    {
        public static MontyCall Load(int MontyCallId)
        {
            MontyCall log = new MontyCall();

            try
            {
                using (CallGeneratorModelDataContext context = new CallGeneratorModelDataContext())
                {
                    log = context.MontyCalls.Where(l => l.Id == MontyCallId).FirstOrDefault<MontyCall>();
                }
            }
            catch (System.Exception ex)
            {
                WriteToEventLogEx(ex.ToString());
                Logger.LogException(ex);
            }

            return log;
        }

        public static MontyCall LoadbyTestOperatorId(int TestOpId)
        {
            MontyCall log = new MontyCall();

            try
            {
                using (CallGeneratorModelDataContext context = new CallGeneratorModelDataContext())
                {
                    log = context.MontyCalls.Where(l => l.TestOperatorId == TestOpId).FirstOrDefault<MontyCall>();
                }
            }
            catch (System.Exception ex)
            {
                WriteToEventLogEx(ex.ToString());
                Logger.LogException(ex);
            }

            return log;
        }

        public static bool Save(MontyCall montyCall)
        {
            bool success = false;
            if (montyCall.Id == default(int))
                success = Insert(montyCall);
            else
                success = Update(montyCall);
            return success;
        }

        private static bool Insert(MontyCall montyCall)
        {
            bool success = false;
            try
            {
                using (CallGeneratorModelDataContext context = new CallGeneratorModelDataContext())
                {
                    context.MontyCalls.InsertOnSubmit(montyCall);
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

        private static bool Update(MontyCall montyCallObj)
        {
            bool success = false;
            MontyCall montyCall = new MontyCall();

            try
            {
                using (CallGeneratorModelDataContext context = new CallGeneratorModelDataContext())
                {
                    montyCall = context.MontyCalls.Single(l => l.Id == montyCallObj.Id);

                    montyCall.TestOperatorId = montyCallObj.TestOperatorId;
                    montyCall.MSISDN = montyCallObj.MSISDN;
                    montyCall.CreationDate = montyCallObj.CreationDate;
                    montyCall.RequestId = montyCallObj.RequestId;
                    montyCall.GeneratedCallId = montyCallObj.GeneratedCallId;
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
