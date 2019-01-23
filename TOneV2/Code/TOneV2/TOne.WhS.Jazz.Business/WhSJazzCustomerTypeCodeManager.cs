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
    public class WhSJazzCustomerTypeCodeManager
    {
        public static Guid _definitionId = new Guid("D1A82FF2-E1F4-4496-93D5-626BBDA9CDC9");
        GenericBusinessEntityManager _genericBusinessEntityManager = new GenericBusinessEntityManager();

        public List<WhSJazzCustomerTypeCode> GetAllCustomerTypeCodes()
        {
            var records = GetCachedCustomerTypeCodes();
            List<WhSJazzCustomerTypeCode> customerTypeCodes = null;

            if (records != null && records.Count > 0)
            {
                customerTypeCodes = new List<WhSJazzCustomerTypeCode>();
                foreach (var record in records)
                {
                    customerTypeCodes.Add(record.Value);
                }
            }
            return customerTypeCodes;
        }


        private Dictionary<Guid, WhSJazzCustomerTypeCode> GetCachedCustomerTypeCodes()
        {
            GenericBusinessEntityManager genericBusinessEntityManager = new GenericBusinessEntityManager();
            return genericBusinessEntityManager.GetCachedOrCreate("GetCachedCustomerTypeCodes", _definitionId, () =>
            {
                List<GenericBusinessEntity> genericBusinessEntities = genericBusinessEntityManager.GetAllGenericBusinessEntities(_definitionId);
                Dictionary<Guid, WhSJazzCustomerTypeCode> result = new Dictionary<Guid, WhSJazzCustomerTypeCode>();

                if (genericBusinessEntities != null)
                {
                    foreach (GenericBusinessEntity genericBusinessEntity in genericBusinessEntities)
                    {
                        if (genericBusinessEntity.FieldValues == null)
                            continue;

                        WhSJazzCustomerTypeCode customerTypeCode = new WhSJazzCustomerTypeCode()
                        {
                            ID = (Guid)genericBusinessEntity.FieldValues.GetRecord("ID"),
                            Name = (string)genericBusinessEntity.FieldValues.GetRecord("Name"),
                            Code = (string)genericBusinessEntity.FieldValues.GetRecord("Code"),
                            CreatedTime = (DateTime)genericBusinessEntity.FieldValues.GetRecord("CreatedTime"),
                            CreatedBy = (int)genericBusinessEntity.FieldValues.GetRecord("CreatedBy"),
                            LastModifiedTime = (DateTime)genericBusinessEntity.FieldValues.GetRecord("LastModifiedTime"),
                            LastModifiedBy = (int)genericBusinessEntity.FieldValues.GetRecord("LastModifiedBy")

                        };
                        result.Add(customerTypeCode.ID, customerTypeCode);
                    }
                }

                return result;
            });
        }

        public IEnumerable<WhSJazzCustomerTypeCodeDetail> GetCustomerTypeCodesInfo(WhSJazzCustomerTypeCodeInfoFilter filter)
        {
            var customerTypeCodes = GetCachedCustomerTypeCodes();
            Func<WhSJazzCustomerTypeCode, bool> filterFunc = (customerTypeCode) =>
            {
                if (filter != null)
                {
                    if (filter.Filters != null && filter.Filters.Count() > 0)
                    {
                        var context = new WhSJazzCustomerTypeCodeFilterContext
                        {
                            CustomerTypeCode = customerTypeCode
                        };
                        foreach (var customerTypeCodeFilter in filter.Filters)
                        {
                            if (!customerTypeCodeFilter.IsMatch(context))
                                return false;
                        }
                    }
                }
                return true;
            };
            return customerTypeCodes.MapRecords((record) =>
            {
                return CustomerTypeCodeInfoMapper(record);
            }, filterFunc);

        }
        private WhSJazzCustomerTypeCodeDetail CustomerTypeCodeInfoMapper(WhSJazzCustomerTypeCode customerTypeCode)
        {
            return new WhSJazzCustomerTypeCodeDetail
            {
                ID = customerTypeCode.ID,
                Name = customerTypeCode.Name
            };
        }

    }

 
}
