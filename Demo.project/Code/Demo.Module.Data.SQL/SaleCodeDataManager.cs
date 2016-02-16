using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Demo.Module.Entities;
using Vanrise.Data.SQL;
using Vanrise.Entities;

namespace Demo.Module.Data.SQL
{
    public class SaleCodeDataManager : BaseSQLDataManager, ISaleCodeDataManager
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
            : base(GetConnectionStringName("DemoProject_DBConnStringKey", "DemoDBConnectionString"))
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



                ExecuteNonQuerySP("[dbo].[sp_Code_CreateTempByFiltered]", tempTableName, input.Query.EffectiveOn, zonesids);
            };

            return RetrieveData(input, createTempTableAction, SaleCodeMapper, _mapper);
        }
        public IEnumerable<SaleCode> GetAllSaleCodes()
        {
            return GetItemsSP("[dbo].[sp_Code_GetAll]", SaleCodeMapper);
        }
        public List<SaleCode> GetSaleCodesByZoneID(long zoneID, DateTime effectiveDate)
        {
            return GetItemsSP("[dbo].[sp_Code_GetByZoneId]", SaleCodeMapper, zoneID, effectiveDate);
        }
        //public List<SaleCode> GetSellingNumberPlanSaleCodes(int sellingNumberPlanId, DateTime effectiveOn)
        //{
        //    return GetItemsSP("TOneWhS_BE.sp_SaleCode_GetBySellingNumberPlan", SaleCodeMapper, sellingNumberPlanId, effectiveOn);
        //}
        public List<SaleCode> GetSaleCodesByPrefix(string codePrefix, DateTime? effectiveOn, bool isFuture, bool getChildCodes, bool getParentCodes)
        {
            return GetItemsSP("[dbo].[sp_Code_GetByCodePrefix]", SaleCodeMapper, codePrefix, effectiveOn, isFuture, getChildCodes, getParentCodes);
        }
        public IEnumerable<string> GetDistinctCodeByPrefixes(int prefixLength, DateTime? effectiveOn, bool isFuture)
        {
            return GetItemsSP("[dbo].[sp_Code_GetDistinctCodePrefixes]", CodePrefixMapper, prefixLength, effectiveOn, isFuture);
        }
        public bool AreZonesUpdated(ref object lastReceivedDataInfo)
        {
            return IsDataUpdated("dbo.Zone", ref lastReceivedDataInfo);
        }
        public List<SaleCode> GetSaleCodesByZoneName( string zoneName, DateTime effectiveDate)
        {
            return GetItemsSP("[dbo].[sp_Code_GetByZoneName]", SaleCodeMapper, zoneName, effectiveDate);
        }
        public List<SaleCode> GetSaleCodesEffectiveAfter( int countryId, DateTime minimumDate)
        {
            return GetItemsSP("[dbo].[sp_Code_GetByDate]", SaleCodeMapper, countryId, minimumDate);
        }
        public List<SaleCode> GetSaleCodesByCountry(int countryId, DateTime effectiveDate)
        {
            return GetItemsSP("[dbo].[sp_Code_GetByCountry]", SaleCodeMapper, countryId, effectiveDate);
        }

        public bool AreSaleCodesUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("dbo.Code", ref updateHandle);
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
