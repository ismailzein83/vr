using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.Common.Data
{
    public interface ICacheRefreshDataManager : IDataManager
    {
        List<CacheRefreshHandle> GetAll();

        bool AreUpdateHandlesEqual(ref object updateHandle, object newUpdateHandle);

        void UpdateCacheTypeHandle(string cacheTypeName);
    }
}
