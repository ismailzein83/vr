using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Common
{
    public static class HandleLoggerException
    {
        public static void WriteException(Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
    }
}
