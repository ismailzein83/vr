using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Integration.Business
{
    public class DataSourceLogger : Entities.IDataSourceLogger
    {
        public void LogEntry(Vanrise.Common.LogEntryType logEntryType, string message)
        {
            throw new NotImplementedException();
        }
    }
}
