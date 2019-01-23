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
    public class WhSJazzProductServiceCodeManager
    {
        public static Guid _definitionId = new Guid("6222BD42-668C-4574-86B5-A71A1F21B623");
        GenericBusinessEntityManager _genericBusinessEntityManager = new GenericBusinessEntityManager();

        public List<WhSJazzProductServiceCode> GetAllProductServiceCodes()
        {
            var records = GetCachedProductServiceCodes();
            List<WhSJazzProductServiceCode> productServiceCodes = null;

            if (records != null && records.Count > 0)
            {
                productServiceCodes = new List<WhSJazzProductServiceCode>();
                foreach (var record in records)
                {
                    productServiceCodes.Add(record.Value);
                }
            }
            return productServiceCodes;
        }


        private Dictionary<Guid, WhSJazzProductServiceCode> GetCachedProductServiceCodes()
        {
            GenericBusinessEntityManager genericBusinessEntityManager = new GenericBusinessEntityManager();
            return genericBusinessEntityManager.GetCachedOrCreate("GetCachedProductServiceCodes", _definitionId, () =>
            {
                List<GenericBusinessEntity> genericBusinessEntities = genericBusinessEntityManager.GetAllGenericBusinessEntities(_definitionId);
                Dictionary<Guid, WhSJazzProductServiceCode> result = new Dictionary<Guid, WhSJazzProductServiceCode>();

                if (genericBusinessEntities != null)
                {
                    foreach (GenericBusinessEntity genericBusinessEntity in genericBusinessEntities)
                    {
                        if (genericBusinessEntity.FieldValues == null)
                            continue;

                        WhSJazzProductServiceCode productServiceCode = new WhSJazzProductServiceCode()
                        {
                            ID = (Guid)genericBusinessEntity.FieldValues.GetRecord("ID"),
                            Name = (string)genericBusinessEntity.FieldValues.GetRecord("Name"),
                            Code = (string)genericBusinessEntity.FieldValues.GetRecord("Code"),
                            CreatedTime = (DateTime)genericBusinessEntity.FieldValues.GetRecord("CreatedTime"),
                            CreatedBy = (int)genericBusinessEntity.FieldValues.GetRecord("CreatedBy"),
                            LastModifiedTime = (DateTime)genericBusinessEntity.FieldValues.GetRecord("LastModifiedTime"),
                            LastModifiedBy = (int)genericBusinessEntity.FieldValues.GetRecord("LastModifiedBy")

                        };
                        result.Add(productServiceCode.ID, productServiceCode);
                    }
                }

                return result;
            });
        }

        public IEnumerable<WhSJazzProductServiceCodeDetail> GetProductServiceCodesInfo(WhSJazzProductServiceCodeInfoFilter filter)
        {
            var productServiceCodes = GetCachedProductServiceCodes();
            Func<WhSJazzProductServiceCode, bool> filterFunc = (productServiceCode) =>
            {
                if (filter != null)
                {
                    if (filter.Filters != null && filter.Filters.Count() > 0)
                    {
                        var context = new WhSJazzProductServiceCodeFilterContext
                        {
                            ProductServiceCode = productServiceCode
                        };
                        foreach (var productServiceCodeFilter in filter.Filters)
                        {
                            if (!productServiceCodeFilter.IsMatch(context))
                                return false;
                        }
                    }
                }
                return true;
            };
            return productServiceCodes.MapRecords((record) =>
            {
                return ProductServiceCodeInfoMapper(record);
            }, filterFunc);

        }
        private WhSJazzProductServiceCodeDetail ProductServiceCodeInfoMapper(WhSJazzProductServiceCode productServiceCode)
        {
            return new WhSJazzProductServiceCodeDetail
            {
                ID = productServiceCode.ID,
                Name = productServiceCode.Name
            };
        }


    }
}
