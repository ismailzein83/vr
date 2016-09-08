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
    public class SaleEntityServiceDataManager : BaseSQLDataManager, ISaleEntityServiceDataManager
    {
        public SaleEntityServiceDataManager()
            : base(GetConnectionStringName("TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString"))
        {

        }

        public IEnumerable<SaleEntityDefaultService> GetEffectiveSaleEntityDefaultServices(DateTime effectiveOn)
        {
            return GetItemsSP("TOneWhS_BE.sp_SaleEntityService_GetEffectiveDefaultServices", SaleEntityDefaultServiceMapper, effectiveOn);
        }

        public IEnumerable<SaleEntityZoneService> GetEffectiveSaleEntityZoneServices(Entities.SalePriceListOwnerType ownerType, int ownerId, DateTime effectiveOn)
        {
            return GetItemsSP("TOneWhS_BE.sp_SaleEntityService_GetEffectiveZoneServices", SaleEntityZoneServiceMapper, ownerType, ownerId, effectiveOn);
        }

        public IEnumerable<SaleEntityDefaultService> GetDefaultServicesEffectiveAfter(SalePriceListOwnerType ownerType, int ownerId, DateTime minimumDate)
        {
            return GetItemsSP("TOneWhS_BE.sp_SaleEntityService_GetDefaultServicesEffectiveAfter", SaleEntityDefaultServiceMapper, ownerType, ownerId, minimumDate);
        }

        public IEnumerable<SaleEntityZoneService> GetZoneServicesEffectiveAfter(SalePriceListOwnerType ownerType, int ownerId, DateTime minimumDate)
        {
            return GetItemsSP("TOneWhS_BE.sp_SaleEntityService_GetZoneServicesEffectiveAfter", SaleEntityZoneServiceMapper, ownerType, ownerId, minimumDate);
        }
        public bool AreSaleEntityServicesUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("TOneWhS_BE.SaleEntityService", ref updateHandle);
        }

        #region Mappers

        private SaleEntityDefaultService SaleEntityDefaultServiceMapper(IDataReader reader)
        {
            return new SaleEntityDefaultService()
            {
                SaleEntityServiceId = (long)reader["ID"],
                PriceListId = (int)reader["PriceListID"],
                Services = Vanrise.Common.Serializer.Deserialize<List<ZoneService>>(reader["Services"] as string),
                BED = (DateTime)reader["BED"],
                EED = GetReaderValue<DateTime?>(reader, "EED")
            };
        }

        private SaleEntityZoneService SaleEntityZoneServiceMapper(IDataReader reader)
        {
            return new SaleEntityZoneService()
            {
                SaleEntityServiceId = (long)reader["ID"],
                PriceListId = (int)reader["PriceListID"],
                ZoneId = (long)reader["ZoneID"],
                Services = Vanrise.Common.Serializer.Deserialize<List<ZoneService>>(reader["Services"] as string),
                BED = (DateTime)reader["BED"],
                EED = GetReaderValue<DateTime?>(reader, "EED")
            };
        }

        #endregion
    }
}
