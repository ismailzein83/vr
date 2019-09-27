using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Common.Data
{
    public interface IReceivedRequestLogDataManager : IDataManager
    {
        void Insert(string actionName, string method, string moduleName, string controllerName, string uri, string path, string requestHeader, string arguments, string requestBody, string responseHeader,
                             string responseStatusCode, bool isSucceded, string bodyResponse, DateTime startDateTime, int? userId);
    }
}
