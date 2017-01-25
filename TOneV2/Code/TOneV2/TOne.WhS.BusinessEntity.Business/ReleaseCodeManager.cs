using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;
using Vanrise.Common;

namespace TOne.WhS.BusinessEntity.Business
{
    public class ReleaseCodeManager
    {
        static Guid s_businessEntityDefinitionId = new Guid("4652abe7-81f7-4129-a222-31933747018d");
        public string GetReleaseCodeDescription(string code)
        {
            return GetRCDescriptionsByCodes().GetRecord(code);
        }

        Dictionary<string, string> GetRCDescriptionsByCodes()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<GenericBusinessEntityManager.CacheManager>().GetOrCreateObject("GetRCDescriptionsByCodes", s_businessEntityDefinitionId,
                () =>
                {
                    Dictionary<string, string> descriptionsByCode = new Dictionary<string, string>();
                    Dictionary<long, GenericBusinessEntity> allReleaseCodes = new GenericBusinessEntityManager().GetCachedGenericBusinessEntities(s_businessEntityDefinitionId);
                    if(allReleaseCodes != null)
                    {
                        foreach(var releaseCode in allReleaseCodes.Values)
                        {
                            if (releaseCode.Details != null)
                            {
                                string code = Utilities.GetPropValueReader("ReleaseCode").GetPropertyValue(releaseCode.Details) as string;
                                string description = Utilities.GetPropValueReader("Description").GetPropertyValue(releaseCode.Details) as string;
                                if (code != null && !descriptionsByCode.ContainsKey(code))
                                    descriptionsByCode.Add(code, description);
                            }
                        }
                    }
                    return descriptionsByCode;
                });
        }
    }
}
