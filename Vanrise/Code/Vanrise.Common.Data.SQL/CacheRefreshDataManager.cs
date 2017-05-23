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
    public class CacheRefreshDataManager : BaseSQLDataManager, ICacheRefreshDataManager
    {
        #region Ctor/Properties

        public CacheRefreshDataManager()
            : base(GetConnectionStringName("ConfigurationDBConnStringKey", "ConfigurationDBConnString"))
        {

        }

        #endregion

        public List<Entities.CacheRefreshHandle> GetAll()
        {
            return GetItemsSP("[common].[sp_CacheRefreshHandle_GetAll]", CacheRefreshHandleMapper);
        }

        public bool AreUpdateHandlesEqual(ref object updateHandle, object newUpdateHandle)
        {
            return !base.IsDataUpdated(ref updateHandle, newUpdateHandle);
        }

        public void UpdateCacheTypeHandle(string cacheTypeName)
        {
            ExecuteNonQuerySP("[common].[sp_CacheRefreshHandle_Update]", cacheTypeName);
        }

        private CacheRefreshHandle CacheRefreshHandleMapper(IDataReader reader)
        {
            return new CacheRefreshHandle
            {
                TypeName = reader["CacheTypeName"] as string,
                LastUpdateInfo = (byte[])reader["timestamp"]
            };
        }


    }
}
