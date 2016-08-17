﻿using System;
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
    public class SupplierCodeDataManager : Vanrise.Data.SQL.BaseSQLDataManager, ISupplierCodeDataManager
    {

        #region ctor/Local Variables
        public SupplierCodeDataManager()
            : base(GetConnectionStringName("TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString"))
        {

        }

        #endregion

        #region Public Methods
        public List<SupplierCode> GetSupplierCodesEffectiveAfter(int supplierId, DateTime minimumDate)
        {
            return GetItemsSP("TOneWhS_BE.sp_SupplierCode_GetByDate", SupplierCodeMapper, supplierId, minimumDate);
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
                cmd.Parameters.Add(new SqlParameter("@IsFuture", isFuture));
                cmd.Parameters.Add(new SqlParameter("@GetChildCodes", getChildCodes));
                cmd.Parameters.Add(new SqlParameter("@GetParentCodes", getParentCodes));
            });
        }
        
        public IEnumerable<CodePrefixInfo> GetDistinctCodeByPrefixes(int prefixLength, DateTime? effectiveOn, bool isFuture)
        {
            return GetItemsSP("TOneWhS_BE.sp_SupplierCode_GetDistinctCodePrefixes", CodePrefixMapper, prefixLength, effectiveOn, isFuture);
        }
        public IEnumerable<CodePrefixInfo> GetSpecificCodeByPrefixes(int prefixLength, IEnumerable<string> codePrefixes, DateTime? effectiveOn, bool isFuture) 
        {
            string _codePrefixes = null;
            if (codePrefixes != null && codePrefixes.Count() > 0)
                _codePrefixes = string.Join<string>(",", codePrefixes);

            return GetItemsSP("TOneWhS_BE.sp_SupplierCode_GetSpecificCodePrefixes", CodePrefixMapper, prefixLength, _codePrefixes, effectiveOn, isFuture);    
        }
        

        public IEnumerable<SupplierCode> GetFilteredSupplierCodes(SupplierCodeQuery query)
        {
            string zoneIds = null;
            if (query.ZoneIds != null && query.ZoneIds.Count() > 0)
                zoneIds = string.Join<int>(",", query.ZoneIds);

            return GetItemsSP("[TOneWhS_BE].[sp_SupplierCode_GetFiltered]", SupplierCodeMapper, query.Code, query.SupplierId, zoneIds, query.EffectiveOn);
        }

        public bool AreSupplierCodesUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("TOneWhS_BE.SupplierCode", ref updateHandle);
        }

        #endregion

        #region Private Methods
        #endregion

        #region Mappers
        SupplierCode SupplierCodeMapper(IDataReader reader)
        {
            SupplierCode supplierCode = new SupplierCode
            {
                Code = GetReaderValue<string>(reader, "Code"),
                SupplierCodeId = GetReaderValue<long>(reader, "ID"),
                ZoneId = (long)reader["ZoneID"],
                BED = GetReaderValue<DateTime>(reader, "BED"),
                EED = GetReaderValue<DateTime?>(reader, "EED"),

            };
            return supplierCode;
        }
        CodePrefixInfo CodePrefixMapper(IDataReader reader)
        {
            return new CodePrefixInfo()
            {
                CodePrefix = reader["CodePrefix"] as string,
                Count = GetReaderValue<int>(reader, "codeCount")
            };
        }

        #endregion
    }
}
