using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Sales.Entities;
using Vanrise.Data.SQL;

namespace TOne.WhS.BusinessEntity.Data.SQL
{
    public class SaleRateDataManager : BaseSQLDataManager, ISaleRateDataManager
    {

        #region ctor/Local Variables
        private static Dictionary<string, string> _mapper = new Dictionary<string, string>();
        static SaleRateDataManager()
        {
            _mapper.Add("Entity.SaleRateId", "ID");
            _mapper.Add("ZoneName", "ZoneID");
            _mapper.Add("Entity.NormalRate", "Rate");
            _mapper.Add("Entity.BED", "BED");
            _mapper.Add("Entity.EED", "EED");
        }
        public SaleRateDataManager()
            : base(GetConnectionStringName("TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString"))
        {
        }

        #endregion

        #region Public Methods
        public Vanrise.Entities.BigResult<Entities.SaleRate> GetSaleRateFilteredFromTemp(Vanrise.Entities.DataRetrievalInput<Entities.SaleRateQuery> input)
        {
            Action<string> createTempTableAction = (tempTableName) =>
            {
                string zonesids = null;
                if (input.Query.ZonesIds != null && input.Query.ZonesIds.Count() > 0)
                    zonesids = string.Join<int>(",", input.Query.ZonesIds);

                ExecuteNonQuerySP("TOneWhS_BE.sp_SaleRate_CreateTempByFiltered", tempTableName, input.Query.EffectiveOn, input.Query.SellingNumberPlanId, zonesids, input.Query.OwnerType, input.Query.OwnerId);
            };

            return RetrieveData(input, createTempTableAction, SaleRateMapper, _mapper);
        }
        public List<SaleRate> GetEffectiveSaleRates(SalePriceListOwnerType ownerType, int ownerId, DateTime effectiveOn)
        {
            return GetItemsSP("TOneWhS_BE.sp_SaleRate_GetByOwnerAndEffective", SaleRateMapper, ownerType, ownerId, effectiveOn);
        }

        public List<SaleRate> GetSaleRatesEffectiveAfter(int sellingNumberPlanId, DateTime minimumDate)
        {
            return GetItemsSP("TOneWhS_BE.sp_SaleRate_GetEffectiveAfter", SaleRateMapper, sellingNumberPlanId, minimumDate);
        }

        public List<SaleRate> GetEffectiveSaleRateByCustomers(IEnumerable<RoutingCustomerInfo> customerInfos, DateTime? effectiveOn, bool isEffectiveInFuture)
        {
            DataTable dtActiveCustomers = CarrierAccountDataManager.BuildRoutingCustomerInfoTable(customerInfos);
            return GetItemsSPCmd("[TOneWhS_BE].[sp_SaleRate_GetEffectiveByCustomers]", SaleRateMapper, (cmd) =>
            {
                var dtPrm = new SqlParameter("@ActiveCustomersInfo", SqlDbType.Structured);
                dtPrm.Value = dtActiveCustomers;
                cmd.Parameters.Add(dtPrm);
                cmd.Parameters.Add(new SqlParameter("@CustomerOwnerType", SalePriceListOwnerType.Customer));
                cmd.Parameters.Add(new SqlParameter("@EffectiveTime", effectiveOn));
                cmd.Parameters.Add(new SqlParameter("@IsFuture", isEffectiveInFuture));
            });
        }
        public bool AreSaleRatesUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("TOneWhS_BE.SaleRate", ref updateHandle);
        }
        public IEnumerable<SaleRate> GetExistingRatesByZoneIds(SalePriceListOwnerType ownerType, int ownerId, IEnumerable<long> zoneIds, DateTime minEED)
        {
            string zoneIdsParameter = string.Join(",", zoneIds);
            return GetItemsSP("TOneWhS_BE.sp_SaleRate_GetExistingByZoneIDs", SaleRateMapper, ownerType, ownerId, zoneIdsParameter, minEED);
        }
        #endregion

        #region Mappers
        
        private SaleRate SaleRateMapper(IDataReader reader)
        {
            SaleRate saleRate = new SaleRate();

            saleRate.SaleRateId = (long)reader["ID"];
            saleRate.ZoneId = (long)reader["ZoneID"];
            saleRate.PriceListId = (int)reader["PriceListID"];

            saleRate.NormalRate = (decimal)reader["Rate"];
            saleRate.OtherRates = reader["OtherRates"] as string != null ? Vanrise.Common.Serializer.Deserialize<Dictionary<int, decimal>>(reader["OtherRates"] as string) : null;

            saleRate.BED = (DateTime)reader["BED"];
            saleRate.EED = GetReaderValue<DateTime?>(reader, "EED");

            return saleRate;
        }

        #endregion
    }
}
