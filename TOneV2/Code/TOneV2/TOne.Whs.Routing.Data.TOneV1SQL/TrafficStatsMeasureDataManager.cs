using System;
using System.Collections.Generic;
using System.Data;
using TOne.WhS.Routing.Data;
using TOne.WhS.Routing.Entities;
using Vanrise.Data.SQL;

namespace TOne.Whs.Routing.Data.TOneV1SQL
{
    public class TrafficStatsMeasureDataManager : BaseSQLDataManager, ITrafficStatsMeasureDataManager
    {
        public TrafficStatsMeasureDataManager()
            : base(GetConnectionStringName("TOneWhS_Analytics_DBConnStringKey", "TOneAnalyticsDBConnString"))
        {

        }
        #region public methods
        public List<SupplierZoneTrafficStatsMeasure> GetQualityMeasurementsGroupBySupplierZone(TimeSpan timeSpan)
        {
            return GetItemsSP("[TOneWhS_Analytics].[sp_TrafficStatsMeasure_GroupBySupplierZone]", SupplierZoneTrafficStatsMeasureMapper, timeSpan.TotalSeconds);
        }

        public List<SaleZoneSupplierTrafficStatsMeasure> GetQualityMeasurementsGroupBySaleZoneSupplier(TimeSpan timeSpan)
        {
            return GetItemsSP("[TOneWhS_Analytics].[sp_TrafficStatsMeasure_GroupBySaleZone_Supplier]", SaleZoneSupplierTrafficStatsMeasureMapper, timeSpan.TotalSeconds);
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
