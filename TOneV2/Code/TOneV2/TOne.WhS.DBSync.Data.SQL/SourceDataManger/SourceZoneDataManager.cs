using System;
using System.Collections.Generic;
using System.Data;
using TOne.WhS.DBSync.Entities;
using Vanrise.Data.SQL;

namespace TOne.WhS.DBSync.Data.SQL
{
    public class SourceZoneDataManager : BaseSQLDataManager
    {
        DateTime? _effectiveFrom;
        bool _onlyEffective;
        public SourceZoneDataManager(string connectionString, DateTime? effectiveFrom, bool onlyEffective)
            : base(connectionString, false)
        {
            _effectiveFrom = effectiveFrom;
            _onlyEffective = onlyEffective;
        }

        public List<SourceZone> GetSourceZones(bool isSaleZone)
        {
            return GetItemsText((isSaleZone ? query_getSourceZones_Sale : query_getSourceZones_Purchase) + MigrationUtils.GetEffectiveQuery("Zone", _onlyEffective, _effectiveFrom), SourceZoneMapper, null);
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

        const string query_getSourceZones_Sale = @"SELECT  ZoneID, CodeGroup, Name, SupplierID, BeginEffectiveDate, EndEffectiveDate  FROM [dbo].[Zone] WITH (NOLOCK) where ZoneID <> -1 and SupplierID = 'SYS' and CodeGroup <> '-' ";

        const string query_getSourceZones_Purchase = @"SELECT  ZoneID, CodeGroup, Name, SupplierID, BeginEffectiveDate, EndEffectiveDate  FROM [dbo].[Zone] WITH (NOLOCK) 
                                                        join CarrierAccount ca on ca.CarrierAccountID = SupplierId
                                                        where ZoneID <> -1 and SupplierID <> 'SYS' and CodeGroup <> '-' ";

    }
}
