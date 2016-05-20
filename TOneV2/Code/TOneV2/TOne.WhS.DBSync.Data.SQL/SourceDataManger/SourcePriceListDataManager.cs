using System;
using System.Collections.Generic;
using System.Data;
using TOne.WhS.DBSync.Entities;
using Vanrise.Data.SQL;

namespace TOne.WhS.DBSync.Data.SQL
{
    public class SourcePriceListDataManager : BaseSQLDataManager
    {
        public SourcePriceListDataManager(string connectionString)
            : base(connectionString, false)
        {
        }

        public List<SourcePriceList> GetSourcePriceLists(bool isSalePriceList)
        {
            return GetItemsText(isSalePriceList ? query_getSaleSourcePriceLists : query_getSupplierSourcePriceLists, SourcePriceListMapper, null);
        }

        private SourcePriceList SourcePriceListMapper(IDataReader arg)
        {
            return new SourcePriceList()
            {
                SourceId = arg["PriceListID"].ToString(),
                SupplierId = arg["SupplierID"] as string,
                CustomerId = arg["CustomerID"] as string,
                BeginEffectiveDate = GetReaderValue<DateTime?>(arg, "BeginEffectiveDate"),
                EndEffectiveDate = GetReaderValue<DateTime?>(arg, "EndEffectiveDate"),
                Description = arg["Description"] as string,
                CurrencyId = arg["CurrencyID"] as string,
                SourceFileBytes = GetReaderValue<byte[]>(arg, "SourceFileBytes"),
                SourceFileName = arg["SourceFileName"] as string,
            };
        }

        const string query_getSaleSourcePriceLists = @"SELECT  PriceListID ,  SupplierID, CustomerID,  Description, CurrencyID,  BeginEffectiveDate,  EndEffectiveDate,
                                                               NULL SourceFileBytes, NULL SourceFileName FROM PriceList WITH (NOLOCK) where SupplierID = 'SYS' ";

        const string query_getSupplierSourcePriceLists = @"SELECT  p.PriceListID PriceListID, p.SupplierID SupplierID, p.CustomerID CustomerID, p.Description Description,
                                                           p.CurrencyID CurrencyID, p.BeginEffectiveDate BeginEffectiveDate, p.EndEffectiveDate EndEffectiveDate, p.SourceFileName SourceFileName,
                                                           data.SourceFileBytes SourceFileBytes FROM PriceList p WITH (NOLOCK) INNER JOIN PriceListData data WITH (NOLOCK)
                                                           ON p.PriceListID = data.PriceListID where p.CustomerID = 'SYS'";
    }
}
