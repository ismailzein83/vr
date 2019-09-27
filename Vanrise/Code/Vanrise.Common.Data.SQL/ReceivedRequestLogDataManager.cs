using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.SQL;

namespace Vanrise.Common.Data.SQL
{
    public class ReceivedRequestLogDataManager : BaseSQLDataManager, IReceivedRequestLogDataManager
    {

        #region ctor/Local Variables
        public ReceivedRequestLogDataManager(): base(GetConnectionStringName("LoggingDBConnStringKey", "LogDBConnString"))
        { }
        #endregion

        #region Public Methods

        public void Insert(string actionName, string method, string moduleName, string controllerName, string uri, string path, string requestHeader, string arguments, string requestBody, string responseHeader, string responseStatusCode, bool isSucceded, string bodyResponse, DateTime startDateTime, int? userId)
        {
            ExecuteNonQuerySP("logging.sp_ReceivedRequestLog_Insert", actionName, method, moduleName, controllerName, uri, path, requestHeader, arguments, requestBody, responseHeader, responseStatusCode, isSucceded, bodyResponse, startDateTime, userId);
        }
        #endregion
    }
}
