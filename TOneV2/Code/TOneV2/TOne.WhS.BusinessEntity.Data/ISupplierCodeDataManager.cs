using System;
using System.Collections.Generic;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Data
{
    public interface ISupplierCodeDataManager : IDataManager
    {
        IEnumerable<SupplierCode> GetSupplierCodesByCode(string codeNumber);
        IEnumerable<SupplierCode> GetFilteredSupplierCodes(SupplierCodeQuery query);

        List<SupplierCode> GetSupplierCodesEffectiveAfter(int supplierId, DateTime minimumDate);

        List<SupplierCode> GetSupplierCodes(DateTime from, DateTime to);


        IEnumerable<CodePrefixInfo> GetDistinctCodeByPrefixes(int prefixLength, DateTime? effectiveOn, bool isFuture);
        IEnumerable<CodePrefixInfo> GetSpecificCodeByPrefixes(int prefixLength, IEnumerable<string> codePrefixes, DateTime? effectiveOn, bool isFuture);

        bool AreSupplierCodesUpdated(ref object updateHandle);

        void LoadSupplierCodes(IEnumerable<RoutingSupplierInfo> activeSupplierInfo, string codePrefix, DateTime? effectiveOn, bool isFuture, Action<SupplierCode> onCodeLoaded);
    }
}
