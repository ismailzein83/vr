using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;
using Vanrise.Common;
using TOne.WhS.Jazz.Entities;
namespace TOne.WhS.Jazz.Business
{
    public class WhsJazzCustomerTypeCodeManager
    {
        public static Guid _definitionId = new Guid("D1A82FF2-E1F4-4496-93D5-626BBDA9CDC9");
        GenericBusinessEntityManager _genericBusinessEntityManager = new GenericBusinessEntityManager();

        public List<WhSJazzCustomerTypeCode> GetAllCustomerTypes()
        {
            var records = GetCachedCustomerTypes();
            List<WhSJazzCustomerTypeCode> customerTypes = null;

            if (records != null && records.Count > 0)
            {
                customerTypes = new List<WhSJazzCustomerTypeCode>();
                foreach (var record in records)
                {
                    customerTypes.Add(record.Value);
                }
            }
            return customerTypes;
        }


        private Dictionary<Guid, WhSJazzCustomerTypeCode> GetCachedCustomerTypes()
        {
            GenericBusinessEntityManager genericBusinessEntityManager = new GenericBusinessEntityManager();
            return genericBusinessEntityManager.GetCachedOrCreate("GetCachedCustomerTypes", _definitionId, () =>
            {
                List<GenericBusinessEntity> genericBusinessEntities = genericBusinessEntityManager.GetAllGenericBusinessEntities(_definitionId);
                Dictionary<Guid, WhSJazzCustomerTypeCode> result = new Dictionary<Guid, WhSJazzCustomerTypeCode>();

                if (genericBusinessEntities != null)
                {
                    foreach (GenericBusinessEntity genericBusinessEntity in genericBusinessEntities)
                    {
                        if (genericBusinessEntity.FieldValues == null)
                            continue;

                        WhSJazzCustomerTypeCode customerType = new WhSJazzCustomerTypeCode()
                        {
                            ID = (Guid)genericBusinessEntity.FieldValues.GetRecord("ID"),
                            Name = (string)genericBusinessEntity.FieldValues.GetRecord("Name"),
                            Code = (string)genericBusinessEntity.FieldValues.GetRecord("Code"),
                            CreatedTime = (DateTime)genericBusinessEntity.FieldValues.GetRecord("CreatedTime"),
                            CreatedBy = (int)genericBusinessEntity.FieldValues.GetRecord("CreatedBy"),
                            LastModifiedTime = (DateTime)genericBusinessEntity.FieldValues.GetRecord("LastModifiedTime"),
                            LastModifiedBy = (int)genericBusinessEntity.FieldValues.GetRecord("LastModifiedBy")

                        };
                        result.Add(customerType.ID, customerType);
                    }
                }

                return result;
            });
        }       

       

    }

 
}
