using System;
using Vanrise.Common;
using Retail.RA.Entities;
using System.Collections.Generic;
using Vanrise.GenericData.Entities;

namespace Retail.RA.Business
{
    public class PeriodDefinitionManager
    {
        static Guid beDefinitionId = new Guid("74A31C7E-E38C-46DE-960C-97A38A7E7107");

        #region Public Methods
        public int? GetPeriodDefinitionIdByDay(DateTime day)
        {
            var cachedPeriodDefinition = GetCachedPeriodDefinitions();

            if (cachedPeriodDefinition == null)
                return null;

            foreach (var periodDefinition in cachedPeriodDefinition.Values)
            {
                if (day >= periodDefinition.FromDate && day < periodDefinition.ToDate)
                    return periodDefinition.PeriodDefinitionId;
            }

            return null;
        }

        #endregion

        #region Private Methods
        private Dictionary<int, PeriodDefinition> GetCachedPeriodDefinitions()
        {
            IGenericBusinessEntityManager genericBusinessEntityManager = Vanrise.GenericData.Entities.BusinessManagerFactory.GetManager<IGenericBusinessEntityManager>();
            return genericBusinessEntityManager.GetCachedOrCreate("GetCachedPeriodDefinitions", beDefinitionId, () =>
            {
                List<GenericBusinessEntity> genericBusinessEntities = genericBusinessEntityManager.GetAllGenericBusinessEntities(beDefinitionId);
                Dictionary<int, PeriodDefinition> periodDefinitionById = new Dictionary<int, PeriodDefinition>();

                if (genericBusinessEntities != null)
                {
                    foreach (GenericBusinessEntity genericBusinessEntity in genericBusinessEntities)
                    {
                        if (genericBusinessEntity.FieldValues == null)
                            continue;

                        PeriodDefinition periodDefinition = new PeriodDefinition
                        {
                            PeriodDefinitionId = (int)genericBusinessEntity.FieldValues.GetRecord("ID"),
                            Name = genericBusinessEntity.FieldValues.GetRecord("Name") as string,
                            FromDate = (DateTime)genericBusinessEntity.FieldValues.GetRecord("From"),
                            ToDate = (DateTime)genericBusinessEntity.FieldValues.GetRecord("To")
                        };
                        periodDefinitionById.Add(periodDefinition.PeriodDefinitionId, periodDefinition);
                    }
                }

                return periodDefinitionById;
            });
        }

        #endregion
    }
}
