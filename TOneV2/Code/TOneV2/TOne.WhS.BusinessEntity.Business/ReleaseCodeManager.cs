using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;
using Vanrise.Common;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Business
{
    public class SwitchReleaseCodeManager
    {
        static Guid s_businessEntityDefinitionId = new Guid("4652abe7-81f7-4129-a222-31933747018d");
        public string GetReleaseCodeDescription(string code)
        {
            string description = null;
            if (code!=null)
                description =  GetRCDescriptionsByCodes().GetRecord(code);
            return description;
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

        public List<SwitchReleaseCause> GetReleaseCausesByCode(string code, int? switchId)
        {
            List<SwitchReleaseCause> releaseCauses = new List<SwitchReleaseCause>();
            var allReleaseCauses = GetAllReleaseCauses();
            if(allReleaseCauses != null)
            {
                releaseCauses.AddRange(allReleaseCauses.Values.FindAllRecords(itm => itm.ReleaseCode == code && (!switchId.HasValue || itm.SwitchId == switchId.Value)));
            }
            return releaseCauses;
        }

        Dictionary<int, SwitchReleaseCause> GetAllReleaseCauses()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<GenericBusinessEntityManager.CacheManager>().GetOrCreateObject("GetAllReleaseCauses", s_businessEntityDefinitionId,
                () =>
                {
                    Dictionary<int, SwitchReleaseCause> releaseCauses = new Dictionary<int, SwitchReleaseCause>();
                    Dictionary<long, GenericBusinessEntity> releaseCausesGeneric = new GenericBusinessEntityManager().GetCachedGenericBusinessEntities(s_businessEntityDefinitionId);
                    if (releaseCausesGeneric != null)
                    {
                        foreach (var releaseCauseGeneric in releaseCausesGeneric.Values)
                        {
                            if (releaseCauseGeneric.Details != null)
                            {
                                SwitchReleaseCause releaseCause = new SwitchReleaseCause
                                {
                                    ReleaseCode = releaseCauseGeneric.Details.ReleaseCode,
                                    SwitchId = releaseCauseGeneric.Details.SwitchId,
                                    SwitchReleaseCauseId = (int)releaseCauseGeneric.GenericBusinessEntityId,
                                    Settings = new SwitchReleaseCauseSetting
                                    {
                                        Description = releaseCauseGeneric.Details.Description,
                                        IsDelivered = releaseCauseGeneric.Details.IsDelivered
                                    }
                                };
                                releaseCauses.Add(releaseCause.SwitchReleaseCauseId, releaseCause);
                            }
                        }
                    }
                    return releaseCauses;
                });
        }
    }
}
