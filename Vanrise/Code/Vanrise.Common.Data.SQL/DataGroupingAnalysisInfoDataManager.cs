using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.SQL;

namespace Vanrise.Common.Data.SQL
{
    public class DataGroupingAnalysisInfoDataManager : BaseSQLDataManager, IDataGroupingAnalysisInfoDataManager
    {
        public DataGroupingAnalysisInfoDataManager()
            : base(GetConnectionStringName("DataGroupingDBConnStringKey", "DataGroupingDBConnString"))
        {

        }

        public bool TryGetAssignedServiceInstanceId(string dataAnalysisUniqueName, out Guid distributorServiceInstanceId)
        {
            object instanceIdAsObj = ExecuteScalarSP("common.sp_DataGroupingAnalysisInfo_GetDistributorServiceInstanceId", dataAnalysisUniqueName);
            if(instanceIdAsObj != null)
            {
                distributorServiceInstanceId = (Guid)instanceIdAsObj;
                return true;
            }
            else
            {
                distributorServiceInstanceId = default(Guid);
                return false;
            }
        }

        public Dictionary<Guid, int> GetDataAnalysisCountByServiceInstanceId()
        {
            Dictionary<Guid, int> countByServiceInstanceIds = new Dictionary<Guid, int>();
            ExecuteReaderSP("common.sp_DataGroupingAnalysisInfo_GetCountByDistributorServiceInstance",
                (reader) =>
                {
                    while(reader.Read())
                    {
                        countByServiceInstanceIds.Add((Guid)reader["DistributorServiceInstanceID"], (int)reader["NbOfDataAnalysis"]);
                    }
                });
            return countByServiceInstanceIds;
        }
        
        public void TryAssignServiceInstanceId(string dataAnalysisUniqueName, ref Guid distributorServiceInstanceId)
        {
            distributorServiceInstanceId = (Guid)ExecuteScalarSP("common.sp_DataGroupingAnalysisInfo_TryAssignServiceInstanceIdAndGet", dataAnalysisUniqueName, distributorServiceInstanceId);
        }

        public List<string> GetDataAnalysisNames(string dataAnalysisNamePrefix)
        {
            return GetItemsSP("[common].[sp_DataGroupingAnalysisInfo_GetAnalysisNamesByPrefix]", (reader) => reader["DataAnalysisName"] as string, dataAnalysisNamePrefix);
        }
    }
}
