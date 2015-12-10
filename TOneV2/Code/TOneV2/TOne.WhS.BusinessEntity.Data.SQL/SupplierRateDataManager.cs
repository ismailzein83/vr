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
            _columnMapper.Add("SupplierRateId", "ID");
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
                BED = GetReaderValue<DateTime>(reader, "BED"),
                EED = GetReaderValue<DateTime?>(reader, "EED"),
                CurrencyId = GetReaderValue<int?>(reader, "CurrencyId")
            };
            return supplierRate;
        }

       


        public BigResult<SupplierRate> GetFilteredSupplierRates(Vanrise.Entities.DataRetrievalInput<SupplierRateQuery> input)
        {
            Action<string> createTempTableAction = (tempTableName) =>
            {
                string zonesids = null;
                if (input.Query.ZoneIds != null && input.Query.ZoneIds.Count() > 0)
                    zonesids = string.Join<int>(",", input.Query.ZoneIds);


                ExecuteNonQuerySP("[TOneWhS_BE].[sp_SupplierRate_CreateTempByFiltered]", tempTableName, input.Query.SupplierId, zonesids, input.Query.EffectiveOn);
            };

            if (input.SortByColumnName != null)
                input.SortByColumnName = input.SortByColumnName.Replace("Entity.", "");

            return RetrieveData(input, createTempTableAction, SupplierRateMapper, _columnMapper);
        }

        


    }
}
