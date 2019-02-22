using System;
using System.Collections.Generic;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Common;
using Vanrise.GenericData.Entities;

namespace TOne.WhS.BusinessEntity.Business
{
    public class TechnicalNumberPlanManager
    {
        static readonly Guid BeDefinitionId = new Guid("660bf005-68c2-4d17-8f0a-8c2015e4d8be");

        public List<TechnicalNumberPlanSettings> GetTechnicalNumberPlans()
        {
            IGenericBusinessEntityManager genericBusinessEntityManager = Vanrise.GenericData.Entities.BusinessManagerFactory.GetManager<IGenericBusinessEntityManager>();
            return genericBusinessEntityManager.GetCachedOrCreate("GetTechnicalNumberPlans", BeDefinitionId, () =>
            {
                List<GenericBusinessEntity> genericBusinessEntities = genericBusinessEntityManager.GetAllGenericBusinessEntities(BeDefinitionId);
                List<TechnicalNumberPlanSettings> technicalNumberPlans= new List<TechnicalNumberPlanSettings>();

                if (genericBusinessEntities != null)
                {
                    foreach (GenericBusinessEntity genericBusinessEntity in genericBusinessEntities)
                    {
                        if (genericBusinessEntity.FieldValues == null)
                            continue;

                        TechnicalNumberPlanSettings technicalNumberPlan = new TechnicalNumberPlanSettings
                        {
                             Codes =(List<ZoneCode>) genericBusinessEntity.FieldValues.GetRecord("Codes")
                        };
                        technicalNumberPlans.Add(technicalNumberPlan);
                    }
                }
                return technicalNumberPlans;
            });
        }
    }
}
