using System;
using TOne.Entities;
using System.Collections.Generic;
using System.Data;
namespace TOne.Data
{
    public interface ICodeMatchDataManager
    {
        //void UpdateSupplierCodeMatches(List<CodeMatch> codeMatches);
        DataTable BuildCodeMatchSchemaTable(bool isFuture);
        //DataTable BuildCodeMatchSchemaTable(bool isFuture, string codeGroup);

        void AddCodeMatchesToTable(List<CodeMatch> codeMatches, DataTable dtCodeMatches);
        void AddCodeMatchRowToTable(DataTable dtCodeMatches, string code, string supplierId, string supplierCode, long supplierCodeId, int supplierZoneId);

        void WriteCodeMatchTableToDB(DataTable dtCodeMatches);

        void CreateTempTable(bool isFuture);
        //void CreateTempTable(bool isFuture, string codeGroup);

        void SwapTableWithTemp(bool isFuture);
        //void SwapTableWithTemp(bool isFuture, string codeGroup);

        void CreateIndexesOnTable(bool isFuture);
        //void CreateIndexesOnTable(bool isFuture, string codeGroup);
    }
}
