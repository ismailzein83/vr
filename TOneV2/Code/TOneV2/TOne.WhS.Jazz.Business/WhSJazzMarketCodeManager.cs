﻿using System;
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
    public class WhsJazzMarketCodeManager
    {
        public static Guid _definitionId = new Guid("A4E5560B-C331-486D-88A5-263F8DB7F161");
        GenericBusinessEntityManager _genericBusinessEntityManager = new GenericBusinessEntityManager();

        public List<WhSJazzMarketCode> GetAllMarketCodes()
        {
            var records = GetCachedMarketCodes();
            List<WhSJazzMarketCode> marketCodes = null;

            if (records != null && records.Count > 0)
            {
                marketCodes = new List<WhSJazzMarketCode>();
                foreach (var record in records)
                {
                    marketCodes.Add(record.Value);
                }
            }
            return marketCodes;
        }

        public IEnumerable<WhSJazzMarketCodeDetail> GetMarketCodesInfo(WhSJazzMarketCodeInfoFilter filter)
        {
            var marketCodes = GetCachedMarketCodes();
            Func<WhSJazzMarketCode, bool> filterFunc = (marketCode) =>
            {
                if (filter != null)
                {
                    if (filter.Filters != null && filter.Filters.Count() > 0)
                    {
                        var context = new WhSJazzMarketCodeFilterContext
                        {
                            MarketCode = marketCode
                        };
                        foreach (var marketCodeFilter in filter.Filters)
                        {
                            if (!marketCodeFilter.IsMatch(context))
                                return false;
                        }
                    }
                }
                return true;
            };
            return marketCodes.MapRecords((record) =>
            {
                return MarketCodeInfoMapper(record);
            }, filterFunc);

        }

        private Dictionary<Guid, WhSJazzMarketCode> GetCachedMarketCodes()
        {
            GenericBusinessEntityManager genericBusinessEntityManager = new GenericBusinessEntityManager();
            return genericBusinessEntityManager.GetCachedOrCreate("GetCachedMarketCodes", _definitionId, () =>
            {
                List<GenericBusinessEntity> genericBusinessEntities = genericBusinessEntityManager.GetAllGenericBusinessEntities(_definitionId);
                Dictionary<Guid, WhSJazzMarketCode> result = new Dictionary<Guid, WhSJazzMarketCode>();

                if (genericBusinessEntities != null)
                {
                    foreach (GenericBusinessEntity genericBusinessEntity in genericBusinessEntities)
                    {
                        if (genericBusinessEntity.FieldValues == null)
                            continue;

                        WhSJazzMarketCode marketCode = new WhSJazzMarketCode()
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
                        result.Add(marketCode.ID, marketCode);
                    }
                }

                return result;
            });
        }
        private WhSJazzMarketCodeDetail MarketCodeInfoMapper(WhSJazzMarketCode marketCode)
        {
            return new WhSJazzMarketCodeDetail
            {
                ID = marketCode.ID,
                Name = marketCode.Name
            };
        }

    }

}
