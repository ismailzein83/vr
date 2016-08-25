using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Common.Data
{
    public interface IDataGroupingAnalysisInfoDataManager : IDataManager
    {
        bool TryGetAssignedServiceInstanceId(string dataAnalysisUniqueName, out Guid distributorServiceInstanceId);

        Dictionary<Guid, int> GetDataAnalysisCountByServiceInstanceId();

        void TryAssignServiceInstanceId(string dataAnalysisUniqueName, ref Guid distributorServiceInstanceId);

        List<string> GetDataAnalysisNames(string dataAnalysisNamePrefix);
    }
}
