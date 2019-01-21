using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;
using Vanrise.Common;

namespace TOne.WhS.Jazz.Business
{
    public class WhsJazzRegionCodeManager
    {
        //    public static Guid _definitionId = new Guid("37BE59A4-FCED-45BB-BE80-490B883FA0D1");
        //    GenericBusinessEntityManager _genericBusinessEntityManager = new GenericBusinessEntityManager();

        //    public List<WhSJazzRegion> GetAllRegions()
        //    {
        //        var records = GetCachedRegions();
        //        List<WhSJazzRegion> regions =null;

        //        if (records != null && records.Count > 0)
        //        {
        //            regions = new List<WhSJazzRegion>();
        //            foreach (var record in records)
        //            {
        //                regions.Add(record.Value);
        //            }
        //        }
        //        return regions;
        //    }


        //    private Dictionary<Guid, WhSJazzRegion> GetCachedRegions()
        //    {
        //        GenericBusinessEntityManager genericBusinessEntityManager = new GenericBusinessEntityManager();
        //        return genericBusinessEntityManager.GetCachedOrCreate("GetCachedRegions", _definitionId, () =>
        //        {
        //            List<GenericBusinessEntity> genericBusinessEntities = genericBusinessEntityManager.GetAllGenericBusinessEntities(_definitionId);
        //            Dictionary<Guid, WhSJazzRegion> result = new Dictionary<Guid, WhSJazzRegion>();

        //            if (genericBusinessEntities != null)
        //            {
        //                foreach (GenericBusinessEntity genericBusinessEntity in genericBusinessEntities)
        //                {
        //                    if (genericBusinessEntity.FieldValues == null)
        //                        continue;

        //                    WhSJazzRegion region = new WhSJazzRegion()
        //                    {
        //                        ID = (Guid)genericBusinessEntity.FieldValues.GetRecord("ID"),
        //                        Name = (string)genericBusinessEntity.FieldValues.GetRecord("Name"),
        //                        Code = (string)genericBusinessEntity.FieldValues.GetRecord("Code"),
        //                        CreatedTime = (DateTime)genericBusinessEntity.FieldValues.GetRecord("CreatedTime"),
        //                        CreatedBy = (int)genericBusinessEntity.FieldValues.GetRecord("CreatedBy"),
        //                        LastModifiedTime = (DateTime)genericBusinessEntity.FieldValues.GetRecord("LastModifiedTime"),
        //                        LastModifiedBy = (int)genericBusinessEntity.FieldValues.GetRecord("LastModifiedBy")

        //                    };
        //                    result.Add(region.ID, region);
        //                }
        //            }

        //            return result;
        //        });
        //    }

        //}


    }
}
