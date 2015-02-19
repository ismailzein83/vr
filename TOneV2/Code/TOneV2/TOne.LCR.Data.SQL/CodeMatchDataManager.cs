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
        public Object PrepareCodeMatchesForDBApply(List<CodeMatch> codeMatches)
        {
            string filePath = GetFilePathForBulkInsert();
            using (System.IO.StreamWriter wr = new System.IO.StreamWriter(filePath))
            {
                foreach (var cm in codeMatches)
                {
                    wr.WriteLine(String.Format("{0}^{1}^{2}^{3}^{4}", cm.Code, cm.SupplierId, cm.SupplierCode, cm.SupplierCodeId, cm.SupplierZoneId));
                }
                wr.Close();
            }
            //string tableName = String.Format("LCR.CodeMatch{0}_temp", isFuture ? "Future" : "Current");
            return new BulkInsertInfo
            {
                TableName = "CodeMatch",// String.Format("LCR.CodeMatch{0}_temp", isFuture ? "Future" : "Current"),
                DataFilePath = filePath,
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
            InsertBulkToTable(preparedCodeMatches as BulkInsertInfo);
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

        #endregion
    }
}
