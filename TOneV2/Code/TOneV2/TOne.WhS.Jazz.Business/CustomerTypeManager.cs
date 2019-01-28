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
    public class CustomerTypeManager
    {
        public static Guid _definitionId = new Guid("D1A82FF2-E1F4-4496-93D5-626BBDA9CDC9");
        GenericBusinessEntityManager _genericBusinessEntityManager = new GenericBusinessEntityManager();

        public List<CustomerType> GetAllCustomerTypes()
        {
            var records = GetCachedCustomerTypes();
            List<CustomerType> customerTypes = null;

            if (records != null && records.Count > 0)
            {
                customerTypes = new List<CustomerType>();
                foreach (var record in records)
                {
                    customerTypes.Add(record.Value);
                }
            }
            return customerTypes;
        }


        private Dictionary<Guid, CustomerType> GetCachedCustomerTypes()
        {
            GenericBusinessEntityManager genericBusinessEntityManager = new GenericBusinessEntityManager();
            return genericBusinessEntityManager.GetCachedOrCreate("GetCachedCustomerTypes", _definitionId, () =>
            {
                List<GenericBusinessEntity> genericBusinessEntities = genericBusinessEntityManager.GetAllGenericBusinessEntities(_definitionId);
                Dictionary<Guid, CustomerType> result = new Dictionary<Guid, CustomerType>();

                if (genericBusinessEntities != null)
                {
                    foreach (GenericBusinessEntity genericBusinessEntity in genericBusinessEntities)
                    {
                        if (genericBusinessEntity.FieldValues == null)
                            continue;

                        CustomerType customerType = new CustomerType()
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

        public IEnumerable<CustomerTypeDetail> GetCustomerTypesInfo(CustomerTypeInfoFilter filter)
        {
            var customerTypes = GetCachedCustomerTypes();
            Func<CustomerType, bool> filterFunc = (customerType) =>
            {
                if (filter != null)
                {
                    if (filter.Filters != null && filter.Filters.Count() > 0)
                    {
                        var context = new CustomerTypeFilterContext
                        {
                            CustomerType = customerType
                        };
                        foreach (var customerTypeFilter in filter.Filters)
                        {
                            if (!customerTypeFilter.IsMatch(context))
                                return false;
                        }
                    }
                }
                return true;
            };
            return customerTypes.MapRecords((record) =>
            {
                return CustomerTypeInfoMapper(record);
            }, filterFunc);

        }
        private CustomerTypeDetail CustomerTypeInfoMapper(CustomerType customerType)
        {
            return new CustomerTypeDetail
            {
                ID = customerType.ID,
                Name = customerType.Name
            };
        }

    }

 
}
