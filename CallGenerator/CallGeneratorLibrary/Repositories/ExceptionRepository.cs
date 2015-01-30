using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CallGeneratorLibrary.Repositories
{
    public class ExceptionRepository
    {
        public static void Save(System.Exception ex)
        {
            NewException exception = new NewException();

            try
            {
                using (CallGeneratorModelDataContext context = new CallGeneratorModelDataContext())
                {
                    exception.Subject = ex.Message + " " + ex.InnerException;
                    exception.Details = ex.StackTrace;
                    exception.ExceptionDate = DateTime.Now;
                    context.NewExceptions.InsertOnSubmit(exception);
                    context.SubmitChanges();
                }
            }
            catch (System.Exception ex2)
            {
                WriteToEventLogEx(ex2.ToString());
            }
        }

        public static void Save(string subject, string details)
        {
            NewException exception = new NewException();

            try
            {
                using (CallGeneratorModelDataContext context = new CallGeneratorModelDataContext())
                {
                    exception.Subject = subject;
                    exception.Details = details;
                    exception.ExceptionDate = DateTime.Now;
                    context.NewExceptions.InsertOnSubmit(exception);
                    context.SubmitChanges();
                }
            }
            catch (System.Exception ex2)
            {
                WriteToEventLogEx(ex2.ToString());
            }
        }
        public static List<CallGeneratorLibrary.NewException> GetExceptions()
        {
            try
            {
                using (CallGeneratorModelDataContext context = new CallGeneratorModelDataContext())
                {
                    var q = from m in context.NewExceptions
                            orderby m.Id descending
                            select m;
                    return q.ToList<CallGeneratorLibrary.NewException>();
                }
            }
            catch (System.Exception ex)
            {
                WriteToEventLogEx(ex.ToString());
                Logger.LogException(ex);
            }

            return null;
        }

        public static bool TruncateExceptions()
        {
            bool success = false;

            try
            {
                using (CallGeneratorModelDataContext context = new CallGeneratorModelDataContext())
                {
                    List<CallGeneratorLibrary.NewException> currencyRate = new List<CallGeneratorLibrary.NewException>();
                    currencyRate = context.NewExceptions.ToList<CallGeneratorLibrary.NewException>();
                    context.NewExceptions.DeleteAllOnSubmit(currencyRate);
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
