using RecordAnalysis.Entities;
using System;
using System.Collections.Generic;
using Vanrise.Common;
using Vanrise.GenericData.Entities;

namespace RecordAnalysis.Business
{
    public class C4InterconnectionManager
    {
        public static readonly Guid BeDefinitionId = new Guid("aac65f3f-c7de-4230-8bab-a5a1a83086fe");

        public C4Interconnection GetC4Interconnection(int c4InterconnectionId)
        {
            var cachedC4Interconnections = GetCachedC4Interconnections();
            if (cachedC4Interconnections == null)
                return null;

            return cachedC4Interconnections.GetRecord(c4InterconnectionId);
        }

        private Dictionary<int, C4Interconnection> GetCachedC4Interconnections()
        {
            IGenericBusinessEntityManager genericBusinessEntityManager = Vanrise.GenericData.Entities.BusinessManagerFactory.GetManager<IGenericBusinessEntityManager>();
            return genericBusinessEntityManager.GetCachedOrCreate("GetCachedC4Interconnections", BeDefinitionId, () =>
            {
                List<GenericBusinessEntity> genericBusinessEntities = genericBusinessEntityManager.GetAllGenericBusinessEntities(BeDefinitionId);

                var c4Interconnections = new Dictionary<int, C4Interconnection>();

                if (genericBusinessEntities != null)
                {
                    foreach (GenericBusinessEntity genericBusinessEntity in genericBusinessEntities)
                    {
                        C4Interconnection item = C4InterconnectionEntityMapper(genericBusinessEntity.FieldValues);
                        if (item == null)
                            continue;

                        c4Interconnections.Add(item.InterconnectionId, item);
                    }
                }
                return c4Interconnections;
            });
        }

        private C4Interconnection C4InterconnectionEntityMapper(Dictionary<string, object> fieldValues)
        {
            if (fieldValues == null)
                return null;

            var entity = new C4Interconnection()
            {
                InterconnectionId = (int)fieldValues.GetRecord("Id"),
                Name = fieldValues.GetRecord("Name") as string,
            };

            return entity;
        }
    }
}