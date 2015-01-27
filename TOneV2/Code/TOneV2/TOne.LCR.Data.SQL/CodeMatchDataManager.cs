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
        public Object PrepareCodeMatchesForDBApply(List<CodeMatch> codeMatches, bool isFuture)
        {
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
        
        public void CreateTempTable(bool isFuture)
        {
            ExecuteNonQueryText(String.Format(query_CreateTempTable, isFuture ? "Future" : "Current"), null);
        }
        
        public void SwapTableWithTemp(bool isFuture)
        {
            ExecuteNonQueryText(String.Format(query_SwapTableWithTemp, isFuture ? "Future" : "Current"), null);
        }
        
        public void CreateIndexesOnTable(bool isFuture)
        {
            ExecuteNonQueryText(String.Format(query_CreateIndexesOnTempTable, isFuture ? "Future" : "Current"), null);
        }
        
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
            CreateTempTable(isFuture);
            DataTable dtDistinctCodes = CodeDataManager.BuildDistinctCodesTable(distinctCodes);

            DataTable dtSuppliersCodeInfo = CodeDataManager.BuildSuppliersCodeInfoTable(suppliersCodeInfo);

            ExecuteNonQueryText(String.Format(query_CopyCodeMatchTableWithValidItems2, isFuture ? "Future" : "Current"),
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

        #endregion

        #region Queries

        const string query_CreateTempTable = @"IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[LCR].[CodeMatch{0}_old]') AND type in (N'U'))
		                                    DROP TABLE [LCR].[CodeMatch{0}_old]

		                                    IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[LCR].[CodeMatch{0}_temp]') AND type in (N'U'))
		                                    DROP TABLE [LCR].[CodeMatch{0}_temp]
		

		                                    CREATE TABLE [LCR].[CodeMatch{0}_temp](
			                                    [Code] [varchar](30) NOT NULL,
			                                    [SupplierID] [varchar](5) NOT NULL,
			                                    [SupplierCode] [varchar](30) NOT NULL,
			                                    [SupplierCodeID] [bigint] NOT NULL,
			                                    [SupplierZoneID] [int] NOT NULL
		                                    ) ON [PRIMARY]";

        const string query_SwapTableWithTemp = @"BEGIN TRANSACTION
                                                IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[LCR].[CodeMatch{0}]') AND type in (N'U'))
                                                    EXEC sp_rename 'LCR.CodeMatch{0}', 'CodeMatch{0}_Old'
		                                        EXEC sp_rename 'LCR.CodeMatch{0}_Temp', 'CodeMatch{0}'
                                                COMMIT TRANSACTION";

        const string query_CreateIndexesOnTempTable = @"/*CREATE CLUSTERED INDEX [IX_CodeMatch_CodeSupplierID] ON [LCR].[CodeMatch{0}_temp] 
		                                                (
			                                                [Code] ASC,
			                                                [SupplierID] ASC
		                                                )*/
		                                                CREATE NONCLUSTERED INDEX [IX_CodeMatch_Code] ON [LCR].[CodeMatch{0}_temp] 
		                                                (
			                                                [Code] ASC
		                                                )
		                                                /*CREATE NONCLUSTERED INDEX [IX_CodeMatch_SupplierID] ON [LCR].[CodeMatch{0}_temp] 
		                                                (
			                                                [SupplierID] ASC
		                                                )
		                                                CREATE NONCLUSTERED INDEX [IX_CodeMatch_SZoneID] ON [LCR].[CodeMatch{0}_temp] 
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

        const string query_GetDistinctCodes = @"	IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[LCR].[CodeMatch{0}]') AND type in (N'U'))
                                                        SELECT '' WHERE 1 = 2
                                                    ELSE
		                                                SELECT distinct Code 
                                                        FROM [LCR].[CodeMatch{0}] cm
                                                        JOIN @ActiveSuppliersCodeInfo sup on cm.SupplierID = sup.SupplierID	";

        const string query_CopyCodeMatchTableWithValidItems = @"IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[LCR].[CodeMatch{0}_Old]') AND type in (N'U'))
		                                                        DROP TABLE [LCR].[CodeMatch{0}_Old]

		                                                        IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[LCR].[CodeMatch{0}_Temp]') AND type in (N'U'))
		                                                        DROP TABLE [LCR].[CodeMatch{0}_Temp]

                                                                SELECT cm.* INTO LCR.CodeMatch{0}_Temp 
                                                                FROM LCR.CodeMatch{0} cm
                                                                JOIN @DistinctCodes distinctCodes ON cm.Code = distinctCodes.DistinctCode
                                                                JOIN @ActiveSuppliersCodeInfo sup on cm.SupplierID = sup.SupplierID
                                                                WHERE sup.HasUpdatedCodes = 0";

        const string query_CopyCodeMatchTableWithValidItems2 = @"INSERT INTO LCR.CodeMatch{0}_Temp WITH (TABLOCK)
                                                                SELECT cm.*
                                                                FROM LCR.CodeMatch{0} cm
                                                                JOIN @DistinctCodes distinctCodes ON cm.Code = distinctCodes.DistinctCode
                                                                JOIN @ActiveSuppliersCodeInfo sup on cm.SupplierID = sup.SupplierID
                                                                WHERE sup.HasUpdatedCodes = 0";

        #endregion
    }
}
