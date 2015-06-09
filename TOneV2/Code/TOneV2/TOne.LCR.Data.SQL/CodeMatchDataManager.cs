using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using TOne.LCR.Entities;
using System.Data;
using TOne.Data.SQL;
using Vanrise.Data.SQL;

namespace TOne.LCR.Data.SQL
{
    public class CodeMatchDataManager : RoutingDataManager, ICodeMatchDataManager
    {
        public object InitialiazeStreamForDBApply()
        {
            return base.InitializeStreamForBulkInsert();
        }

        public void WriteCodeMatchToStream(CodeMatch codeMatch, object stream)
        {
            StreamForBulkInsert streamForBulkInsert = stream as StreamForBulkInsert;
            streamForBulkInsert.WriteRecord(String.Format("{0}^{1}^{2}^{3}^{4}^{5}^{6}^{7}",
                codeMatch.Code, codeMatch.SupplierId, codeMatch.SupplierCode, codeMatch.SupplierCodeId, codeMatch.SupplierZoneId,
                codeMatch.SupplierRate != null ? (object)codeMatch.SupplierRate.Rate : null, codeMatch.SupplierRate != null ? (object)codeMatch.SupplierRate.ServicesFlag : null,
                codeMatch.SupplierRate != null ? (object)codeMatch.SupplierRate.PriceListId : null));
        }

        public object FinishDBApplyStream(object stream)
        {
            StreamForBulkInsert streamForBulkInsert = stream as StreamForBulkInsert;
            streamForBulkInsert.Close();
            return new StreamBulkInsertInfo
            {
                TableName = "CodeMatch",
                Stream = streamForBulkInsert,
                TabLock = true,
                FieldSeparator = '^'
            };
        }

        public void CreateIndexesOnTable()
        {
            ExecuteNonQueryText(query_CreateIndexesOnTable, null);
        }

        public void ApplyCodeMatchesToDB(Object preparedCodeMatches)
        {
            InsertBulkToTable(preparedCodeMatches as BaseBulkInsertInfo);
        }
        public CodeMatchesByZoneId GetCodeMatchesByCodes(IEnumerable<string> codes, out HashSet<int> supplierZoneIds, out HashSet<int> customerZoneIds)
        {
            HashSet<int> customerZoneIdsTemp = new HashSet<int>();
            HashSet<int> SupplierZoneIdsTemp = new HashSet<int>();
            supplierZoneIds = new HashSet<int>();
            CodeMatchesByZoneId codeMatches = new CodeMatchesByZoneId();
            DataTable codesDataTable = BuildInfoTable<string>(codes, "Code");
            ExecuteReaderText(query_GetCodeMatchesByCodes,
                (reader) =>
                {
                    while (reader.Read())
                    {
                        CodeMatchesByCode codeMatchesByCode;
                        CodeMatch codeMatch = new CodeMatch
                       {
                           Code = reader["Code"] as string,
                           SupplierCode = reader["SupplierCode"] as string,
                           SupplierCodeId = GetReaderValue<Int64>(reader, "SupplierCodeId"),
                           SupplierId = reader["SupplierId"] as string,
                           SupplierRate = new RateInfo()
                           {
                               PriceListId = GetReaderValue<Int32>(reader, "PriceListId"),
                               Rate = GetReaderValue<decimal>(reader, "SupplierRate"),
                               ServicesFlag = GetReaderValue<short>(reader, "ServicesFlag"),
                               ZoneId = GetReaderValue<Int32>(reader, "SupplierZoneId")
                           }
                       };

                        if (!codeMatches.TryGetValue(codeMatch.SupplierRate.ZoneId, out codeMatchesByCode))
                        {
                            codeMatchesByCode = new CodeMatchesByCode();
                            codeMatches.Add(codeMatch.SupplierRate.ZoneId, codeMatchesByCode);
                        }
                        if (!codeMatchesByCode.ContainsKey(codeMatch.Code))
                            codeMatchesByCode.Add(codeMatch.Code, codeMatch);
                        if (String.Compare(codeMatch.SupplierId, "SYS") == 0)
                            customerZoneIdsTemp.Add(codeMatch.SupplierZoneId);
                        else
                            SupplierZoneIdsTemp.Add(codeMatch.SupplierZoneId);
                    }
                },

               (cmd) =>
               {
                   var dtPrm = new SqlParameter("@CodeList", SqlDbType.Structured);
                   dtPrm.TypeName = "StringIDType";
                   dtPrm.Value = codesDataTable;
                   cmd.Parameters.Add(dtPrm);
               });
            supplierZoneIds = SupplierZoneIdsTemp;
            customerZoneIds = customerZoneIdsTemp;

            return codeMatches;
        }

        private DataTable BuildInfoTable<T>(IEnumerable<T> ids, string columnName)
        {
            DataTable dtInfoTable = new DataTable();
            dtInfoTable.Columns.Add(columnName, typeof(T));
            dtInfoTable.BeginLoadData();
            foreach (var t in ids)
            {
                DataRow dr = dtInfoTable.NewRow();
                dr[columnName] = t;
                dtInfoTable.Rows.Add(dr);
            }
            dtInfoTable.EndLoadData();
            return dtInfoTable;
        }

        #region Private Methods


        #endregion

        #region Queries



        const string query_CreateIndexesOnTable = @"/*CREATE CLUSTERED INDEX [IX_CodeMatch_CodeSupplierID] ON [CodeMatch] 
		                                                (
			                                                [Code] ASC,
			                                                [SupplierID] ASC
		                                                )*/
		                                                CREATE NONCLUSTERED INDEX [IX_CodeMatch_Code] ON [CodeMatch] 
		                                                (
			                                                [Code] ASC
		                                                )
		                                                /*CREATE NONCLUSTERED INDEX [IX_CodeMatch_SupplierID] ON [CodeMatch] 
		                                                (
			                                                [SupplierID] ASC
		                                                )
		                                                CREATE NONCLUSTERED INDEX [IX_CodeMatch_SZoneID] ON [CodeMatch] 
		                                                (
			                                                [SupplierZoneID] ASC
		                                                )*/ ";

        const string query_GetCodeMatchesByCodes = @"SELECT  cm.[Code]
                                                            ,cm.[SupplierID]
                                                            ,cm.[SupplierCode]
                                                            ,cm.[SupplierCodeID]
                                                            ,cm.[SupplierZoneID]
                                                            ,cm.[SupplierRate]
                                                            ,cm.[ServicesFlag]
                                                            ,cm.[PriceListID]
                                                     FROM [CodeMatch] cm
                                                     JOIN @CodeList c ON c.Code = cm.Code
                                                     order by cm.[SupplierZoneID]";
        #endregion



    }
}
