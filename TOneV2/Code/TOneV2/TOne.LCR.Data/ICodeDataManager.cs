using System;
using System.Collections.Generic;
using TOne.Entities;
using TOne.LCR.Entities;
namespace TOne.LCR.Data
{
    public interface ICodeDataManager : IDataManager
    {
        List<string> GetUpdatedCodesSuppliers(byte[] updatedAfter, out byte[] newLastTimestamp);
        List<String> GetDistinctCodes(bool isFuture);
        //List<String> GetDistinctCodes(bool isFuture, char firstDigit);
        //List<String> GetDistinctCodes(bool isFuture, string codeGroup);
        //List<String> GetDistinctCodes(bool isFuture, DateTime effectiveOn, bool getChangedGroupsOnly);

        List<LCRCode> GetSupplierCodes(string supplierId, DateTime effectiveOn);
        void LoadCodesByUpdatedSuppliers(byte[] codeUpdatedAfter, DateTime effectiveOn, Action<string, List<LCRCode>> onSupplierCodesReady);
        void LoadCodesForActiveSuppliers(bool isFuture, Action<string, List<LCRCode>> onSupplierCodesReady);
        void LoadCodesForActiveSuppliers(DateTime effectiveOn, List<SupplierCodeInfo> suppliersCodeInfo, bool onlySuppliersWithUpdatedCodes, Action<string, List<LCRCode>> onSupplierCodesRead);
        //void LoadCodesForActiveSuppliers(bool isFuture, char firstDigit, Action<string, List<LCRCode>> onSupplierCodesReady);
        //void LoadCodesForActiveSuppliers(bool isFuture, string codeGroup, Action<string, List<LCRCode>> onSupplierCodesReady);
        //void LoadCodesForActiveSuppliers(bool isFuture, DateTime effectiveOn, bool getChangedGroupsOnly, Action<string, List<LCRCode>> onSupplierCodesReady);
        Dictionary<string, Dictionary<string, LCRCode>> GetOrderedCodesForActiveSuppliers(bool isFuture);

        List<SupplierCodeInfo> GetActiveSupplierCodeInfo(DateTime effectiveAfter, DateTime effectiveOn);

        List<string> GetDistinctCodes(List<SupplierCodeInfo> suppliersCodeInfo, DateTime effectiveOn);

        void LoadCodeMatchesFromDistinctCodes(CodeList distinctCodes, DateTime effectiveOn, List<SupplierCodeInfo> suppliersCodeInfo, Action<CodeMatch> onCodeMatchReady);
    }
}
