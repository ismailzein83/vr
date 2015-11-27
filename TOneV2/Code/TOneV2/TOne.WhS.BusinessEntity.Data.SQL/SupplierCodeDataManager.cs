﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.Data.SQL;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Data.SQL;
using Vanrise.Entities;

namespace TOne.WhS.BusinessEntity.Data.SQL
{
    public class SupplierCodeDataManager : BaseTOneDataManager, ISupplierCodeDataManager
    {

        private static Dictionary<string, string> _columnMapper = new Dictionary<string, string>();

        public SupplierCodeDataManager()
            : base(GetConnectionStringName("TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString"))
        {
          
        }

        static SupplierCodeDataManager()
        {
            _columnMapper.Add("SupplierCodeId", "ID");
            _columnMapper.Add("SupplierZoneName", "ZoneID");
            _columnMapper.Add("BeginEffectiveDate", "BED");
            _columnMapper.Add("EndEffectiveDate", "EED");
        }

        public List<SupplierCode> GetSupplierCodesEffectiveAfter(int supplierId, DateTime minimumDate)
        {
            return GetItemsSP("TOneWhS_BE.sp_SupplierCode_GetByDate", SupplierCodeMapper, supplierId,minimumDate);
        }
        SupplierCode SupplierCodeMapper(IDataReader reader)
        {
            SupplierCode supplierCode = new SupplierCode
            {
                Code = GetReaderValue<string>(reader, "Code"),
                SupplierCodeId = GetReaderValue<long>(reader, "ID"),
                ZoneId = (long)reader["ZoneID"],
                BeginEffectiveDate = GetReaderValue<DateTime>(reader, "BED"),
                EndEffectiveDate = GetReaderValue<DateTime?>(reader, "EED"),
                
            };
            return supplierCode;
        }

        string CodePrefixMapper(IDataReader reader)
        {
            return reader["CodePrefix"].ToString();
        }

        public List<SupplierCode> GetSupplierCodes(int supplierId, DateTime effectiveOn)
        {
            return GetItemsSP("TOneWhS_BE.sp_SupplierCode_GetBySupplier", SupplierCodeMapper, supplierId, effectiveOn);
        }

        public List<SupplierCode> GetActiveSupplierCodesByPrefix(string codePrefix, DateTime? effectiveOn, bool isFuture, bool getChildCodes, bool getParentCodes, IEnumerable<RoutingSupplierInfo> supplierInfo)
        {
            DataTable dtActiveSuppliers = CarrierAccountDataManager.BuildRoutingSupplierInfoTable(supplierInfo);
            return GetItemsSPCmd("TOneWhS_BE.sp_SupplierCode_GetActiveCodeByPrefix", SupplierCodeMapper, (cmd) =>
            {
                var dtPrm = new SqlParameter("@ActiveSuppliersInfo", SqlDbType.Structured);
                dtPrm.Value = dtActiveSuppliers;
                cmd.Parameters.Add(dtPrm);

                cmd.Parameters.Add(new SqlParameter("@CodePrefix", codePrefix));
                cmd.Parameters.Add(new SqlParameter("@EffectiveOn", effectiveOn));
                cmd.Parameters.Add(new SqlParameter("@GetChildCodes", getChildCodes));
                cmd.Parameters.Add(new SqlParameter("@GetParentCodes", getParentCodes));
            });
        }

        public IEnumerable<string> GetDistinctCodeByPrefixes(int prefixLength, DateTime? effectiveOn, bool isFuture)
        {
            return GetItemsSP("TOneWhS_BE.sp_SupplierCode_GetDistinctCodePrefixes", CodePrefixMapper, prefixLength, effectiveOn, isFuture);
        }

        public Vanrise.Entities.BigResult<SupplierCode> GetFilteredSupplierCodes(Vanrise.Entities.DataRetrievalInput<SupplierCodeQuery> input)
        {
            Action<string> createTempTableAction = (tempTableName) =>
            {
                string zonesids = null;
                if (input.Query.ZonesIds != null && input.Query.ZonesIds.Count() > 0)
                    zonesids = string.Join<int>(",", input.Query.ZonesIds);

                ExecuteNonQuerySP("[TOneWhS_BE].[sp_SupplierCode_CreateTempByFiltered]", tempTableName, input.Query.Code, zonesids, input.Query.EffectiveOn);
            };

            if (input.SortByColumnName != null)
                input.SortByColumnName = input.SortByColumnName.Replace("Entity.", "");

            return RetrieveData(input, createTempTableAction, SupplierCodeMapper, _columnMapper);
        }



    }
}
