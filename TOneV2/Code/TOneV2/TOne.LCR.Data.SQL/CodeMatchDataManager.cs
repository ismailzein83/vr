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
        public class DBApplyPrepareInfo
        {
            public string FilePath { get; set; }
            public System.IO.StreamWriter StreamWriter { get; set; }
        }

        public object InitialiazeStreamForDBApply()
        {
            string filePath = GetFilePathForBulkInsert();
            return new DBApplyPrepareInfo
            {
                FilePath = filePath,
                StreamWriter = new System.IO.StreamWriter(filePath)
            };
        }

        public void WriteCodeMatchToStream(CodeMatch codeMatch, object stream)
        {
            DBApplyPrepareInfo prepareInfo = stream as DBApplyPrepareInfo;
            prepareInfo.StreamWriter.WriteLine(String.Format("{0}^{1}^{2}^{3}^{4}^{5}^{6}^{7}", 
                codeMatch.Code, codeMatch.SupplierId, codeMatch.SupplierCode, codeMatch.SupplierCodeId, codeMatch.SupplierZoneId, 
                codeMatch.SupplierRate != null ? (object)codeMatch.SupplierRate.Rate : null, codeMatch.SupplierRate != null ? (object)codeMatch.SupplierRate.ServicesFlag : null,
                codeMatch.SupplierRate != null ? (object)codeMatch.SupplierRate.PriceListId : null));
        }

        public object FinishDBApplyStream(object stream)
        {
            DBApplyPrepareInfo prepareInfo = stream as DBApplyPrepareInfo;
            prepareInfo.StreamWriter.Close();
            prepareInfo.StreamWriter.Dispose();
            return new BulkInsertInfo
            {
                TableName = "CodeMatch",
                DataFilePath = prepareInfo.FilePath,
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
