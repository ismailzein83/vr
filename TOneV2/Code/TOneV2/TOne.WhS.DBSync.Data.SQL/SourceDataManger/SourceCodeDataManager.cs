using System;
using System.Collections.Generic;
using System.Data;
using TOne.WhS.DBSync.Entities;
using Vanrise.Data.SQL;

namespace TOne.WhS.DBSync.Data.SQL
{
    public class SourceCodeDataManager : BaseSQLDataManager
    {
        DateTime? _effectiveFrom;
        bool _onlyEffective;
        public SourceCodeDataManager(string connectionString, DateTime? effectiveFrom, bool onlyEffective)
            : base(connectionString, false)
        {
            _effectiveFrom = effectiveFrom;
            _onlyEffective = onlyEffective;
        }

        public List<SourceCode> GetSourceCodes(bool isSaleCode)
        {
            return GetItemsText((isSaleCode ? query_getSourceCodes_Sale : query_getSourceCodes_Purchase) + MigrationUtils.GetEffectiveQuery("Code", _onlyEffective, _effectiveFrom), SourceCodeMapper, null);
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
            ExecuteReaderText((isSaleCode ? query_getSourceCodes_Sale : query_getSourceCodes_Purchase) + MigrationUtils.GetEffectiveQuery("Code", onlyEffective, _effectiveFrom), (reader) =>
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
                                                    
                                                    where Zone.SupplierID = 'SYS'  and zone.CodeGroup <> '-' ";

        const string query_getSourceCodes_Purchase = @"SELECT Code.ID ID, Code.Code Code, Code.ZoneID ZoneID, 
                                                        Code.BeginEffectiveDate BeginEffectiveDate, Code.EndEffectiveDate EndEffectiveDate, Zone.CodeGroup CodeGroup
                                                        FROM Code WITH (NOLOCK) INNER JOIN Zone WITH (NOLOCK)  ON Code.ZoneID = Zone.ZoneID
                                                        Join CarrierAccount ca on ca.CarrierAccountID = Zone.SupplierID
                                                        where Zone.SupplierID <> 'SYS'  and ca.AccountType <> 0  and zone.CodeGroup <> '-' ";
    }
}
