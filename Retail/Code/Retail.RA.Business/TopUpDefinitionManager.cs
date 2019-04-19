using Retail.RA.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;
using Vanrise.Common;

namespace Retail.RA.Business
{
    public class TopUpDefinitionManager
    {
        static Guid s_topUpDefinitionBEDefinitionId = new Guid("6ab8d220-b8f8-469f-a40b-00f86a374154");

        public TopUpDefinition GetTopUpDefinition(int topUpDefinitionId)
        {
            var topUpDefinitions = GetCachedTopUpDefinitions();
            if (topUpDefinitions == null || topUpDefinitions.Count == 0)
                return null;
            return topUpDefinitions.GetRecord(topUpDefinitionId);
        }
        public int? GetTopUpTypeId(string sourceID, long operatorId)
        {
            var topUpDefinitions = GetCachedTopUpDefinitions();
            if (topUpDefinitions == null || topUpDefinitions.Count == 0)
            {
                return null;
            }
            else
            {
                foreach(var definition in topUpDefinitions.Values)
                {
                    if (definition.Settings != null && definition.Settings.OperatorID == operatorId && definition.Settings.SourceID == sourceID)
                    {
                        return definition.ID;
                    }
                }
                return null;
            }
        }

        #region Private Methods
        private Dictionary<int, TopUpDefinition> GetCachedTopUpDefinitions()
        {
            IGenericBusinessEntityManager genericBusinessEntityManager = Vanrise.GenericData.Entities.BusinessManagerFactory.GetManager<IGenericBusinessEntityManager>();
            return genericBusinessEntityManager.GetCachedOrCreate("GetCachedTopUpDefinitions", s_topUpDefinitionBEDefinitionId, () =>
            {
                Dictionary<int, TopUpDefinition> result = new Dictionary<int, TopUpDefinition>();
                IEnumerable<GenericBusinessEntity> genericBusinessEntities = genericBusinessEntityManager.GetAllGenericBusinessEntities(s_topUpDefinitionBEDefinitionId, null);
                if (genericBusinessEntities != null)
                {
                    foreach (GenericBusinessEntity genericBusinessEntity in genericBusinessEntities)
                    {
                        if (genericBusinessEntity.FieldValues != null)
                        {
                            TopUpDefinition topUpDefinition = new TopUpDefinition()
                            {
                                ID = (int)genericBusinessEntity.FieldValues.GetRecord("ID"),
                                Name = genericBusinessEntity.FieldValues.GetRecord("Name") as string,
                                Settings = new TopUpDefinitionSettings()
                                {
                                    Amount = (decimal)genericBusinessEntity.FieldValues.GetRecord("Amount"),
                                    DoesCreditExpire = (bool)genericBusinessEntity.FieldValues.GetRecord("DoesCreditExpire"),
                                    DuePeriod = (int)genericBusinessEntity.FieldValues.GetRecord("DuePeriod"),
                                    OperatorID = (long)genericBusinessEntity.FieldValues.GetRecord("Operator"),
                                    SourceID = genericBusinessEntity.FieldValues.GetRecord("SourceID") as string
                                }
                            };
                            result.Add(topUpDefinition.ID, topUpDefinition);
                        }
                    }
                }
                return result;
            });
        }
        #endregion
    }
}
