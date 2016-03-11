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
        DateTime? _dataTransformationDefinitionCacheLastCheck;
        DateTime? _dataRecordTypeCacheLastCheck;

        protected override bool ShouldSetCacheExpired()
        {
            return
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<DataRecordTypeManager.CacheManager>().IsCacheExpired(ref _dataRecordTypeCacheLastCheck) 
                |
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<DataTransformationDefinitionManager.CacheManager>().IsCacheExpired(ref _dataTransformationDefinitionCacheLastCheck);
        }
    }
}
