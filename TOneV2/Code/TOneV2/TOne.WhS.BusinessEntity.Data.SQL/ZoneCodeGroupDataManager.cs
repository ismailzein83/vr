using System;
using System.Linq;
using System.Collections.Generic;
using Vanrise.Common;
using Vanrise.Data.SQL;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Data.SQL
{
    public class ZoneCodeGroupDataManager : BaseSQLDataManager, IZoneCodeGroupDataManager
    {
        #region ctor/Local Variables
        public ZoneCodeGroupDataManager()
            : base(GetConnectionStringName("TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString"))
        {
        }

        #endregion

        #region Public Methods

        public List<ZoneCodeGroup> GetSaleZoneCodeGroups(DateTime? effectiveOn, bool isFuture)
        {
            Dictionary<long, List<string>> codeGroupsByZone = new Dictionary<long, List<string>>();

            ExecuteReaderSP("[TOneWhS_BE].[sp_ZoneCodeGroup_GetSale]",
                (reader) =>
                {
                    while (reader.Read())
                    {
                        long zoneId = (long)reader["ZoneId"];
                        string codeGroup = reader["CodeGroup"] as string;
                        List<string> codeGroups = codeGroupsByZone.GetOrCreateItem(zoneId);
                        codeGroups.Add(codeGroup);
                    }
                }, effectiveOn, isFuture);

            return codeGroupsByZone.Select(itm => new ZoneCodeGroup() { CodeGroups = itm.Value, ZoneId = itm.Key, IsSale = true }).ToList();
        }

        public List<ZoneCodeGroup> GetCostZoneCodeGroups(DateTime? effectiveOn, bool isFuture)
        {
            Dictionary<long, List<string>> codeGroupsByZone = new Dictionary<long, List<string>>();

            ExecuteReaderSP("[TOneWhS_BE].[sp_ZoneCodeGroup_GetCost]",
                (reader) =>
                {
                    while (reader.Read())
                    {
                        long zoneId = (long)reader["ZoneId"];
                        string codeGroup = reader["CodeGroup"] as string;
                        List<string> codeGroups = codeGroupsByZone.GetOrCreateItem(zoneId);
                        codeGroups.Add(codeGroup);
                    }
                }, effectiveOn, isFuture);

            return codeGroupsByZone.Select(itm => new ZoneCodeGroup() { CodeGroups = itm.Value, ZoneId = itm.Key, IsSale = false }).ToList();
        }

        #endregion
    }
}
