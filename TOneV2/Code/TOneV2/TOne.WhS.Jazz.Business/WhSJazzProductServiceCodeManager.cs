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
    public class WhsJazzProductServiceCodeManager
    {
        //    public static Guid _definitionId = new Guid("6222BD42-668C-4574-86B5-A71A1F21B623");
        //    GenericBusinessEntityManager _genericBusinessEntityManager = new GenericBusinessEntityManager();

        //    public List<WhSJazzProductService> GetAllProductServices()
        //    {
        //        var records = GetCachedProductServices();
        //        List<WhSJazzProductService> productServices = null;

        //        if (records != null && records.Count > 0)
        //        {
        //            productServices = new List<WhSJazzProductService>();
        //            foreach (var record in records)
        //            {
        //                productServices.Add(record.Value);
        //            }
        //        }
        //        return productServices;
        //    }


        //    private Dictionary<Guid, WhSJazzProductService> GetCachedProductServices()
        //    {
        //        GenericBusinessEntityManager genericBusinessEntityManager = new GenericBusinessEntityManager();
        //        return genericBusinessEntityManager.GetCachedOrCreate("GetCachedProductServices", _definitionId, () =>
        //        {
        //            List<GenericBusinessEntity> genericBusinessEntities = genericBusinessEntityManager.GetAllGenericBusinessEntities(_definitionId);
        //            Dictionary<Guid, WhSJazzProductService> result = new Dictionary<Guid, WhSJazzProductService>();

        //            if (genericBusinessEntities != null)
        //            {
        //                foreach (GenericBusinessEntity genericBusinessEntity in genericBusinessEntities)
        //                {
        //                    if (genericBusinessEntity.FieldValues == null)
        //                        continue;

        //                    WhSJazzProductService productService = new WhSJazzProductService()
        //                    {
        //                        ID = (Guid)genericBusinessEntity.FieldValues.GetRecord("ID"),
        //                        Name = (string)genericBusinessEntity.FieldValues.GetRecord("Name"),
        //                        Code = (string)genericBusinessEntity.FieldValues.GetRecord("Code"),
        //                        CreatedTime = (DateTime)genericBusinessEntity.FieldValues.GetRecord("CreatedTime"),
        //                        CreatedBy = (int)genericBusinessEntity.FieldValues.GetRecord("CreatedBy"),
        //                        LastModifiedTime = (DateTime)genericBusinessEntity.FieldValues.GetRecord("LastModifiedTime"),
        //                        LastModifiedBy = (int)genericBusinessEntity.FieldValues.GetRecord("LastModifiedBy")

        //                    };
        //                    result.Add(productService.ID, productService);
        //                }
        //            }

        //            return result;
        //        });
        //    }

        //}

        public class WhSJazzProductService
        {
            public Guid ID { get; set; }
            public string Name { get; set; }
            public string Code { get; set; }
            public DateTime CreatedTime { get; set; }
            public DateTime LastModifiedTime { get; set; }
            public int LastModifiedBy { get; set; }
            public int CreatedBy { get; set; }
        }
    }
}
