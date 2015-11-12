using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Data.SQL;

namespace TOne.WhS.BusinessEntity.Data.SQL
{
    public class SupplierZoneDataManager:BaseSQLDataManager,ISupplierZoneDataManager
    {
        public SupplierZoneDataManager()
            : base(GetConnectionStringName("TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString"))
        {
          
        }

        public List<SupplierZone> GetSupplierZones(int supplierId, DateTime effectiveDate)
        {
            return GetItemsSP("TOneWhS_BE.sp_SupplierZone_GetBySupplierId", SupplierZoneMapper, supplierId, effectiveDate);
        }
        SupplierZone SupplierZoneMapper(IDataReader reader)
        {
            SupplierZone supplierZone = new SupplierZone
            {
                SupplierId = (int)reader["SupplierID"],
                CountryId = (int)reader["CountryId"],
                SupplierZoneId = (long)reader["ID"],
                Name = reader["Name"] as string,
                BeginEffectiveDate = GetReaderValue<DateTime>(reader, "BED"),
                EndEffectiveDate = GetReaderValue<DateTime?>(reader, "EED")
            };
            return supplierZone;
        }


        public long ReserveIDRange(int numberOfIDs)
        {
            object  lastIndex=ExecuteScalarSP("TOneWhS_BE.sp_SupplierZoneIDManager_ReserveIDRange", numberOfIDs);
            return (long)lastIndex;
        }


        public List<SupplierZone> GetSupplierZones()
        {
            return GetItemsSP("TOneWhS_BE.sp_SupplierZone_GetAll", SupplierZoneMapper);
        }

        public bool AreSupplierZonesUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("TOneWhS_BE.SupplierZone", ref updateHandle);
        }
    }
}
