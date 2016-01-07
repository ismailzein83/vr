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
        Dictionary<string, string> _mapper;

        public SaleCodeDataManager()
            : base(GetConnectionStringName("TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString"))
        {
            _mapper = new Dictionary<string, string>();
            _mapper.Add("Entity.SaleCodeId", "ID");
            _mapper.Add("ZoneName", "ZoneID");
            _mapper.Add("Entity.Code", "Code");
            _mapper.Add("Entity.BED", "BED");
            _mapper.Add("Entity.EED", "EED");
        }

        public Vanrise.Entities.BigResult<Entities.SaleCode> GetSaleCodeFilteredFromTemp(Vanrise.Entities.DataRetrievalInput<Entities.SaleCodeQuery> input)
        {
            Action<string> createTempTableAction = (tempTableName) =>
            {
                string zonesids = null;
                if (input.Query.ZonesIds != null && input.Query.ZonesIds.Count() > 0)
                    zonesids = string.Join<int>(",", input.Query.ZonesIds);



                ExecuteNonQuerySP("TOneWhS_BE.sp_SaleCode_CreateTempByFiltered", tempTableName, input.Query.EffectiveOn, input.Query.SellingNumberPlanId, zonesids);
            };

            return RetrieveData(input, createTempTableAction, SaleCodeMapper, _mapper);
        }
        public IEnumerable<SaleCode> GetAllSaleCodes()
        {
            return GetItemsSP("TOneWhS_BE.sp_SaleCode_GetAll", SaleCodeMapper);
        }

        public List<SaleCode> GetSaleCodesByZoneID(long zoneID, DateTime effectiveDate)
        {
            return GetItemsSP("TOneWhS_BE.sp_SaleCode_ByZondId", SaleCodeMapper, zoneID, effectiveDate);
        }
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

        public List<SaleCode> GetSellingNumberPlanSaleCodes(int sellingNumberPlanId, DateTime effectiveOn)
        {
            return GetItemsSP("TOneWhS_BE.sp_SaleCode_BySellingNumberPlan", SaleCodeMapper, sellingNumberPlanId, effectiveOn);
        }

        public List<SaleCode> GetSaleCodesByPrefix(string codePrefix, DateTime? effectiveOn, bool isFuture, bool getChildCodes, bool getParentCodes)
        {
            return GetItemsSP("TOneWhS_BE.sp_SaleCode_GetByCodePrefix", SaleCodeMapper, codePrefix, effectiveOn, getChildCodes, getParentCodes);
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
            return GetItemsSP("TOneWhS_BE.sp_SaleCode_ByZondName", SaleCodeMapper, sellingNumberPlanId, zoneName, effectiveDate);
        }


        public List<SaleCode> GetSaleCodesEffectiveAfter(int sellingNumberPlanId, int countryId, DateTime minimumDate)
        {
            return GetItemsSP("TOneWhS_BE.sp_SaleCode_GetByDate", SaleCodeMapper, sellingNumberPlanId, countryId, minimumDate);
        }


        public List<SaleCode> GetSaleCodesByCountry(int countryId, DateTime effectiveDate)
        {
            return GetItemsSP("TOneWhS_BE.sp_SaleCode_ByCountry", SaleCodeMapper, countryId, effectiveDate);
        }
    }
}
