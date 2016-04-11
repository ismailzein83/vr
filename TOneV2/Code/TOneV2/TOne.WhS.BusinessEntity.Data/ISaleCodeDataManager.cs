﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Data
{
    public interface ISaleCodeDataManager : IDataManager
    {
        IEnumerable<SaleCode> GetAllSaleCodes();

        Vanrise.Entities.BigResult<Entities.SaleCode> GetSaleCodeFilteredFromTemp(Vanrise.Entities.DataRetrievalInput<Entities.SaleCodeQuery> input);
        List<SaleCode> GetSaleCodesByZoneID(long zoneID, DateTime effectiveDate);

        List<SaleCode> GetSaleCodesAllByZoneID(long zoneID, DateTime effectiveDate);

        List<SaleCode> GetSellingNumberPlanSaleCodes(int sellingNumberPlanId, DateTime effectiveOn);

        List<SaleCode> GetSaleCodesByCountry(int countryId, DateTime effectiveDate);

        List<SaleCode> GetSaleCodesByPrefix(string codePrefix, DateTime? effectiveOn, bool isFuture, bool getChildCodes, bool getParentCodes);

        IEnumerable<string> GetDistinctCodeByPrefixes(int prefixLength, DateTime? effectiveOn, bool isFuture);
        List<SaleCode> GetSaleCodesByZoneName(int sellingNumberPlanId, string zoneName, DateTime effectiveDate);
        bool AreZonesUpdated(ref object updateHandle);
        List<SaleCode> GetSaleCodesEffectiveAfter(int sellingNumberPlanId, int countryId, DateTime minimumDate);

        bool AreSaleCodesUpdated(ref object updateHandle);
    }
}
