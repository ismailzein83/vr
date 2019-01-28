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
    public class MarketManager
    {
        public static Guid _definitionId = new Guid("A4E5560B-C331-486D-88A5-263F8DB7F161");
        GenericBusinessEntityManager _genericBusinessEntityManager = new GenericBusinessEntityManager();

        public List<Market> GetAllMarkets()
        {
            var records = GetCachedMarkets();
            List<Market> markets = null;

            if (records != null && records.Count > 0)
            {
                markets = new List<Market>();
                foreach (var record in records)
                {
                    markets.Add(record.Value);
                }
            }
            return markets;
        }

        public IEnumerable<MarketDetail> GetMarketsInfo(MarketInfoFilter filter)
        {
            var markets = GetCachedMarkets();
            Func<Market, bool> filterFunc = (market) =>
            {
                if (filter != null)
                {
                    if (filter.Filters != null && filter.Filters.Count() > 0)
                    {
                        var context = new MarketFilterContext
                        {
                            Market = market
                        };
                        foreach (var marketFilter in filter.Filters)
                        {
                            if (!marketFilter.IsMatch(context))
                                return false;
                        }
                    }
                }
                return true;
            };
            return markets.MapRecords((record) =>
            {
                return MarketInfoMapper(record);
            }, filterFunc);

        }

        public Market GetMarketById(Guid marketId)
        {
            var markets = GetCachedMarkets();
            return markets.GetRecord(marketId);
        }

        private Dictionary<Guid, Market> GetCachedMarkets()
        {
            GenericBusinessEntityManager genericBusinessEntityManager = new GenericBusinessEntityManager();
            return genericBusinessEntityManager.GetCachedOrCreate("GetCachedMarkets", _definitionId, () =>
            {
                List<GenericBusinessEntity> genericBusinessEntities = genericBusinessEntityManager.GetAllGenericBusinessEntities(_definitionId);
                Dictionary<Guid, Market> result = new Dictionary<Guid, Market>();

                if (genericBusinessEntities != null)
                {
                    foreach (GenericBusinessEntity genericBusinessEntity in genericBusinessEntities)
                    {
                        if (genericBusinessEntity.FieldValues == null)
                            continue;

                        Market market = new Market()
                        {
                            ID = (Guid)genericBusinessEntity.FieldValues.GetRecord("ID"),
                            Name = (string)genericBusinessEntity.FieldValues.GetRecord("Name"),
                            Code = (string)genericBusinessEntity.FieldValues.GetRecord("Code"),
                            ProductServiceId = (Guid)genericBusinessEntity.FieldValues.GetRecord("ProductServiceId"),
                            CreatedTime = (DateTime)genericBusinessEntity.FieldValues.GetRecord("CreatedTime"),
                            CreatedBy = (int)genericBusinessEntity.FieldValues.GetRecord("CreatedBy"),
                            LastModifiedTime = (DateTime)genericBusinessEntity.FieldValues.GetRecord("LastModifiedTime"),
                            LastModifiedBy = (int)genericBusinessEntity.FieldValues.GetRecord("LastModifiedBy")

                        };
                        result.Add(market.ID, market);
                    }
                }

                return result;
            });
        }
        private MarketDetail MarketInfoMapper(Market market)
        {
            return new MarketDetail
            {
                ID = market.ID,
                Name = market.Name
            };
        }

    }

}
