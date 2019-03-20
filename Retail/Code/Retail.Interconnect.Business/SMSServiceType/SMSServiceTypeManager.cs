using Retail.Interconnect.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
using Vanrise.GenericData.Entities;

namespace Retail.Interconnect.Business
{
    public class SMSServiceTypeManager
    {
        static Guid businessEntityDefinitionId = new Guid("D5153143-A6EF-44D1-A6F8-FEA6B399D853");

        #region Public Methods
        #endregion

        #region Private Methods
        private Dictionary<int, SMSServiceType> GetCachedSMSServicesType()
        {
            IGenericBusinessEntityManager genericBusinessEntityManager = Vanrise.GenericData.Entities.BusinessManagerFactory.GetManager<IGenericBusinessEntityManager>();
            return genericBusinessEntityManager.GetCachedOrCreate("GetCachedSMSServicesType", businessEntityDefinitionId, () =>
            {
                Dictionary<int, SMSServiceType> result = new Dictionary<int, SMSServiceType>();
                IEnumerable<GenericBusinessEntity> genericBusinessEntities = genericBusinessEntityManager.GetAllGenericBusinessEntities(businessEntityDefinitionId);
                if (genericBusinessEntities != null)
                {
                    foreach (GenericBusinessEntity genericBusinessEntity in genericBusinessEntities)
                    {
                        SMSServiceType smsServiceType = new SMSServiceType()
                        {
                            SMSServiceTypeId = (int)genericBusinessEntity.FieldValues.GetRecord("ID"),
                            Name = genericBusinessEntity.FieldValues.GetRecord("Name") as string,
                            Symbol = genericBusinessEntity.FieldValues.GetRecord("Symbol") as string,
                            CreatedTime = (DateTime)genericBusinessEntity.FieldValues.GetRecord("CreatedTime"),
                            LastModifiedTime = (DateTime)genericBusinessEntity.FieldValues.GetRecord("LastModifiedTime"),
                            CreatedBy = (int)genericBusinessEntity.FieldValues.GetRecord("CreatedBy"),
                            LastModifiedBy = (int)genericBusinessEntity.FieldValues.GetRecord("LastModifiedBy"),
                        };
                        result.Add(smsServiceType.SMSServiceTypeId, smsServiceType);
                    }
                }
                return result;
            });
        }
        #endregion


        #region Mappers
        #endregion
    }
}
