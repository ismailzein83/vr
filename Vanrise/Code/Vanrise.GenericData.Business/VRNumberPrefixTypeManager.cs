using System;
using System.Collections.Generic;
using Vanrise.Common;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.Business
{
    public class VRNumberPrefixTypeManager
    {
        static Guid beDefinitionId = new Guid("fe36264a-1db1-4c1d-9228-036a43195d4d");

        public Dictionary<Guid, VRNumberPrefixType> GetNumberPrefixTypes()
        {
            return GetCachedNumberPrefixTypes();
        }

        private Dictionary<Guid, VRNumberPrefixType> GetCachedNumberPrefixTypes()
        {
            IGenericBusinessEntityManager genericBusinessEntityManager = Vanrise.GenericData.Entities.BusinessManagerFactory.GetManager<IGenericBusinessEntityManager>();

            return genericBusinessEntityManager.GetCachedOrCreate("GetCachedNumberPrefixTypes", beDefinitionId, () =>
            {
                Dictionary<Guid, VRNumberPrefixType> results = new Dictionary<Guid, VRNumberPrefixType>();

                List<GenericBusinessEntity> genericBusinessEntities = genericBusinessEntityManager.GetAllGenericBusinessEntities(beDefinitionId);
                if (genericBusinessEntities == null)
                    return results;

                foreach (GenericBusinessEntity genericBusinessEntity in genericBusinessEntities)
                {
                    if (genericBusinessEntity.FieldValues == null)
                        continue;

                    VRNumberPrefixType vrNumberPrefixType = new VRNumberPrefixType()
                    {
                        ID = (Guid)genericBusinessEntity.FieldValues.GetRecord("Id"),
                        Name = genericBusinessEntity.FieldValues.GetRecord("Name") as string
                    };
                    results.Add(vrNumberPrefixType.ID, vrNumberPrefixType);
                }

                return results;
            });
        }
    }
}