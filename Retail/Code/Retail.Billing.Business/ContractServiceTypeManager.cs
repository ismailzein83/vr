using Retail.Billing.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;
using Vanrise.Common;

namespace Retail.Billing.Business
{
    public class ContractServiceTypeManager
    {
        static readonly Guid s_BeDefinitionId = new Guid("7aa1c025-db5a-4e17-9958-354871b893db");
        static IGenericBusinessEntityManager s_genericBusinessEntityManager = Vanrise.GenericData.Entities.BusinessManagerFactory.GetManager<IGenericBusinessEntityManager>();

        public List<ContractServiceType> GetServiceTypes(Guid contractTypeId)
        {
            return GetServiceTypesByContractTypeId().GetRecord(contractTypeId);
        }

        public Dictionary<Guid, List<ContractServiceType>> GetServiceTypesByContractTypeId()
        {
            return GetCachedOrCreate("GetServiceTypesByContractTypeId", () =>
            {
                Dictionary<Guid, List<ContractServiceType>> serviceTypesByContractTypeId = new Dictionary<Guid, List<ContractServiceType>>();
                foreach(var serviceType in GetServiceTypes().Values)
                {
                    serviceTypesByContractTypeId.GetOrCreateItem(serviceType.ContractTypeId).Add(serviceType);
                }

                return serviceTypesByContractTypeId;
            });
        }

        public Dictionary<Guid, ContractServiceType> GetServiceTypes()
        {
            return GetCachedOrCreate("GetServiceTypes", () =>
            {
                Dictionary<Guid, ContractServiceType> serviceTypes = new Dictionary<Guid, ContractServiceType>();

                var entities = s_genericBusinessEntityManager.GetAllGenericBusinessEntities(s_BeDefinitionId, null, null);

                if (entities != null)
                {
                    foreach(var entity in entities)
                    {
                        var serviceType = ContractServiceTypeMapper(entity);

                        serviceTypes.Add(serviceType.ContractServiceTypeId, serviceType);
                    }
                }

                return serviceTypes;
            });
        }

        T GetCachedOrCreate<T>(object cacheName, Func<T> createObject)
        {
            return s_genericBusinessEntityManager.GetCachedOrCreate(cacheName, s_BeDefinitionId, createObject);
        }

        #region Mappers
        private ContractServiceType ContractServiceTypeMapper(GenericBusinessEntity genericBusinessEntity)
        {
            ContractServiceType serviceType = new ContractServiceType()
            {
                ContractServiceTypeId = (Guid)genericBusinessEntity.FieldValues.GetRecord("ID"),
                Name = genericBusinessEntity.FieldValues.GetRecord("Name") as string,
                ContractTypeId = (Guid)genericBusinessEntity.FieldValues.GetRecord("ContractType")
            };            

            return serviceType;
        }
        #endregion
    }
}
