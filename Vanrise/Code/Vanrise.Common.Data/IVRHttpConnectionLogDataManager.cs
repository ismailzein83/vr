using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Common.Data
{
    public interface IVRHttpConnectionLogDataManager : IDataManager
    {
        bool Insert(Guid VRHttpConnectionId, string BaseURL, string Path, string Parameters, string RequestHeaders, string RequestBody, DateTime RequestTime,
            string ResponseHeaders, string Response, DateTime? ResponseTime, HttpStatusCode? ResponseStatusCode, bool IsSucceded, string Exception);
    }
}
