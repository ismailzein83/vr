﻿using System;
using System.Collections.Generic;
using System.Data;
using TOne.WhS.DBSync.Entities;
using Vanrise.Data.SQL;

namespace TOne.WhS.DBSync.Data.SQL
{
    public class SourceCodeDataManager : BaseSQLDataManager
    {
        public SourceCodeDataManager(string connectionString)
            : base(connectionString, false)
        {
        }

        public List<SourceCode> GetSourceCodes(bool isSaleCode, bool onlyEffective)
        {
            string queryOnlyEffective =  onlyEffective ? "and Code.IsEffective ='Y'" : string.Empty;
            return GetItemsText((isSaleCode ?query_getSourceCodes_Sale : query_getSourceCodes_Purchase) + queryOnlyEffective, SourceCodeMapper, null);
        }

        private SourceCode SourceCodeMapper(IDataReader arg)
        {
            return new SourceCode()
            {
                SourceId = arg["ID"].ToString(),
                ZoneId = GetReaderValue<int>(arg, "ZoneID"),
                Code = arg["Code"] as string,
                CodeGroup = arg["CodeGroup"] as string,
                BeginEffectiveDate = (DateTime)arg["BeginEffectiveDate"],
                EndEffectiveDate = GetReaderValue<DateTime?>(arg, "EndEffectiveDate"),
            };
        }


        public void LoadSourceItems(bool isSaleCode, bool onlyEffective, Action<SourceCode> itemToAdd)
        {
            string queryOnlyEffective = onlyEffective ? "and Code.IsEffective ='Y'" : string.Empty;
            ExecuteReaderText((isSaleCode ? query_getSourceCodes_Sale : query_getSourceCodes_Purchase) + queryOnlyEffective, (reader) =>
                {
                    while (reader.Read())
                    {
                        itemToAdd(SourceCodeMapper(reader));
                    }
                }, null);
        }


        const string query_getSourceCodes_Sale = @"SELECT Code.ID ID, Code.Code Code, Code.ZoneID ZoneID, 
                                                    Code.BeginEffectiveDate BeginEffectiveDate, Code.EndEffectiveDate EndEffectiveDate, Zone.CodeGroup CodeGroup
                                                    FROM Code WITH (NOLOCK) INNER JOIN Zone WITH (NOLOCK)  ON Code.ZoneID = Zone.ZoneID
                                                    
                                                    where Zone.SupplierID = 'SYS'  ";

        const string query_getSourceCodes_Purchase = @"SELECT Code.ID ID, Code.Code Code, Code.ZoneID ZoneID, 
                                                        Code.BeginEffectiveDate BeginEffectiveDate, Code.EndEffectiveDate EndEffectiveDate, Zone.CodeGroup CodeGroup
                                                        FROM Code WITH (NOLOCK) INNER JOIN Zone WITH (NOLOCK)  ON Code.ZoneID = Zone.ZoneID
                                                        Join CarrierAccount ca on ca.CarrierAccountID = Zone.SupplierID
                                                        where Zone.SupplierID <> 'SYS'  and ca.AccountType <> 0  ";
    }
}
