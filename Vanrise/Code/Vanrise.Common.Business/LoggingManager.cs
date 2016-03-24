using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Logging.SQL;

namespace Vanrise.Common.Business
{
    public class LoggingManager
    {
        SQLDataManager GetDataManager()
        {
            var logHandlers = LoggerFactory.GetLogger().LogHandlers;
            if(logHandlers != null)
            {
                foreach(var handler in logHandlers)
                {
                    SQLLogger sqlLogger = handler as SQLLogger;
                    if (sqlLogger != null)
                        return sqlLogger.DataManager;
                }
            }
            return null;
        }
    }
}
