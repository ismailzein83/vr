using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Common.Data
{
    public interface IBigDataServiceDataManager : IDataManager
    {
        bool Insert(string serviceUrl, int runtimeProcessId, out long id);

        void Update(long bigDataServiceId, long totalRecordsCount, IEnumerable<Guid> cachedObjectIds);

        bool AreBigDataServicesUpdated(ref object updateHandle);

        void DeleteTimedOutServices(IEnumerable<int> notInRunningProcessIds);

        IEnumerable<Entities.BigDataService> GetAll();
    }
}
