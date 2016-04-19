using System;
using System.Collections.Generic;
using System.Data;
using TOne.WhS.Routing.Entities;
using Vanrise.Data.SQL;

namespace TOne.WhS.Routing.Data.SQL
{
    public class TrafficStatsMeasureDataManager : BaseSQLDataManager, ITrafficStatsMeasureDataManager
    {
        public TrafficStatsMeasureDataManager()
            : base(GetConnectionStringName("TOneWhS_TS_DBConnStringKey", "TOneWhS_TS_DBConnString"))
        {

        }
        #region public methods
        public List<SupplierZoneTrafficStatsMeasure> GetQualityMeasurementsGroupBySupplierZone(TimeSpan timeSpan)
        {
            return GetItemsSP("[TOneWhS_Analytic].[sp_TrafficStatsMeasure_GroupBySupplierZone]", SupplierZoneTrafficStatsMeasureMapper, timeSpan.TotalSeconds);
        }

        public List<SaleZoneSupplierTrafficStatsMeasure> GetQualityMeasurementsGroupBySaleZoneSupplier(TimeSpan timeSpan)
        {
            return GetItemsSP("[TOneWhS_Analytic].[sp_TrafficStatsMeasure_GroupBySaleZone_Supplier]", SaleZoneSupplierTrafficStatsMeasureMapper, timeSpan.TotalSeconds);
        }
        #endregion

        #region Mappers

        SupplierZoneTrafficStatsMeasure SupplierZoneTrafficStatsMeasureMapper(IDataReader reader)
        {
            var instance = new SupplierZoneTrafficStatsMeasure
            {
                SupplierZoneId = (long)reader["SupplierZoneId"],
                TotalDeliveredAttempts = GetReaderValue<int>(reader, "TotalDeliveredAttempts"),
                TotalDurationInSeconds = GetReaderValue<decimal>(reader, "TotalDurationInSeconds"),
                TotalNumberOfAttempts = GetReaderValue<int>(reader, "TotalNumberOfAttempts"),
                TotalSuccesfulAttempts = GetReaderValue<int>(reader, "TotalSuccesfulAttempts")
            };
            
            return instance;
        }

        SaleZoneSupplierTrafficStatsMeasure SaleZoneSupplierTrafficStatsMeasureMapper(IDataReader reader)
        {
            var instance = new SaleZoneSupplierTrafficStatsMeasure
            {
                SaleZoneId = (long)reader["SaleZoneId"],
                SupplierId = (int)reader["SupplierId"],
                TotalDeliveredAttempts = GetReaderValue<int>(reader, "TotalDeliveredAttempts"),
                TotalDurationInSeconds = GetReaderValue<decimal>(reader, "TotalDurationInSeconds"),
                TotalNumberOfAttempts = GetReaderValue<int>(reader, "TotalNumberOfAttempts"),
                TotalSuccesfulAttempts = GetReaderValue<int>(reader, "TotalSuccesfulAttempts")
            };
            return instance;
        }
        #endregion
    }
}
