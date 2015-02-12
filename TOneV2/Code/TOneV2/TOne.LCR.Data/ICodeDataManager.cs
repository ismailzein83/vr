using System;
using System.Collections.Generic;
using TOne.Entities;
using TOne.LCR.Entities;
namespace TOne.LCR.Data
{
    public interface ICodeDataManager : IDataManager
    {
        void LoadCodesForActiveSuppliers(DateTime effectiveOn, List<SupplierCodeInfo> suppliersCodeInfo, bool onlySuppliersWithUpdatedCodes, Action<string, List<LCRCode>> onSupplierCodesRead);

        List<SupplierCodeInfo> GetActiveSupplierCodeInfo(DateTime effectiveAfter, DateTime effectiveOn);

        List<string> GetDistinctCodes(List<SupplierCodeInfo> suppliersCodeInfo, DateTime effectiveOn);

        void LoadCodeMatchesFromDistinctCodes(CodeList distinctCodes, DateTime effectiveOn, List<SupplierCodeInfo> suppliersCodeInfo, Action<CodeMatch> onCodeMatchReady);

    /// <summary>
    /// Create DistinctCodesTable, Insert Distinct Codes from CodeMatchCurrent, then Create Primary Key Index
    /// </summary>
    /// <returns>Maximum ID</returns>
       int CreateandInsertDistinctCodesTable();
    }
}
