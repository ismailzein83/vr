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
    public class WhSJazzRegionCodeManager
    {
        public static Guid _definitionId = new Guid("37BE59A4-FCED-45BB-BE80-490B883FA0D1");
        GenericBusinessEntityManager _genericBusinessEntityManager = new GenericBusinessEntityManager();

        public List<WhSJazzRegionCode> GetAllRegionCodes()
        {
            var records = GetCachedRegionCodes();
            List<WhSJazzRegionCode> regionCodes = null;

            if (records != null && records.Count > 0)
            {
                regionCodes = new List<WhSJazzRegionCode>();
                foreach (var record in records)
                {
                    regionCodes.Add(record.Value);
                }
            }
            return regionCodes;
        } 


        private Dictionary<Guid, WhSJazzRegionCode> GetCachedRegionCodes()
        {
            GenericBusinessEntityManager genericBusinessEntityManager = new GenericBusinessEntityManager();
            return genericBusinessEntityManager.GetCachedOrCreate("GetCachedRegionCodes", _definitionId, () =>
            {
                List<GenericBusinessEntity> genericBusinessEntities = genericBusinessEntityManager.GetAllGenericBusinessEntities(_definitionId);
                Dictionary<Guid, WhSJazzRegionCode> result = new Dictionary<Guid, WhSJazzRegionCode>();

                if (genericBusinessEntities != null)
                {
                    foreach (GenericBusinessEntity genericBusinessEntity in genericBusinessEntities)
                    {
                        if (genericBusinessEntity.FieldValues == null)
                            continue;

                        WhSJazzRegionCode regionCode = new WhSJazzRegionCode()
                        {
                            ID = (Guid)genericBusinessEntity.FieldValues.GetRecord("ID"),
                            Name = (string)genericBusinessEntity.FieldValues.GetRecord("Name"),
                            Code = (string)genericBusinessEntity.FieldValues.GetRecord("Code"),
                            CreatedTime = (DateTime)genericBusinessEntity.FieldValues.GetRecord("CreatedTime"),
                            CreatedBy = (int)genericBusinessEntity.FieldValues.GetRecord("CreatedBy"),
                            LastModifiedTime = (DateTime)genericBusinessEntity.FieldValues.GetRecord("LastModifiedTime"),
                            LastModifiedBy = (int)genericBusinessEntity.FieldValues.GetRecord("LastModifiedBy")

                        };
                        result.Add(regionCode.ID, regionCode);
                    }
                }

                return result;
            });
        }

        public IEnumerable<WhSJazzRegionCodeDetail> GetRegionCodesInfo(WhSJazzRegionCodeInfoFilter filter)
        {
            var regionCodes = GetCachedRegionCodes();
            Func<WhSJazzRegionCode, bool> filterFunc = (regionCode) =>
            {
                if (filter != null)
                {
                    if (filter.Filters != null && filter.Filters.Count() > 0)
                    {
                        var context = new WhSJazzRegionCodeFilterContext
                        {
                            RegionCode = regionCode
                        };
                        foreach (var regionCodeFilter in filter.Filters)
                        {
                            if (!regionCodeFilter.IsMatch(context))
                                return false;
                        }
                    }
                }
                return true;
            };
            return regionCodes.MapRecords((record) =>
            {
                return RegionCodeInfoMapper(record);
            }, filterFunc);

        }
        private WhSJazzRegionCodeDetail RegionCodeInfoMapper(WhSJazzRegionCode regionCode)
        {
            return new WhSJazzRegionCodeDetail
            {
                ID = regionCode.ID,
                Name = regionCode.Name
            };
        }

    }


}

