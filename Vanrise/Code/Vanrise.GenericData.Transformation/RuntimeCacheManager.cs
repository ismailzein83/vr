using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Business;

namespace Vanrise.GenericData.Transformation
{
    public class RuntimeCacheManager : Vanrise.Caching.BaseCacheManager
    {
        DataTransformationDefinitionManager.CacheManager _dataTransformationDefinitionCacheManager = 
            Vanrise.Caching.CacheManagerFactory.GetCacheManager<DataTransformationDefinitionManager.CacheManager>(Guid.NewGuid());
        DataRecordTypeManager.CacheManager _dataRecordTypeCacheManager = 
            Vanrise.Caching.CacheManagerFactory.GetCacheManager<DataRecordTypeManager.CacheManager>(Guid.NewGuid());
        protected override bool ShouldSetCacheExpired()
        {
            return _dataRecordTypeCacheManager.IsCacheExpired() || _dataTransformationDefinitionCacheManager.IsCacheExpired();
        }
    }
}
