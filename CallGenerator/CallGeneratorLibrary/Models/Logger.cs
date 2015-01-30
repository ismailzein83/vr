using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CallGeneratorLibrary.Repositories;

namespace CallGeneratorLibrary
{
    public class Logger
    {
        public static void LogAudit(NewActionLog actionLog)
        {
            AuditRepository.Save(actionLog);
        }

        public static void LogException(System.Exception ex)
        {
            ExceptionRepository.Save(ex);
        }

        public static void LogCompiler(string subject, string details)
        {
            ExceptionRepository.Save(subject, details);
        }
    }
}
