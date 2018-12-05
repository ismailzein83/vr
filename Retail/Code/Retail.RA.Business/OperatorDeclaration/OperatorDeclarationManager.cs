using System;
using Vanrise.Common;
using Retail.RA.Entities;
using System.Collections.Generic;
using Vanrise.GenericData.Entities;

namespace Retail.RA.Business
{
    public class OperatorDeclarationManager
    {
        static Guid beDefinitionId = new Guid("6E73A6E0-B3E2-4930-86ED-9EF509D56960");
        private Dictionary<long, OperatorDeclaration> GetCachedOperatorDeclarations()
        {
            IGenericBusinessEntityManager genericBusinessEntityManager = Vanrise.GenericData.Entities.BusinessManagerFactory.GetManager<IGenericBusinessEntityManager>();
            return genericBusinessEntityManager.GetCachedOrCreate("GetCachedOperatorDeclarations", beDefinitionId, () =>
            {
                List<GenericBusinessEntity> genericBusinessEntities = genericBusinessEntityManager.GetAllGenericBusinessEntities(beDefinitionId);
                Dictionary<long, OperatorDeclaration> operatorDeclarationByPeriod = new Dictionary<long, OperatorDeclaration>();
                Dictionary<int, OperatorDeclaration> operatorDeclarationById = new Dictionary<int, OperatorDeclaration>();

                if (genericBusinessEntities != null)
                {
                    foreach (GenericBusinessEntity genericBusinessEntity in genericBusinessEntities)
                    {
                        if (genericBusinessEntity.FieldValues == null)
                            continue;

                        OperatorDeclaration operatorDeclaration = new OperatorDeclaration
                        {
                            ID = (int)genericBusinessEntity.FieldValues.GetRecord("ID"),
                            OperatorId = (int)genericBusinessEntity.FieldValues.GetRecord("OperatorId")
                        };
                        operatorDeclarationByPeriod.Add(operatorDeclaration.ID, operatorDeclaration);
                    }
                }

                return operatorDeclarationByPeriod;
            });
        }
    }
}
