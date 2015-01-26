using System;
using TOne.LCR.Entities;
using System.Collections.Generic;
using System.Data;
using TOne.Entities;
namespace TOne.LCR.Data
{
    public interface ICodeMatchDataManager : IDataManager
    {
        //void UpdateSupplierCodeMatches(List<CodeMatch> codeMatches);
        DataTable BuildCodeMatchSchemaTable(bool isFuture);
        //DataTable BuildCodeMatchSchemaTable(bool isFuture, string codeGroup);

        void AddCodeMatchesToTable(List<CodeMatch> codeMatches, DataTable dtCodeMatches);
        void AddCodeMatchRowToTable(DataTable dtCodeMatches, string code, string supplierId, string supplierCode, long supplierCodeId, int supplierZoneId);

        void WriteCodeMatchTableToDB(DataTable dtCodeMatches);

        void WriteCodeMatchesDB(List<CodeMatch> codeMatches, bool isFuture);

        Object PrepareCodeMatchesForDBApply(List<CodeMatch> codeMatches, bool isFuture);

        void ApplyCodeMatchesToDB(Object preparedCodeMatches);

        void CreateTempTable(bool isFuture);
        //void CreateTempTable(bool isFuture, string codeGroup);

        void SwapTableWithTemp(bool isFuture);
        //void SwapTableWithTemp(bool isFuture, string codeGroup);

        void CreateIndexesOnTable(bool isFuture);
        //void CreateIndexesOnTable(bool isFuture, string codeGroup);

        List<string> GetDistinctCodes(bool isFuture, List<SupplierCodeInfo> suppliersCodeInfo);

        void CopyCodeMatchTableWithValidItems(bool isFuture, CodeList distinctCodes, List<SupplierCodeInfo> suppliersCodeInfo);

        
    }
}
