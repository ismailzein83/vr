using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Data.SQL;
using Vanrise.Entities;

namespace TOne.WhS.BusinessEntity.Data.SQL
{
    public class SupplierRateDataManager : BaseSQLDataManager, ISupplierRateDataManager
    {

        private static Dictionary<string, string> _columnMapper = new Dictionary<string, string>();
        public SupplierRateDataManager()
            : base(GetConnectionStringName("TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString"))
        {

        }

        static SupplierRateDataManager()
        {
            _columnMapper.Add("CurrencyName", "CurrencyID");
            _columnMapper.Add("SupplierZoneName", "ZoneID");
            _columnMapper.Add("Entity.SupplierRateId", "ID");
        }

        public List<SupplierRate> GetSupplierRates(int supplierId, DateTime minimumDate)
        {
            return GetItemsSP("TOneWhS_BE.sp_SupplierRate_GetByDate", SupplierRateMapper, supplierId, minimumDate);
        }
        public List<SupplierRate> GetEffectiveSupplierRates(int supplierId, DateTime effectiveDate)
        {
            return GetItemsSP("TOneWhS_BE.sp_SupplierRate_GetBySupplierAndEffective", SupplierRateMapper, supplierId, effectiveDate);
        }
        public bool AreSupplierRatesUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("TOneWhS_BE.SupplierRate", ref updateHandle);
        }
        public List<SupplierRate> GetEffectiveSupplierRatesBySuppliers(DateTime? effectiveOn, bool isEffectiveInFuture, IEnumerable<RoutingSupplierInfo> supplierInfos)
        {
            DataTable dtActiveSuppliers = CarrierAccountDataManager.BuildRoutingSupplierInfoTable(supplierInfos);
            return GetItemsSPCmd("TOneWhS_BE.sp_SupplierRate_GetEffectiveBySuppliers", SupplierRateMapper, (cmd) =>
            {
                //, effectiveOn, isEffectiveInFuture
                var dtPrm = new SqlParameter("@ActiveSuppliersInfo", SqlDbType.Structured);
                dtPrm.Value = dtActiveSuppliers;
                cmd.Parameters.Add(dtPrm);

                cmd.Parameters.Add(new SqlParameter("@EffectiveTime", effectiveOn));
                cmd.Parameters.Add(new SqlParameter("@IsFuture", isEffectiveInFuture));
            });
        }
        public List<SupplierRate> GetAllSupplierRates(DateTime? effectiveOn, bool isEffectiveInFuture)
        {
            return GetItemsSP("TOneWhS_BE.sp_SupplierRate_GetAll", SupplierRateMapper, effectiveOn, isEffectiveInFuture);
        }
        SupplierRate SupplierRateMapper(IDataReader reader)
        {
            SupplierRate supplierRate = new SupplierRate
            {
                NormalRate = GetReaderValue<decimal>(reader, "NormalRate"),
                OtherRates = reader["OtherRates"] as string != null ? Vanrise.Common.Serializer.Deserialize<Dictionary<int, decimal>>(reader["OtherRates"] as string) : null,
                SupplierRateId = (long)reader["ID"],
                ZoneId = (long)reader["ZoneID"],
                PriceListId = GetReaderValue<int>(reader, "PriceListID"),
                BeginEffectiveDate = GetReaderValue<DateTime>(reader, "BED"),
                EndEffectiveDate = GetReaderValue<DateTime?>(reader, "EED"),
                CurrencyId = GetReaderValue<int?>(reader, "CurrencyId")
            };
            return supplierRate;
        }

       


        public BigResult<SupplierRate> GetFilteredSupplierRates(Vanrise.Entities.DataRetrievalInput<SupplierRateQuery> input)
        {
            Action<string> createTempTableAction = (tempTableName) =>
            {
                ExecuteNonQueryText(CreateTempTableIfNotExists(tempTableName, input.Query.SupplierId, input.Query.ZoneId, input.Query.EffectiveOn), (cmd) => { });
            };

            return RetrieveData(input, createTempTableAction, SupplierRateMapper, _columnMapper);
        }

        private string CreateTempTableIfNotExists(string tempTableName, int supplierId, int? zoneId, DateTime? effectiveDate)
        {
            StringBuilder query = new StringBuilder(@"
                IF NOT OBJECT_ID('#TEMP_TABLE_NAME#', N'U') IS NOT NULL
                BEGIN

                SELECT  rate.[ID]
                      , rate.[PriceListID]
                      , rate.[ZoneID]
	                  , (case when   priceList.[CurrencyID] is null then rate.[CurrencyID] else  priceList.[CurrencyID]    end) as CurrencyID
                      , rate.[NormalRate]
                      , rate.[OtherRates]
                      , rate.[BED]
                      , rate.[EED]
                      , rate.[timestamp]
                  into #TEMP_TABLE_NAME#
                  FROM [TOneWhS_BE].[SupplierRate] rate inner join [TOneWhS_BE].[SupplierPriceList] priceList on rate.PriceListID=priceList.ID
		
                #WHERE_CLAUSE#
		
                END
            ");

            query.Replace("#TEMP_TABLE_NAME#", tempTableName);
            query.Replace("#WHERE_CLAUSE#", GetWhereClause(supplierId, zoneId, effectiveDate));

            return query.ToString();
        }

        private string GetWhereClause(int supplierId, int? zoneId, DateTime? effectiveDate)
        {
            StringBuilder whereClause = new StringBuilder();

            whereClause.Append("WHERE (priceList.SupplierID =" + supplierId + ")");

            if (zoneId.HasValue)
                whereClause.Append(" AND rate.ZoneID = " + zoneId + "");


            if (effectiveDate.HasValue)
            {
                whereClause.Append(" AND rate.BeginEffectiveDate <= '" + effectiveDate.Value + "'");
                whereClause.Append(" AND (rate.EndEffectiveDate is null or rate.EndEffectiveDate > '" + effectiveDate.Value + "') ");
            }

            return whereClause.ToString();
        }


    }
}
