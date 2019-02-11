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
    public class TaxCodeManager
    {
        public static Guid _definitionId = new Guid("EA7443B5-EEC8-4A51-AA26-3A15FE9B2ABE");
        GenericBusinessEntityManager _genericBusinessEntityManager = new GenericBusinessEntityManager();

        public IEnumerable<TaxCode> GetAllTaxCodes()
        {
            return GetCachedTaxCodes().Values;
        }


        private Dictionary<Guid, TaxCode> GetCachedTaxCodes()
        {
            GenericBusinessEntityManager genericBusinessEntityManager = new GenericBusinessEntityManager();
            return genericBusinessEntityManager.GetCachedOrCreate("GetCachedTaxCodes", _definitionId, () =>
            {
                List<GenericBusinessEntity> genericBusinessEntities = genericBusinessEntityManager.GetAllGenericBusinessEntities(_definitionId);
                Dictionary<Guid, TaxCode> result = new Dictionary<Guid, TaxCode>();

                if (genericBusinessEntities != null)
                {
                    foreach (GenericBusinessEntity genericBusinessEntity in genericBusinessEntities)
                    {
                        if (genericBusinessEntity.FieldValues == null)
                            continue;

                        TaxCode taxCode = new TaxCode()
                        {
                            ID = (Guid)genericBusinessEntity.FieldValues.GetRecord("ID"),
                            SwitchId = (int)genericBusinessEntity.FieldValues.GetRecord("SwitchId"),
                            Name = (string)genericBusinessEntity.FieldValues.GetRecord("Name"),
                            Code = (string)genericBusinessEntity.FieldValues.GetRecord("Code"),
                            Type = (ReportDefinitionDirectionEnum)genericBusinessEntity.FieldValues.GetRecord("Type"),
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

        public TaxCode GetTaxCode(int switchId, ReportDefinitionDirectionEnum taxCodeType )
        {
            var taxCodes = GetCachedTaxCodes();
            return taxCodes.FindRecord(x => x.SwitchId == switchId && x.Type==taxCodeType);
        }
        public IEnumerable<TaxCodeDetail> GetTaxCodesInfo(TaxCodeInfoFilter filter)
        {
            var taxCodes = GetCachedTaxCodes();
            Func<TaxCode, bool> filterFunc = (taxCode) =>
            {
                if (filter != null)
                {
                    if (filter.Filters != null && filter.Filters.Count() > 0)
                    {
                        var context = new TaxCodeFilterContext
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
        private TaxCodeDetail TaxCodeInfoMapper(TaxCode taxCode)
        {
            return new TaxCodeDetail
            {
                ID = taxCode.ID,
                Name = taxCode.Name
            };
        }

    }


}
