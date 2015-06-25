using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLINumberLibrary
{
    public class ExceptionRepository
    {
        public static void Save(System.Exception ex)
        {
            Exception exception = new Exception();

            try
            {
                using (CLINumberModelDataContext context = new CLINumberModelDataContext())
                {
                    exception.Subject = ex.Message + " " + ex.InnerException;
                    exception.Details = ex.StackTrace;
                    exception.ExceptionDate = DateTime.Now;
                    context.Exceptions.InsertOnSubmit(exception);
                    context.SubmitChanges();
                }
            }
            catch (System.Exception ex2)
            {
                Logger.LogException(ex2);
            }
        }

        public static void Save(string subject, string details)
        {
            Exception exception = new Exception();

            try
            {
                using (CLINumberModelDataContext context = new CLINumberModelDataContext())
                {
                    exception.Subject = subject;
                    exception.Details = details;
                    exception.ExceptionDate = DateTime.Now;
                    context.Exceptions.InsertOnSubmit(exception);
                    context.SubmitChanges();
                }
            }
            catch (System.Exception ex)
            {
                Logger.LogException(ex);
            }
        }

        public static List<CLINumberLibrary.Exception> GetExceptions()
        {
            try
            {
                using (CLINumberModelDataContext context = new CLINumberModelDataContext())
                {
                    var q = from m in context.Exceptions
                            orderby m.Id descending
                            select m;
                    return q.ToList<CLINumberLibrary.Exception>();
                }
            }
            catch (System.Exception ex)
            {
                Logger.LogException(ex);
            }

            return null;
        }

        public static bool TruncateExceptions()
        {
            bool success = false;

            try
            {
                using (CLINumberModelDataContext context = new CLINumberModelDataContext())
                {
                    List<CLINumberLibrary.Exception> currencyRate = new List<CLINumberLibrary.Exception>();
                    currencyRate = context.Exceptions.ToList<CLINumberLibrary.Exception>();
                    context.Exceptions.DeleteAllOnSubmit(currencyRate);
                    context.SubmitChanges();
                    success = true;
                }
            }
            catch (System.Exception ex)
            {
                Logger.LogException(ex);
            }
            return success;
        }
    }
}
