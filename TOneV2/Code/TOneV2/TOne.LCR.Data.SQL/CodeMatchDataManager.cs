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
    public class CodeMatchDataManager : BaseTOneDataManager, ICodeMatchDataManager
    {
        #region Static

        static CodeMatchDataManager()
        {
            CreateSchemaTables();
        }

        static DataTable s_supplierCodeMatchSchemaTable;
        static DataTable s_codeMatchSchemaTable;
        static void CreateSchemaTables()
        {
            s_codeMatchSchemaTable = new DataTable();
            s_codeMatchSchemaTable.TableName = "LCR.CodeMatch_temp";

            s_codeMatchSchemaTable.Columns.Add("Code", typeof(string));
            s_codeMatchSchemaTable.Columns.Add("SupplierID", typeof(string));
            s_codeMatchSchemaTable.Columns.Add("SupplierCode", typeof(string));
            s_codeMatchSchemaTable.Columns.Add("SupplierCodeID", typeof(long));
            s_codeMatchSchemaTable.Columns.Add("SupplierZoneID", typeof(int));

            s_supplierCodeMatchSchemaTable = new DataTable();
            s_supplierCodeMatchSchemaTable.TableName = "LCR.CodeMatch";

            s_supplierCodeMatchSchemaTable.Columns.Add("Code", typeof(string));
            s_supplierCodeMatchSchemaTable.Columns.Add("SupplierCode", typeof(string));
            s_supplierCodeMatchSchemaTable.Columns.Add("SupplierCodeID", typeof(long));
            s_supplierCodeMatchSchemaTable.Columns.Add("SupplierZoneID", typeof(int));
        }


        #endregion

        //public List<SupplierCodeMatch> GetSupplierCodeMatches(string supplierId, bool isFuture)
        //{
        //    return GetItems("LCR.sp_CodeMatch_GetBySupplier", (reader) =>
        //        {
        //            return new SupplierCodeMatch
        //            {
        //                 Code = reader["Code"] as string,
        //                 SupplierCode = reader["SupplierCode"] as string,
        //                 SupplierCodeId = (int)reader["SupplierCodeID"],
        //                 SupplierZoneId = (int)reader["SupplierZoneID"]
        //            };
        //        }, supplierId, isFuture);
        //}

        

        //public void UpdateSupplierCodeMatches(string supplierId, bool isFuture, List<SupplierCodeMatch> codeMatches)
        //{
        //    if (codeMatches.Count < 1)
        //        return;
        //    DataTable dtNewCodeMatch = s_codeMatchSchemaTable.Clone();
        //    dtNewCodeMatch.BeginLoadData();
        //    DataTable dtSupplierCodeMatch = BuildDataTableFromList(codeMatches);

        //    ExecuteReaderWithCmd("LCR.sp_CodeMatch_UpdateFromSupplierCodeMatch",
        //        (reader) =>
        //        {
        //            while (reader.Read())
        //            {
        //                DataRow row = dtNewCodeMatch.NewRow();
        //                row["Code"] = reader["Code"];
        //                row["SupplierID"] = supplierId;
        //                row["SupplierCode"] = reader["SupplierCode"];
        //                row["SupplierCodeID"] = reader["SupplierCodeID"];
        //                row["SupplierZoneID"] = reader["SupplierZoneID"];
        //                row["IsFuture"] = isFuture;
        //                dtNewCodeMatch.Rows.Add(row);
        //            }
        //        },
        //        (cmd) =>
        //        {
        //            cmd.Parameters.Add(new SqlParameter("@SupplierID", supplierId));
        //            cmd.Parameters.Add(new SqlParameter("@IsFuture", isFuture));
        //            var dtPrm = new SqlParameter("@SupplierCodeMatch", SqlDbType.Structured);
        //            dtPrm.Value = dtSupplierCodeMatch;
        //            cmd.Parameters.Add(dtPrm);
        //        });
        //    dtNewCodeMatch.EndLoadData();
        //    if (dtNewCodeMatch.Rows.Count > 0)
        //        WriteDataTableToDB(dtNewCodeMatch, SqlBulkCopyOptions.KeepNulls);
        //}

        public DataTable BuildCodeMatchSchemaTable(bool isFuture, string codeGroup)
        {
            DataTable dt = s_codeMatchSchemaTable.Clone();
            dt.TableName = String.Format("LCR.CodeMatch{0}{1}_temp", isFuture ? "Future" : "Current", codeGroup);
            dt.BeginLoadData();
            return dt;
        }

        public DataTable BuildCodeMatchSchemaTable(bool isFuture)
        {
            DataTable dt = s_codeMatchSchemaTable.Clone();
            dt.TableName = String.Format("LCR.CodeMatch{0}_temp", isFuture ? "Future" : "Current");
            dt.BeginLoadData();
            return dt;
        }

        public void AddCodeMatchesToTable(List<CodeMatch> codeMatches, DataTable dtCodeMatches)
        {
            foreach (var cm in codeMatches)
            {
                DataRow row = dtCodeMatches.NewRow();
                row["Code"] = cm.Code;
                row["SupplierID"] = cm.SupplierId;
                row["SupplierCode"] = cm.SupplierCode;
                row["SupplierCodeID"] = cm.SupplierCodeId;
                row["SupplierZoneID"] = cm.SupplierZoneId;
                row["IsFuture"] = cm.IsFuture;
                dtCodeMatches.Rows.Add(row);
            }
        }

        public void AddCodeMatchRowToTable(DataTable dtCodeMatches, string code, string supplierId, string supplierCode, long supplierCodeId, int supplierZoneId)
        {
            DataRow row = dtCodeMatches.NewRow();
            row["Code"] = code;
            row["SupplierID"] = supplierId;
            row["SupplierCode"] = supplierCode;
            row["SupplierCodeID"] = supplierCodeId;
            row["SupplierZoneID"] = supplierZoneId;
            dtCodeMatches.Rows.Add(row);
        }

        public void WriteCodeMatchTableToDB(DataTable dtCodeMatches)
        {
            dtCodeMatches.EndLoadData();
            WriteDataTableToDB(dtCodeMatches, SqlBulkCopyOptions.TableLock, false);
        }

        public Object PrepareCodeMatchesForDBApply(List<CodeMatch> codeMatches, bool isFuture)
        {
            //StringBuilder strBuilder = new StringBuilder();
            //foreach (var cm in codeMatches)
            //{
            //    strBuilder.AppendLine(String.Format("{0},{1},{2},{3},{4}", cm.Code, cm.SupplierId, cm.SupplierCode, cm.SupplierCodeId, cm.SupplierZoneId));
            //}
            //string filePath = CreateFileForBulkInsert(strBuilder.ToString());
            string filePath = GetFilePathForBulkInsert();
            using (System.IO.StreamWriter wr = new System.IO.StreamWriter(filePath))
            {
                foreach (var cm in codeMatches)
                {
                    wr.WriteLine(String.Format("{0},{1},{2},{3},{4}", cm.Code, cm.SupplierId, cm.SupplierCode, cm.SupplierCodeId, cm.SupplierZoneId));
                }
                wr.Close();
            }
            string tableName = String.Format("LCR.CodeMatch{0}_temp", isFuture ? "Future" : "Current");
            return new BulkInsertInfo
            {
                TableName = String.Format("LCR.CodeMatch{0}_temp", isFuture ? "Future" : "Current"),
                DataFilePath = filePath,
                TabLock = true,
                FieldSeparator = ','
            };
        }

        public void ApplyCodeMatchesToDB(Object preparedCodeMatches)
        {
            InsertBulkToTable(preparedCodeMatches as BulkInsertInfo);
        }

        public void WriteCodeMatchesDB(List<CodeMatch> codeMatches, bool isFuture)
        {
            string filePath = String.Format(@"C:\CodeMatch\{0}.txt", Guid.NewGuid());
            string errorFilePath = String.Format(@"C:\CodeMatch\Error\{0}.txt", Guid.NewGuid());
            
            StringBuilder strBuilder = new StringBuilder();
            foreach (var cm in codeMatches)
            {
                strBuilder.AppendLine(String.Format("{0},{1},{2},{3},{4}", cm.Code, cm.SupplierId, cm.SupplierCode, cm.SupplierCodeId, cm.SupplierZoneId));
            }
            System.IO.File.WriteAllText(filePath, strBuilder.ToString());
            //using (System.IO.StreamWriter wr = new System.IO.StreamWriter(filePath))
            //{
            //    foreach (var cm in codeMatches)
            //    {
            //        wr.WriteLine(String.Format("{0},{1},{2},{3},{4}", cm.Code, cm.SupplierId, cm.SupplierCode, cm.SupplierCodeId, cm.SupplierZoneId));
            //    }
            //    wr.Close();
            //}
            System.Diagnostics.Process processBulkCopy = new System.Diagnostics.Process();
            string tableName = String.Format("LCR.CodeMatch{0}_temp", isFuture ? "Future" : "Current");
            string bulkCopyArgs = String.Format("{0} in {1} -e {2} -c -d ToneSpactronNew -S 192.168.110.180\\redundant  -U sa -P development@cce$$ -F2 -t , -b 100000 -h TABLOCK", tableName, filePath, errorFilePath);
            var procStartInfo = new System.Diagnostics.ProcessStartInfo("bcp", bulkCopyArgs);
            procStartInfo.RedirectStandardOutput = true;
            procStartInfo.UseShellExecute = false;
            procStartInfo.CreateNoWindow = true;
            processBulkCopy.StartInfo = procStartInfo;
            processBulkCopy.Start();
            processBulkCopy.WaitForExit();
        }

        //public void UpdateSupplierCodeMatches(List<CodeMatch> codeMatches)
        //{
        //    DataTable dt = BuildDataTableFromList(codeMatches);
        //    WriteDataTableToDB(dt, SqlBulkCopyOptions.TableLock | SqlBulkCopyOptions.FireTriggers | SqlBulkCopyOptions.UseInternalTransaction, false);
        //}

        public void CreateTempTable(bool isFuture)
        {
            ExecuteNonQueryText(String.Format(query_CreateTempTable, isFuture ? "Future" : "Current", null), null);
            //ExecuteNonQueryCmdText(String.Format(query_CreateIndexesOnTempTable, isFuture ? "Future" : "Current", null), null);
        }

        public void CreateTempTable(bool isFuture, string codeGroup)
        {
            ExecuteNonQueryText(String.Format(query_CreateTempTable, isFuture ? "Future" : "Current", codeGroup), null);
            //ExecuteNonQueryCmdText(String.Format(query_CreateIndexesOnTempTable, isFuture ? "Future" : "Current", codeGroup), null);
        }

        public void SwapTableWithTemp(bool isFuture)
        {
            ExecuteNonQueryText(String.Format(query_SwapTableWithTemp, isFuture ? "Future" : "Current", null), null);
        }

        public void SwapTableWithTemp(bool isFuture, string codeGroup)
        {
            ExecuteNonQueryText(String.Format(query_SwapTableWithTemp, isFuture ? "Future" : "Current", codeGroup), null);
        }

        public void CreateIndexesOnTable(bool isFuture)
        {
            ExecuteNonQueryText(String.Format(query_CreateIndexesOnTempTable, isFuture ? "Future" : "Current", null), null);
        }

        public void CreateIndexesOnTable(bool isFuture, string codeGroup)
        {
            ExecuteNonQueryText(String.Format(query_CreateIndexesOnTempTable, isFuture ? "Future" : "Current", codeGroup), null);
        }

        #region Private Methods

        public void FillCodeMatchesFromCodes(TOne.Entities.CodeList distinctCodes, List<SupplierCodeInfo> suppliersCodeInfo, DateTime effectiveOn)
        {
            DateTime start = DateTime.Now;
            Console.WriteLine("{0}: Code Match Creation started", DateTime.Now);
            DataTable dtDistinctCodesWithPossibleMatches = CodeDataManager.BuildDistinctCodesWithPossibleValuesTable(distinctCodes);

            DataTable dtSuppliersCodeInfo = CodeDataManager.BuildSuppliersCodeInfoTable(suppliersCodeInfo);

            ExecuteNonQueryText(query_FillCodeMatchesFromCodes, (cmd) =>
                {
                    cmd.Parameters.Add(new SqlParameter("@EffectiveOn", effectiveOn));
                    var dtPrm = new SqlParameter("@DistinctCodesWithPossibleMatches", SqlDbType.Structured);
                    dtPrm.Value = dtDistinctCodesWithPossibleMatches;
                    dtPrm.TypeName = "LCR.DistinctCodeWithPossibleMatchTable";
                    cmd.Parameters.Add(dtPrm);

                    dtPrm = new SqlParameter("@ActiveSuppliersCodeInfo", SqlDbType.Structured);
                    dtPrm.Value = dtSuppliersCodeInfo;
                    dtPrm.TypeName = "LCR.SuppliersCodeInfoType";
                    cmd.Parameters.Add(dtPrm);
                });
            Console.WriteLine("{0}: Code Match Creation done in {1}", DateTime.Now, (DateTime.Now - start));
        }

        #endregion

        #region Queries

        const string query_CreateTempTable = @"IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[LCR].[CodeMatch{0}{1}_old]') AND type in (N'U'))
		                                    DROP TABLE [LCR].[CodeMatch{0}{1}_old]

		                                    IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[LCR].[CodeMatch{0}{1}_temp]') AND type in (N'U'))
		                                    DROP TABLE [LCR].[CodeMatch{0}{1}_temp]
		

		                                    CREATE TABLE [LCR].[CodeMatch{0}{1}_temp](
			                                    [Code] [varchar](30) NOT NULL,
			                                    [SupplierID] [varchar](5) NOT NULL,
			                                    [SupplierCode] [varchar](30) NOT NULL,
			                                    [SupplierCodeID] [bigint] NOT NULL,
			                                    [SupplierZoneID] [int] NOT NULL
		                                    ) ON [PRIMARY]";

        const string query_SwapTableWithTemp = @"BEGIN TRANSACTION
                                                IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[LCR].[CodeMatch{0}{1}]') AND type in (N'U'))
                                                EXEC sp_rename 'LCR.CodeMatch{0}{1}', 'CodeMatch{0}{1}_Old'
		                                        EXEC sp_rename 'LCR.CodeMatch{0}{1}_Temp', 'CodeMatch{0}{1}'
                                                COMMIT TRANSACTION";

        const string query_CreateIndexesOnTempTable = @"CREATE CLUSTERED INDEX [IX_CodeMatch_CodeSupplierID] ON [LCR].[CodeMatch{0}{1}_temp] 
		                                                (
			                                                [Code] ASC,
			                                                [SupplierID] ASC
		                                                )
		                                               /* CREATE NONCLUSTERED INDEX [IX_CodeMatch_Code] ON [LCR].[CodeMatch{0}{1}_temp] 
		                                                (
			                                                [Code] ASC
		                                                )
		                                                CREATE NONCLUSTERED INDEX [IX_CodeMatch_SupplierID] ON [LCR].[CodeMatch{0}{1}_temp] 
		                                                (
			                                                [SupplierID] ASC
		                                                )
		                                                CREATE NONCLUSTERED INDEX [IX_CodeMatch_SZoneID] ON [LCR].[CodeMatch{0}{1}_temp] 
		                                                (
			                                                [SupplierZoneID] ASC
		                                                )*/ ";

        const string query_FillCodeMatchesFromCodes = @"
	WITH AllCodeMatches AS
	(
		SELECT  distinctCodes.DistinctCode, Z.SupplierID, c.Code SupplierCode, c.ID SupplierCodeID, C.ZoneID SupplierZoneID
		, ROW_NUMBER() OVER (PARTITION BY Z.SupplierID, distinctCodes.DistinctCode ORDER BY distinctCodes.PossibleMatch DESC) RowNumber
		FROM @DistinctCodesWithPossibleMatches distinctCodes 
		JOIN Code c WITH (NOLOCK) on distinctCodes.PossibleMatch = c.Code
		JOIN Zone z WITH (NOLOCK) on c.ZoneID = z.ZoneID
		JOIN @ActiveSuppliersCodeInfo sup on z.SupplierID = sup.SupplierID
		WHERE c.BeginEffectiveDate <= @EffectiveOn
			AND (c.EndEffectiveDate IS NULL OR c.EndEffectiveDate > @EffectiveOn)
	)
	SELECT * INTO LCR.CodeMatchCurrentTest2 FROM AllCodeMatches WHERE RowNumber = 1";

        const string query_GetDistinctCodes = @"	
		        SELECT distinct Code 
                FROM [LCR].[CodeMatch{0}] cm
                JOIN @ActiveSuppliersCodeInfo sup on cm.SupplierID = sup.SupplierID	";

        const string query_CopyCodeMatchTableWithValidItems = @"IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[LCR].[CodeMatch{0}_Temp]') AND type in (N'U'))
		                                                        DROP TABLE [LCR].[CodeMatch{0}_Temp]

                                                                SELECT cm.* INTO LCR.CodeMatch{0}_Temp 
                                                                FROM LCR.CodeMatch{0} cm
                                                                JOIN @DistinctCodes distinctCodes ON cm.Code = distinctCodes.DistinctCode
                                                                JOIN @ActiveSuppliersCodeInfo sup on cm.SupplierID = sup.SupplierID
                                                                WHERE sup.HasUpdatedCodes = 0";

        #endregion


        public List<string> GetDistinctCodes(bool isFuture, List<SupplierCodeInfo> suppliersCodeInfo)
        {
            DataTable dtSuppliersCodeInfo = CodeDataManager.BuildSuppliersCodeInfoTable(suppliersCodeInfo);

            return GetItemsText(String.Format(query_GetDistinctCodes, isFuture ? "Future" : "Current"), 
                (reader) => reader["Code"] as string,
                (cmd) =>
                {
                    var dtPrm = new SqlParameter("@ActiveSuppliersCodeInfo", SqlDbType.Structured);
                    dtPrm.Value = dtSuppliersCodeInfo;
                    dtPrm.TypeName = "LCR.SuppliersCodeInfoType";
                    cmd.Parameters.Add(dtPrm);
                });
        }

        public void CopyCodeMatchTableWithValidItems(bool isFuture, TOne.Entities.CodeList distinctCodes, List<SupplierCodeInfo> suppliersCodeInfo)
        {
            DataTable dtDistinctCodes = CodeDataManager.BuildDistinctCodesTable(distinctCodes);

            DataTable dtSuppliersCodeInfo = CodeDataManager.BuildSuppliersCodeInfoTable(suppliersCodeInfo);

            ExecuteNonQueryText(String.Format(query_CopyCodeMatchTableWithValidItems, isFuture ? "Future" : "Current"),
                (cmd) =>
                {
                    var dtPrm = new SqlParameter("@DistinctCodes", SqlDbType.Structured);
                    dtPrm.Value = dtDistinctCodes;
                    dtPrm.TypeName = "LCR.DistinctCodesType";
                    cmd.Parameters.Add(dtPrm);

                    dtPrm = new SqlParameter("@ActiveSuppliersCodeInfo", SqlDbType.Structured);
                    dtPrm.Value = dtSuppliersCodeInfo;
                    dtPrm.TypeName = "LCR.SuppliersCodeInfoType";
                    cmd.Parameters.Add(dtPrm);
                });
        }
        

        #region Private Methods

        internal static CodeMatch CodeMatchMapper(IDataReader reader)
        {
            return new CodeMatch
            {
                Code = reader["Code"] as string,
                SupplierCode = reader["SupplierCode"] as string,
                SupplierId = reader["SupplierID"] as string,
                SupplierCodeId = (long)reader["SupplierCodeId"],
                SupplierZoneId = (int)reader["SupplierZoneId"]
            };
        }       
        
        private DataTable BuildDataTableFromList(List<CodeMatch> codeMatches)
        {
            DataTable dt = s_codeMatchSchemaTable.Clone();
            dt.BeginLoadData();
            foreach (var cm in codeMatches)
            {
                DataRow row = dt.NewRow();
                row["Code"] = cm.Code;
                row["SupplierID"] = cm.SupplierId;
                row["SupplierCode"] = cm.SupplierCode;
                row["SupplierCodeID"] = cm.SupplierCodeId;
                row["SupplierZoneID"] = cm.SupplierZoneId;
                row["IsFuture"] = cm.IsFuture;
                dt.Rows.Add(row);
            }
            dt.EndLoadData();
            return dt;
        }


        #endregion

    }
}
