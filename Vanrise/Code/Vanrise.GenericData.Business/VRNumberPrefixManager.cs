using System;
using System.Collections.Generic;
using Vanrise.Common;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.Business
{
    public class VRNumberPrefixManager
    {
        static Guid beDefinitionId = new Guid("992C93A5-0D90-41CD-8501-7235B4CAB09E");

        public Guid? GetNumberPrefixTypeId(string number, bool isExactMatch = false)
        {
            if (string.IsNullOrEmpty(number))
                return null;

            Dictionary<string, VRNumberPrefix> numberPrefixes = GetCachedNumberPrefixes(isExactMatch);
            if (numberPrefixes == null || numberPrefixes.Count == 0)
                return null;

            VRNumberPrefix vrNumberPrefix;

            if (!isExactMatch)
            {
                StringCodeIterator stringCodeIterator = new StringCodeIterator(numberPrefixes.Keys);
                string matchingNumberPrefix = stringCodeIterator.GetLongestMatch(number);
                if (string.IsNullOrEmpty(matchingNumberPrefix))
                    return null;

                if (!numberPrefixes.TryGetValue(matchingNumberPrefix, out vrNumberPrefix))
                    throw new Exception($"numberPrefixes does not contain number: '{matchingNumberPrefix}'");
            }
            else
            {
                if (!numberPrefixes.TryGetValue(number, out vrNumberPrefix))
                    return null;
            }

            return vrNumberPrefix.Type;
        }

        private Dictionary<string, VRNumberPrefix> GetCachedNumberPrefixes(bool isExactMatch)
        {
            string cacheName = isExactMatch ? "GetCachedShortNumbers" : "GetCachedNumberPrefixes";

            IGenericBusinessEntityManager genericBusinessEntityManager = Vanrise.GenericData.Entities.BusinessManagerFactory.GetManager<IGenericBusinessEntityManager>();
            return genericBusinessEntityManager.GetCachedOrCreate(cacheName, beDefinitionId, () =>
            {
                Dictionary<string, VRNumberPrefix> results = new Dictionary<string, VRNumberPrefix>();

                List<GenericBusinessEntity> genericBusinessEntities = genericBusinessEntityManager.GetAllGenericBusinessEntities(beDefinitionId);
                if (genericBusinessEntities == null)
                    return results;

                foreach (GenericBusinessEntity genericBusinessEntity in genericBusinessEntities)
                {
                    if (genericBusinessEntity.FieldValues == null)
                        continue;

                    bool isExact = (bool)genericBusinessEntity.FieldValues.GetRecord("IsExact");
                    if (isExact != isExactMatch)
                        continue;

                    VRNumberPrefix vrNumberPrefix = new VRNumberPrefix()
                    {
                        ID = (int)genericBusinessEntity.FieldValues.GetRecord("Id"),
                        Number = genericBusinessEntity.FieldValues.GetRecord("Number") as string,
                        Type = (Guid)genericBusinessEntity.FieldValues.GetRecord("Type"),
                        IsExact = isExact
                    };
                    results.Add(vrNumberPrefix.Number, vrNumberPrefix);
                }

                return results;
            });
        }
    }
}