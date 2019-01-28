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
    public class RegionManager
    {
        public static Guid _definitionId = new Guid("37BE59A4-FCED-45BB-BE80-490B883FA0D1");
        GenericBusinessEntityManager _genericBusinessEntityManager = new GenericBusinessEntityManager();

        public List<Region> GetAllRegions()
        {
            var records = GetCachedRegions();
            List<Region> regions = null;

            if (records != null && records.Count > 0)
            {
                regions = new List<Region>();
                foreach (var record in records)
                {
                    regions.Add(record.Value);
                }
            }
            return regions;
        } 


        private Dictionary<Guid, Region> GetCachedRegions()
        {
            GenericBusinessEntityManager genericBusinessEntityManager = new GenericBusinessEntityManager();
            return genericBusinessEntityManager.GetCachedOrCreate("GetCachedRegions", _definitionId, () =>
            {
                List<GenericBusinessEntity> genericBusinessEntities = genericBusinessEntityManager.GetAllGenericBusinessEntities(_definitionId);
                Dictionary<Guid, Region> result = new Dictionary<Guid, Region>();

                if (genericBusinessEntities != null)
                {
                    foreach (GenericBusinessEntity genericBusinessEntity in genericBusinessEntities)
                    {
                        if (genericBusinessEntity.FieldValues == null)
                            continue;

                        Region region = new Region()
                        {
                            ID = (Guid)genericBusinessEntity.FieldValues.GetRecord("ID"),
                            Name = (string)genericBusinessEntity.FieldValues.GetRecord("Name"),
                            Code = (string)genericBusinessEntity.FieldValues.GetRecord("Code"),
                            CreatedTime = (DateTime)genericBusinessEntity.FieldValues.GetRecord("CreatedTime"),
                            CreatedBy = (int)genericBusinessEntity.FieldValues.GetRecord("CreatedBy"),
                            LastModifiedTime = (DateTime)genericBusinessEntity.FieldValues.GetRecord("LastModifiedTime"),
                            LastModifiedBy = (int)genericBusinessEntity.FieldValues.GetRecord("LastModifiedBy")

                        };
                        result.Add(region.ID, region);
                    }
                }

                return result;
            });
        }

        public IEnumerable<RegionDetail> GetRegionsInfo(RegionInfoFilter filter)
        {
            var regions = GetCachedRegions();
            Func<Region, bool> filterFunc = (region) =>
            {
                if (filter != null)
                {
                    if (filter.Filters != null && filter.Filters.Count() > 0)
                    {
                        var context = new RegionFilterContext
                        {
                            Region = region
                        };
                        foreach (var regionFilter in filter.Filters)
                        {
                            if (!regionFilter.IsMatch(context))
                                return false;
                        }
                    }
                }
                return true;
            };
            return regions.MapRecords((record) =>
            {
                return RegionInfoMapper(record);
            }, filterFunc);

        }

        public Region GetRegionById(Guid regionId)
        {
            var regions = GetCachedRegions();
            return regions.GetRecord(regionId);
        }
        private RegionDetail RegionInfoMapper(Region region)
        {
            return new RegionDetail
            {
                ID = region.ID,
                Name = region.Name
            };
        }

    }


}

