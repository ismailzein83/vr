using System;
using System.Collections.Generic;
using System.Data;
using TOne.WhS.DBSync.Entities;
using Vanrise.Data.SQL;

namespace TOne.WhS.DBSync.Data.SQL
{
    public class SourceZoneDataManager : BaseSQLDataManager
    {
        public SourceZoneDataManager(string connectionString)
            : base(connectionString, false)
        {
        }

        public List<SourceZone> GetSourceZones(bool isSaleZone, bool onlyEffective)
        {
            string queryOnlyEffective = onlyEffective ? "and Zone.IsEffective = 'Y'" : string.Empty;
            return GetItemsText(query_getSourceZones + (isSaleZone ? "where SupplierID = 'SYS'" : "where SupplierID <> 'SYS'") + queryOnlyEffective, SourceZoneMapper, null);
        }

        private SourceZone SourceZoneMapper(IDataReader arg)
        {
            return new SourceZone()
            {
                SourceId = arg["ZoneID"].ToString(),
                Name = arg["Name"] as string,
                CodeGroup = arg["CodeGroup"] as string,
                SupplierId = arg["SupplierID"] as string,
                BED = (DateTime)arg["BeginEffectiveDate"],
                EED = GetReaderValue<DateTime?>(arg, "EndEffectiveDate")
            };
        }

        const string query_getSourceZones = @"SELECT  ZoneID, CodeGroup, Name, SupplierID, BeginEffectiveDate, EndEffectiveDate  FROM [dbo].[Zone] WITH (NOLOCK) ";
    }
}
