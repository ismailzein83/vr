using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.Data.SQL;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Data.SQL;

namespace TOne.WhS.BusinessEntity.Data.SQL
{
    public class SaleCodeDataManager : BaseTOneDataManager, ISaleCodeDataManager
    {

        #region ctor/Local Variables

        private static Dictionary<string, string> _mapper = new Dictionary<string, string>();
        static SaleCodeDataManager()
        {
            _mapper = new Dictionary<string, string>();
            _mapper.Add("Entity.SaleCodeId", "ID");
            _mapper.Add("ZoneName", "ZoneID");
            _mapper.Add("Entity.Code", "Code");
            _mapper.Add("Entity.BED", "BED");
            _mapper.Add("Entity.EED", "EED");
        }
        public SaleCodeDataManager()
            : base(GetConnectionStringName("TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString"))
        {

        }
        #endregion

        #region Public Methods
        public Vanrise.Entities.BigResult<Entities.SaleCode> GetSaleCodeFilteredFromTemp(Vanrise.Entities.DataRetrievalInput<Entities.SaleCodeQuery> input)
        {
            Action<string> createTempTableAction = (tempTableName) =>
            {
                string zonesids = null;
                if (input.Query.ZonesIds != null && input.Query.ZonesIds.Count() > 0)
                    zonesids = string.Join<int>(",", input.Query.ZonesIds);



                ExecuteNonQuerySP("TOneWhS_BE.sp_SaleCode_CreateTempByFiltered", tempTableName, input.Query.EffectiveOn,input.Query.Code, input.Query.SellingNumberPlanId, zonesids);
            };

            return RetrieveData(input, createTempTableAction, SaleCodeMapper, _mapper);
        }
        public IEnumerable<SaleCode> GetAllSaleCodes()
        {
            return GetItemsSP("TOneWhS_BE.sp_SaleCode_GetAll", SaleCodeMapper);
        }
        public List<SaleCode> GetSaleCodesByZoneID(long zoneID, DateTime effectiveDate)
        {
            return GetItemsSP("TOneWhS_BE.sp_SaleCode_GetByZoneId", SaleCodeMapper, zoneID, effectiveDate);
        }

        public List<SaleCode> GetSaleCodesEffectiveByZoneID(long zoneID, DateTime effectiveDate)
        {
            return GetItemsSP("TOneWhS_BE.sp_SaleCode_GetEffectiveByZoneId", SaleCodeMapper, zoneID, effectiveDate);
        }

        public List<SaleCode> GetSellingNumberPlanSaleCodes(int sellingNumberPlanId, DateTime effectiveOn)
        {
            return GetItemsSP("TOneWhS_BE.sp_SaleCode_GetBySellingNumberPlan", SaleCodeMapper, sellingNumberPlanId, effectiveOn);
        }

        public List<SaleCode> GetSaleCodesEffectiveAfter(int sellingNumberPlanId, DateTime effectiveOn)
        {
            return GetItemsSP("TOneWhS_BE.sp_SaleCode_GetEffectiveAndPendingBySellingNumberPlan", SaleCodeMapper, sellingNumberPlanId, effectiveOn); 
        }
        public List<SaleCode> GetSaleCodesByPrefix(string codePrefix, DateTime? effectiveOn, bool isFuture, bool getChildCodes, bool getParentCodes)
        {
            return GetItemsSP("TOneWhS_BE.sp_SaleCode_GetByCodePrefix", SaleCodeMapper, codePrefix, effectiveOn, isFuture, getChildCodes, getParentCodes);
        }
        public IEnumerable<string> GetDistinctCodeByPrefixes(int prefixLength, DateTime? effectiveOn, bool isFuture)
        {
            return GetItemsSP("TOneWhS_BE.sp_SaleCode_GetDistinctCodePrefixes", CodePrefixMapper, prefixLength, effectiveOn, isFuture);
        }
        public bool AreZonesUpdated(ref object lastReceivedDataInfo)
        {
            return IsDataUpdated("TOneWhS_BE.SaleCode", ref lastReceivedDataInfo);
        }
        public List<SaleCode> GetSaleCodesByZoneName(int sellingNumberPlanId, string zoneName, DateTime effectiveDate)
        {
            return GetItemsSP("TOneWhS_BE.sp_SaleCode_GetByZoneName", SaleCodeMapper, sellingNumberPlanId, zoneName, effectiveDate);
        }
        public List<SaleCode> GetSaleCodesEffectiveAfter(int sellingNumberPlanId, int countryId, DateTime minimumDate)
        {
            return GetItemsSP("TOneWhS_BE.sp_SaleCode_GetByDate", SaleCodeMapper, sellingNumberPlanId, countryId, minimumDate);
        }
        public List<SaleCode> GetSaleCodesByCountry(int countryId, DateTime effectiveDate)
        {
            return GetItemsSP("TOneWhS_BE.sp_SaleCode_GetByCountry", SaleCodeMapper, countryId, effectiveDate);
        }

        public List<SaleCode> GetSaleCodesByZoneIDs(List<long> zoneIds, DateTime effectiveDate)
        {
            string allZoneIds = null;
            if (zoneIds != null && zoneIds.Count() > 0)
                allZoneIds = string.Join<long>(",", zoneIds);
            return GetItemsSP("TOneWhS_BE.sp_SaleCode_GetByZoneIds", SaleCodeMapper, allZoneIds, effectiveDate);
        }

        public bool AreSaleCodesUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("TOneWhS_BE.SaleCode", ref updateHandle);
        }

        #endregion

        #region Private Methods

        #endregion

        #region Mappers
        SaleCode SaleCodeMapper(IDataReader reader)
        {
            SaleCode saleCode = new SaleCode
            {
                SaleCodeId = (long)reader["ID"],
                Code = reader["Code"] as string,
                ZoneId = GetReaderValue<long>(reader, "ZoneID"),
                BED = GetReaderValue<DateTime>(reader, "BED"),
                EED = GetReaderValue<DateTime?>(reader, "EED")
            };
            return saleCode;
        }
        string CodePrefixMapper(IDataReader reader)
        {
            return reader["CodePrefix"].ToString();
        }
        #endregion
    }
}
