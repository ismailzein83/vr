using System;
using System.Collections.Generic;
using System.Data;
using TOne.WhS.DBSync.Entities;
using Vanrise.Data.SQL;

namespace TOne.WhS.DBSync.Data.SQL
{
    public class SourcePriceListDataManager : BaseSQLDataManager
    {
        DateTime? _effectiveFrom;
        bool _onlyEffective;
        public SourcePriceListDataManager(string connectionString, DateTime? effectiveFrom, bool onlyEffective)
            : base(connectionString, false)
        {
            _effectiveFrom = effectiveFrom;
            _onlyEffective = onlyEffective;
        }

        public List<SourcePriceList> GetSourcePriceLists(bool isSalePriceList, bool migratePriceListData)
        {
            string query = null;

            if (isSalePriceList)
                query = query_getSaleSourcePriceLists;

            if (migratePriceListData && string.IsNullOrEmpty(query))
                query = query_getSupplierSourcePriceListsWithData;

            if (string.IsNullOrEmpty(query))
                query = query_getSupplierSourcePriceLists;

            return GetItemsText(query + MigrationUtils.GetEffectiveQuery("p", _onlyEffective, _effectiveFrom), SourcePriceListMapper, null);
        }

        public void LoadSourceItems(bool isSalePriceList, bool migratePriceListData, Action<SourcePriceList> itemToAdd)
        {
            string query_getSourceSupplierPriceLists = (migratePriceListData ? query_getSupplierSourcePriceListsWithData : query_getSupplierSourcePriceLists)
                                                        + MigrationUtils.GetEffectiveQuery("p", _onlyEffective, _effectiveFrom);
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
            string isSent = (arg["IsSent"] as string);
            return new SourcePriceList()
            {
                SourceId = arg["PriceListID"].ToString(),
                SupplierId = arg["SupplierID"] as string,
                CustomerId = arg["CustomerID"] as string,
                CurrencyId = arg["CurrencyID"] as string,
                BED = (DateTime)arg["BeginEffectiveDate"],
                SourceFileBytes = GetReaderValue<byte[]>(arg, "SourceFileBytes"),
                SourceFileName = arg["SourceFileName"] as string,
                IsSent = string.IsNullOrEmpty(isSent) || isSent.Equals("N") ? false : true,
                Description = arg["Description"] as string               
            };
        }



        const string query_getSaleSourcePriceLists = @"SELECT  PriceListID ,  SupplierID, CustomerID,  Description, CurrencyID,  BeginEffectiveDate ,  EndEffectiveDate,
                                                            NULL SourceFileBytes, NULL SourceFileName, IsSent, Description FROM PriceList p WITH (NOLOCK) 
															join CarrierAccount ca on ca.CarrierAccountID = p.CustomerID
															where SupplierID = 'SYS' and ca.AccountType <> 2 ";

        const string query_getSupplierSourcePriceLists = @"SELECT     p.PriceListID, p.SupplierID, p.CustomerID, p.CurrencyID, NULL SourceFileName, p.BeginEffectiveDate , NULL SourceFileBytes, IsSent,Description
                                                            FROM         PriceList AS p WITH (NOLOCK)
                                                           join CarrierAccount ca on ca.CarrierAccountID = p.SupplierID
															where p.CustomerID = 'SYS' and ca.AccountType <> 0 ";

        const string query_getSupplierSourcePriceListsWithData = @"SELECT     p.PriceListID, p.SupplierID, p.CustomerID, p.CurrencyID, p.SourceFileName, p.BeginEffectiveDate , data.SourceFileBytes, p.IsSent,p.Description
                                                            FROM PriceList AS p WITH (NOLOCK)
															LEFT JOIN PriceListData AS data WITH (NOLOCK) ON p.PriceListID = data.PriceListID
															join CarrierAccount ca on ca.CarrierAccountID = p.SupplierID
                                                            WHERE     (p.CustomerID = 'SYS') and ca.AccountType <> 0 ";

    }
}
