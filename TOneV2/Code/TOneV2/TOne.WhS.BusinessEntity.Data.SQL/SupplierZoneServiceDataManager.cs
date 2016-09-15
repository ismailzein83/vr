﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Data.SQL;

namespace TOne.WhS.BusinessEntity.Data.SQL
{
    public class SupplierZoneServiceDataManager : BaseSQLDataManager, ISupplierZoneServiceDataManager
    {
        #region ctor/Local Variables
        
        public SupplierZoneServiceDataManager()
            : base(GetConnectionStringName("TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString"))
        {

        }

        #endregion
       
        #region Public Methods
       
        public List<SupplierZoneService> GetSupplierZonesServicesEffectiveAfter(int supplierId, DateTime minimumDate)
        {
            return GetItemsSP("TOneWhS_BE.sp_SupplierZonesServices_GetByDate", SupplierZoneServiceMapper, supplierId, minimumDate);
        }

        public IEnumerable<SupplierZoneService> GetFilteredSupplierZoneServices(SupplierZoneServiceQuery query)
        {
            string zonesids = null;
            if (query.ZoneIds != null && query.ZoneIds.Count() > 0)
                zonesids = string.Join<int>(",", query.ZoneIds);

            return GetItemsSP("[TOneWhS_BE].[sp_SupplierZoneService_GetFiltered]", SupplierZoneServiceMapper, query.SupplierId, zonesids, query.EffectiveOn);
        }
       
        #endregion
       

        #region Mappers
        
        SupplierZoneService SupplierZoneServiceMapper(IDataReader reader)
        {
            return new SupplierZoneService()
            {
                SupplierZoneServiceId = (long)reader["ID"],
                ZoneId = (long)reader["ZoneID"],
                ReceivedServices = Vanrise.Common.Serializer.Deserialize<List<Entities.ZoneService>>(reader["ReceivedServicesFlag"] as string),
                EffectiveServices = Vanrise.Common.Serializer.Deserialize<List<Entities.ZoneService>>(reader["EffectiveServiceFlag"] as string),
                BED = GetReaderValue<DateTime>(reader, "BED"),
                EED = GetReaderValue<DateTime?>(reader, "EED"),
            };
        }
        #endregion

    }
}
