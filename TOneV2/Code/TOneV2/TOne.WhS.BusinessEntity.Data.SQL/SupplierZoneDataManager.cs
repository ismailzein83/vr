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
    public class SupplierZoneDataManager : BaseSQLDataManager, ISupplierZoneDataManager
    {

        #region ctor/Local Variables
        public SupplierZoneDataManager()
            : base(GetConnectionStringName("TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString"))
        {

        }

        #endregion

        #region Public Methods
        public List<SupplierZone> GetSupplierZones(int supplierId, DateTime effectiveDate)
        {
            return GetItemsSP("TOneWhS_BE.sp_SupplierZone_GetBySupplierId", SupplierZoneMapper, supplierId, effectiveDate);
        }

        public List<SupplierZoneService> GetSupplierZonesServicesEffectiveAfter(int supplierId, DateTime minimumDate)
        {
            return GetItemsSP("TOneWhS_BE.sp_SupplierZonesServices_GetByDate", SupplierZoneServiceMapper, supplierId, minimumDate);
        }
        public List<SupplierZone> GetSupplierZonesEffectiveAfter(int supplierId, DateTime minimumDate)
        {
            return GetItemsSP("TOneWhS_BE.sp_SupplierZone_GetByDate", SupplierZoneMapper, supplierId, minimumDate);
        }
        public List<SupplierZone> GetSupplierZones()
        {
            return GetItemsSP("TOneWhS_BE.sp_SupplierZone_GetAll", SupplierZoneMapper);
        }
        public bool AreSupplierZonesUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("TOneWhS_BE.SupplierZone", ref updateHandle);
        }
        #endregion

        #region Private Methods
        #endregion

        #region Mappers
        SupplierZone SupplierZoneMapper(IDataReader reader)
        {
            SupplierZone supplierZone = new SupplierZone
            {
                SupplierId = (int)reader["SupplierID"],
                CountryId = (int)reader["CountryID"],
                SupplierZoneId = (long)reader["ID"],
                Name = reader["Name"] as string,
                BED = GetReaderValue<DateTime>(reader, "BED"),
                EED = GetReaderValue<DateTime?>(reader, "EED"),
                SourceId = reader["SourceID"] as string
            };
            return supplierZone;
        }


        SupplierZoneService SupplierZoneServiceMapper(IDataReader reader)
        {
            return new SupplierZoneService()
            {
                SupplierZoneServiceId = (long)reader["ID"],
                ZoneId = (long)reader["ZoneID"],
                ReceivedServices = Vanrise.Common.Serializer.Deserialize<List<Entities.ZoneService>>(reader["ReceivedServiceFlag"] as string),
                EffectiveServices = Vanrise.Common.Serializer.Deserialize<List<Entities.ZoneService>>(reader["EffectiveServiceFlag"] as string),
                BED = GetReaderValue<DateTime>(reader, "BED"),
                EED = GetReaderValue<DateTime?>(reader, "EED"),
            };
        }
        #endregion

    }
}
