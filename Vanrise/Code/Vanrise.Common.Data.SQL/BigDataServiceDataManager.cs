using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.SQL;
using Vanrise.Entities;

namespace Vanrise.Common.Data.SQL
{
    public class BigDataServiceDataManager : BaseSQLDataManager, IBigDataServiceDataManager
    {
        public BigDataServiceDataManager()
            : base(GetConnectionStringName("BigDataServiceDBConnStringKey", "BigDataServiceDBConnString"))
        {

        }

        public bool Insert(string serviceUrl, int runtimeProcessId, out long id)
        {
            Object idAsObject;
            if (ExecuteNonQuerySP("common.sp_BigDataService_Insert", out idAsObject, serviceUrl, runtimeProcessId) > 0)
            {
                id = (long)idAsObject;
                return true;
            }
            else
            {
                id = 0;
                return false;
            }
        }

        public void Update(long bigDataServiceId, long totalRecordsCount, IEnumerable<Guid> cachedObjectIds)
        {
            ExecuteNonQuerySP("common.sp_BigDataService_Update", bigDataServiceId, totalRecordsCount, cachedObjectIds != null ? String.Join(",", cachedObjectIds) : null);
        }

        public bool AreBigDataServicesUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("[common].[BigDataService]", ref updateHandle);
        }

        public void DeleteTimedOutServices(IEnumerable<int> notInRunningProcessIds)
        {
            ExecuteNonQuerySP("common.sp_BigDataService_DeleteTimedOut", String.Join(",", notInRunningProcessIds));
        }

        public IEnumerable<Entities.BigDataService> GetAll()
        {
            return GetItemsSP("common.sp_BigDataService_GetAll", BigDataServiceMapper);
        }

        #region Mappers

        BigDataService BigDataServiceMapper(IDataReader reader)
        {
            BigDataService instance = new BigDataService
            {
                BigDataServiceId = (long)reader["ID"],
                URL = reader["ServiceURL"] as string,
                TotalCachedRecordsCount = GetReaderValue<long>(reader, "TotalCachedRecordsCount"),
                 CachedObjectIds = new HashSet<Guid>()
            };
            string serializedCachedObjectIds = reader["CachedObjectIds"] as string;
            if(serializedCachedObjectIds != null)
            {
                foreach( var cachedObjectIdString in serializedCachedObjectIds.Split(','))
                {
                    if (!String.IsNullOrEmpty(cachedObjectIdString))
                        instance.CachedObjectIds.Add(Guid.Parse(cachedObjectIdString));
                }
            }
            return instance;
        }

        #endregion
    }
}
