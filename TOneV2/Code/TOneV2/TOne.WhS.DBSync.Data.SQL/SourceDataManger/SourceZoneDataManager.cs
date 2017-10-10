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
            string queryOnlyEffective = onlyEffective ? " and Zone.IsEffective = 'Y'" : string.Empty;
            return GetItemsText((isSaleZone ? query_getSourceZones_Sale : query_getSourceZones_Purchase) + queryOnlyEffective, SourceZoneMapper, null);
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

        const string query_getSourceZones_Sale = @"SELECT  ZoneID, CodeGroup, Name, SupplierID, BeginEffectiveDate, EndEffectiveDate  FROM [dbo].[Zone] WITH (NOLOCK) where ZoneID <> -1 and SupplierID = 'SYS' ";

        const string query_getSourceZones_Purchase = @"SELECT  ZoneID, CodeGroup, Name, SupplierID, BeginEffectiveDate, EndEffectiveDate  FROM [dbo].[Zone] WITH (NOLOCK) 
                                                        join CarrierAccount ca on ca.CarrierAccountID = SupplierId
                                                        where ZoneID <> -1 and SupplierID <> 'SYS' and ca.AccountType <> 0 ";

    }
}
