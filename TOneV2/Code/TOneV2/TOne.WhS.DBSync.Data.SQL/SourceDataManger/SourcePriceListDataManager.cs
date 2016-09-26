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

        public List<SourcePriceList> GetSourcePriceLists(bool isSalePriceList, bool migratePriceListData)
        {
            if (isSalePriceList)
                return GetItemsText(query_getSaleSourcePriceLists, SourcePriceListMapper, null);

            if (migratePriceListData)
                return GetItemsText(query_getSupplierSourcePriceListsWithData, SourcePriceListMapper, null);

            return GetItemsText(query_getSupplierSourcePriceLists, SourcePriceListMapper, null);
        }

        public void LoadSourceItems(bool isSalePriceList, bool migratePriceListData, Action<SourcePriceList> itemToAdd)
        {
            string query_getSourceSupplierPriceLists = migratePriceListData ? query_getSupplierSourcePriceListsWithData : query_getSupplierSourcePriceLists;
            ExecuteReaderText(query_getSourceSupplierPriceLists, (reader) =>
                    {
                        while (reader.Read())
                        {
                            itemToAdd(SourcePriceListMapper(reader));
                        }
                    }, null);
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

        const string query_getSupplierSourcePriceLists = @"SELECT     p.PriceListID, p.SupplierID, p.CustomerID, p.CurrencyID, NULL SourceFileName, p.BeginEffectiveDate , NULL SourceFileBytes
                                                            FROM         PriceList AS p WITH (NOLOCK)
                                                            WHERE     (p.CustomerID = 'SYS')";

        const string query_getSupplierSourcePriceListsWithData = @"SELECT     p.PriceListID, p.SupplierID, p.CustomerID, p.CurrencyID, p.SourceFileName, p.BeginEffectiveDate , data.SourceFileBytes
                                                            FROM         PriceList AS p WITH (NOLOCK) LEFT JOIN
                                                                                    PriceListData AS data WITH (NOLOCK) ON p.PriceListID = data.PriceListID
                                                            WHERE     (p.CustomerID = 'SYS')";

    }
}
