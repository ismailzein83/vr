using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.Business
{
    public class VRNumberPrefixManager
    {
        static Guid beDefinitionId = new Guid("992C93A5-0D90-41CD-8501-7235B4CAB09E");

        public Guid? GetNumberPrefixTypeId(string number)
        {
            if (string.IsNullOrEmpty(number))
                return null;

            Dictionary<string, VRNumberPrefix> numberPrefixes = GetCachedNumberPrefixes();
            if (numberPrefixes == null)
                return null;

            StringCodeIterator stringCodeIterator = new StringCodeIterator(numberPrefixes.Keys);
            string matchingNumberPrefix = stringCodeIterator.GetLongestMatch(number);
            if (string.IsNullOrEmpty(matchingNumberPrefix))
                return null;

            VRNumberPrefix vrNumberPrefix;
            if (!numberPrefixes.TryGetValue(matchingNumberPrefix, out vrNumberPrefix))
                throw new Exception($"numberPrefixes does not contain number: '{matchingNumberPrefix}'");

            return vrNumberPrefix.Type;
        }

        private Dictionary<string, VRNumberPrefix> GetCachedNumberPrefixes()
        {
            IGenericBusinessEntityManager genericBusinessEntityManager = Vanrise.GenericData.Entities.BusinessManagerFactory.GetManager<IGenericBusinessEntityManager>();
            return genericBusinessEntityManager.GetCachedOrCreate("GetCachedNumberPrefixes", beDefinitionId, () =>
            {
                List<GenericBusinessEntity> genericBusinessEntities = genericBusinessEntityManager.GetAllGenericBusinessEntities(beDefinitionId);
                Dictionary<string, VRNumberPrefix> results = new Dictionary<string, VRNumberPrefix>();

                if (genericBusinessEntities != null)
                {
                    foreach (GenericBusinessEntity genericBusinessEntity in genericBusinessEntities)
                    {
                        if (genericBusinessEntity.FieldValues == null)
                            continue;

                        VRNumberPrefix vrNumberPrefix = new VRNumberPrefix()
                        {
                            ID = (int)genericBusinessEntity.FieldValues.GetRecord("Id"),
                            Number = genericBusinessEntity.FieldValues.GetRecord("Number") as string,
                            Type = (Guid)genericBusinessEntity.FieldValues.GetRecord("Type")
                        };
                        results.Add(vrNumberPrefix.Number, vrNumberPrefix);
                    }
                }

                return results.Count > 0 ? results : null;
            });
        }
    }
}