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
            Exception exception = new Exception();

            try
            {
                using ( CallGeneratorModelDataContext context = new CallGeneratorModelDataContext())
                {
                    exception.Subject = ex.Message + " " + ex.InnerException;
                    exception.Details = ex.StackTrace;
                    exception.ExceptionDate = DateTime.Now;
                    context.Exceptions.InsertOnSubmit(exception);
                    context.SubmitChanges();
                }
            }
            catch(System.Exception ex2) {
                WriteToEventLogEx(ex2.ToString());
            }
        }

        public static void Save(string subject, string details)
        {
            Exception exception = new Exception();

            try
            {
                using (CallGeneratorModelDataContext context = new CallGeneratorModelDataContext())
                {
                    exception.Subject = subject;
                    exception.Details = details;
                    exception.ExceptionDate = DateTime.Now;
                    context.Exceptions.InsertOnSubmit(exception);
                    context.SubmitChanges();
                }
            }
            catch (System.Exception ex2)
            {
                WriteToEventLogEx(ex2.ToString());
            }
        }
        public static List<CallGeneratorLibrary.Exception> GetExceptions()
        {
            try
            {
                using (CallGeneratorModelDataContext context = new CallGeneratorModelDataContext())
                {
                    var q = from m in context.Exceptions
                            orderby m.Id descending
                            select m;
                    return q.ToList<CallGeneratorLibrary.Exception>();
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
                    List<CallGeneratorLibrary.Exception> currencyRate = new List<CallGeneratorLibrary.Exception>();
                    currencyRate = context.Exceptions.ToList<CallGeneratorLibrary.Exception>();
                    context.Exceptions.DeleteAllOnSubmit(currencyRate);
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
            //string cs = "Call Generator Lib Excep";
            //EventLog elog = new EventLog();
            //if (!EventLog.SourceExists(cs))
            //{
            //    EventLog.CreateEventSource(cs, cs);
            //}
            //elog.Source = cs;
            //elog.EnableRaisingEvents = true;
            //elog.WriteEntry(message);
        }
    }
}
