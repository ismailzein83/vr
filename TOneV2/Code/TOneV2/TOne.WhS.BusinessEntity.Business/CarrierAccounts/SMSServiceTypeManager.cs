using System;
using System.Collections.Generic;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Common;
using Vanrise.GenericData.Entities;

namespace TOne.WhS.BusinessEntity.Business
{
    public class SMSServiceTypeManager
    {
        static Guid businessEntityDefinitionId = new Guid("E32B1008-F956-4055-8B94-48583E34AE83");

        #region Public Methods
        public SMSServiceType GetSMSServiceTypeById(int smsServiceTypeId)
        {
            return GetCachedSMSServicesType().GetRecord(smsServiceTypeId);
        }


        public IEnumerable<SMSServiceTypeInfo> GetSMSServicesTypeInfo(SMSServiceTypeFilter filter)
        {
            var smsServicesType = GetCachedSMSServicesType();
            Func<SMSServiceType, bool> filterExpression = (itm) =>
            {
                if (filter != null)
                    return false;

                return true;
            };
            return smsServicesType.MapRecords(SMSServiceTypeInfoMapper, filterExpression);
        }
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
        private SMSServiceTypeInfo SMSServiceTypeInfoMapper(SMSServiceType smsServiceType)
        {
            return new SMSServiceTypeInfo()
            {
                SMSServiceTypeInfoId = smsServiceType.SMSServiceTypeId,
                Symbol = smsServiceType.Symbol,
            };
        }
        #endregion
    }
}
