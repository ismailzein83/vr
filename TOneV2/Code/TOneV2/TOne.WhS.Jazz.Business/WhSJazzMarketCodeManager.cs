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
    public class WhsJazzMarketCodeManager
    {
        //    public static Guid _definitionId = new Guid("A4E5560B-C331-486D-88A5-263F8DB7F161");
        //    GenericBusinessEntityManager _genericBusinessEntityManager = new GenericBusinessEntityManager();

        //    public List<WhSJazzMarket> GetAllMarkets()
        //    {
        //        var records = GetCachedMarkets();
        //        List<WhSJazzMarket> markets = null;

        //        if (records != null && records.Count > 0)
        //        {
        //            markets = new List<WhSJazzMarket>();
        //            foreach (var record in records)
        //            {
        //                markets.Add(record.Value);
        //            }
        //        }
        //        return markets;
        //    }


        //    private Dictionary<Guid, WhSJazzMarket> GetCachedMarkets()
        //    {
        //        GenericBusinessEntityManager genericBusinessEntityManager = new GenericBusinessEntityManager();
        //        return genericBusinessEntityManager.GetCachedOrCreate("GetCachedMarkets", _definitionId, () =>
        //        {
        //            List<GenericBusinessEntity> genericBusinessEntities = genericBusinessEntityManager.GetAllGenericBusinessEntities(_definitionId);
        //            Dictionary<Guid, WhSJazzMarket> result = new Dictionary<Guid, WhSJazzMarket>();

        //            if (genericBusinessEntities != null)
        //            {
        //                foreach (GenericBusinessEntity genericBusinessEntity in genericBusinessEntities)
        //                {
        //                    if (genericBusinessEntity.FieldValues == null)
        //                        continue;

        //                    WhSJazzMarket market = new WhSJazzMarket()
        //                    {
        //                        ID = (Guid)genericBusinessEntity.FieldValues.GetRecord("ID"),
        //                        Name = (string)genericBusinessEntity.FieldValues.GetRecord("Name"),
        //                        Code = (string)genericBusinessEntity.FieldValues.GetRecord("Code"),
        //                        ProductServiceId = (Guid)genericBusinessEntity.FieldValues.GetRecord("ProductServiceId"),
        //                        CreatedTime = (DateTime)genericBusinessEntity.FieldValues.GetRecord("CreatedTime"),
        //                        CreatedBy = (int)genericBusinessEntity.FieldValues.GetRecord("CreatedBy"),
        //                        LastModifiedTime = (DateTime)genericBusinessEntity.FieldValues.GetRecord("LastModifiedTime"),
        //                        LastModifiedBy = (int)genericBusinessEntity.FieldValues.GetRecord("LastModifiedBy")

        //                    };
        //                    result.Add(market.ID, market);
        //                }
        //            }

        //            return result;
        //        });
        //    }

        //}

        public class WhSJazzMarket
        {
            public Guid ID { get; set; }
            public string Name { get; set; }
            public string Code { get; set; }
            public Guid ProductServiceId { get; set; }
            public DateTime CreatedTime { get; set; }
            public DateTime LastModifiedTime { get; set; }
            public int LastModifiedBy { get; set; }
            public int CreatedBy { get; set; }
        }
    }
}
