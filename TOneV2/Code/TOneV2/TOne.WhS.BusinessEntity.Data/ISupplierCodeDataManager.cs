﻿using System;
using System.Collections.Generic;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Entities;

namespace TOne.WhS.BusinessEntity.Data
{
    public interface ISupplierCodeDataManager:IDataManager
    {
        IEnumerable<SupplierCode> GetFilteredSupplierCodes(SupplierCodeQuery query);

        List<SupplierCode> GetSupplierCodesEffectiveAfter(int supplierId, DateTime minimumDate);
        List<SupplierCode> GetSupplierCodes(int supplierId, DateTime effectiveOn);

        List<SupplierCode> GetActiveSupplierCodesByPrefix(string codePrefix, DateTime? effectiveOn, bool isFuture, bool getChildCodes, bool getParentCodes, IEnumerable<RoutingSupplierInfo> supplierInfo);

        IEnumerable<string> GetDistinctCodeByPrefixes(int prefixLength, DateTime? effectiveOn, bool isFuture);

        bool AreSupplierCodesUpdated(ref object updateHandle);
    }
}
