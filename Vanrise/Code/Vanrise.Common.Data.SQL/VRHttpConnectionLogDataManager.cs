using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.SQL;

namespace Vanrise.Common.Data.SQL
{
    public class VRHttpConnectionLogDataManager : BaseSQLDataManager, IVRHttpConnectionLogDataManager
    {
        public VRHttpConnectionLogDataManager()
    : base(GetConnectionStringName("LoggingDBConnStringKey", "LogDBConnString"))
        {

        }
        public bool Insert(Guid VRHttpConnectionId, string BaseURL, string Path, string Parameters, string RequestHeaders, string RequestBody, DateTime RequestTime,
            string ResponseHeaders, string Response, DateTime? ResponseTime, HttpStatusCode? ResponseStatusCode, bool IsSucceded, string Exception)
        {
            return (ExecuteNonQuerySP("logging.sp_VRHttpConnectionLog_Insert", VRHttpConnectionId, BaseURL, Path, Parameters, RequestHeaders, RequestBody, RequestTime,
             ResponseHeaders, Response, ResponseTime, ResponseStatusCode, IsSucceded, Exception) > 0);
        }
    }
}
