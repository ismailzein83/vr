using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLINumberLibrary
{
    public class Logger
    {
        public static void LogException(System.Exception ex)
        {
            ExceptionRepository.Save(ex);
        }
    }
}
