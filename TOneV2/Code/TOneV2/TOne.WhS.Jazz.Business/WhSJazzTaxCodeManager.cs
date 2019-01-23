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
    public class WhSJazzTaxCodeManager
    {
        public static Guid _definitionId = new Guid("EA7443B5-EEC8-4A51-AA26-3A15FE9B2ABE");
        GenericBusinessEntityManager _genericBusinessEntityManager = new GenericBusinessEntityManager();

        public IEnumerable<WhSJazzTaxCode> GetAllTaxCodes()
        {
            return GetCachedTaxCodes().Values;
        }


        private Dictionary<Guid, WhSJazzTaxCode> GetCachedTaxCodes()
        {
            GenericBusinessEntityManager genericBusinessEntityManager = new GenericBusinessEntityManager();
            return genericBusinessEntityManager.GetCachedOrCreate("GetCachedTaxCodes", _definitionId, () =>
            {
                List<GenericBusinessEntity> genericBusinessEntities = genericBusinessEntityManager.GetAllGenericBusinessEntities(_definitionId);
                Dictionary<Guid, WhSJazzTaxCode> result = new Dictionary<Guid, WhSJazzTaxCode>();

                if (genericBusinessEntities != null)
                {
                    foreach (GenericBusinessEntity genericBusinessEntity in genericBusinessEntities)
                    {
                        if (genericBusinessEntity.FieldValues == null)
                            continue;

                        WhSJazzTaxCode taxCode = new WhSJazzTaxCode()
                        {
                            ID = (Guid)genericBusinessEntity.FieldValues.GetRecord("ID"),
                            SwitchId = (int)genericBusinessEntity.FieldValues.GetRecord("SwitchId"),
                            Name = (string)genericBusinessEntity.FieldValues.GetRecord("Name"),
                            Code = (string)genericBusinessEntity.FieldValues.GetRecord("Code"),
                            Type = (TaxCodeTypeEnum)genericBusinessEntity.FieldValues.GetRecord("Type"),
                            CreatedTime = (DateTime)genericBusinessEntity.FieldValues.GetRecord("CreatedTime"),
                            CreatedBy = (int)genericBusinessEntity.FieldValues.GetRecord("CreatedBy"),
                            LastModifiedTime = (DateTime)genericBusinessEntity.FieldValues.GetRecord("LastModifiedTime"),
                            LastModifiedBy = (int)genericBusinessEntity.FieldValues.GetRecord("LastModifiedBy")

                        };
                        result.Add(taxCode.ID, taxCode);
                    }
                }

                return result;
            });
        }

        public IEnumerable<WhSJazzTaxCodeDetail> GetTaxCodesInfo(WhSJazzTaxCodeInfoFilter filter)
        {
            var taxCodes = GetCachedTaxCodes();
            Func<WhSJazzTaxCode, bool> filterFunc = (taxCode) =>
            {
                if (filter != null)
                {
                    if (filter.Filters != null && filter.Filters.Count() > 0)
                    {
                        var context = new WhSJazzTaxCodeFilterContext
                        {
                            TaxCode = taxCode
                        };
                        foreach (var taxCodeFilter in filter.Filters)
                        {
                            if (!taxCodeFilter.IsMatch(context))
                                return false;
                        }
                    }
                }
                return true;
            };
            return taxCodes.MapRecords((record) =>
            {
                return TaxCodeInfoMapper(record);
            }, filterFunc);

        }
        private WhSJazzTaxCodeDetail TaxCodeInfoMapper(WhSJazzTaxCode taxCode)
        {
            return new WhSJazzTaxCodeDetail
            {
                ID = taxCode.ID,
                Name = taxCode.Name
            };
        }

    }


}
