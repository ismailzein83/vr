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
                CurrencyId = arg["CurrencyID"] as string,
                BED = (DateTime)arg["BeginEffectiveDate"],
                SourceFileBytes = GetReaderValue<byte[]>(arg, "SourceFileBytes"),
                SourceFileName = arg["SourceFileName"] as string,
            };
        }

        const string query_getSaleSourcePriceLists = @"SELECT  PriceListID ,  SupplierID, CustomerID,  Description, CurrencyID,  BeginEffectiveDate ,  EndEffectiveDate,
                                                               NULL SourceFileBytes, NULL SourceFileName FROM PriceList WITH (NOLOCK) where SupplierID = 'SYS' ";

        const string query_getSupplierSourcePriceLists = @"SELECT     p.PriceListID, p.SupplierID, p.CustomerID, p.CurrencyID, p.SourceFileName, p.BeginEffectiveDate , data.SourceFileBytes
                                                            FROM         PriceList AS p WITH (NOLOCK) LEFT JOIN
                                                                                    PriceListData AS data WITH (NOLOCK) ON p.PriceListID = data.PriceListID
                                                            WHERE     (p.CustomerID = 'SYS')";
    }
}
