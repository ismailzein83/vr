using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Data
{
    public interface ISaleCodeDataManager : IDataManager
    {
        IEnumerable<SaleCode> GetSaleCodesByCode(string codeNumber);
        IEnumerable<SaleCode> GetAllSaleCodes();

        IEnumerable<SaleCode> GetFilteredSaleCodes(SaleCodeQuery query);

        IEnumerable<SaleCode> GetSaleCodesByZone(SaleCodeQueryByZone query);

        List<SaleCode> GetSaleCodesByZoneID(long zoneID, DateTime effectiveDate);

        List<SaleCode> GetSaleCodesByCodeGroups(List<int> codeGroupsIds);
        List<SaleCode> GetSaleCodesByCodeId(IEnumerable<long> codeIds);
        List<SaleCode> GetSaleCodesEffectiveByZoneID(long zoneID, DateTime effectiveDate);

        List<SaleCode> GetSaleCodes(DateTime effectiveOn);

        List<SaleCode> GetSaleCodesEffectiveAfter(int sellingNumberPlanId, DateTime effectiveOn, long? processInstanceId);

        List<SaleCode> GetSaleCodesByCountry(int countryId, DateTime effectiveDate);

        List<SaleCode> GetSaleCodesByPrefix(string codePrefix, DateTime? effectiveOn, bool isFuture, bool getChildCodes, bool getParentCodes);

        IEnumerable<CodePrefixInfo> GetDistinctCodeByPrefixes(int prefixLength, DateTime? effectiveOn, bool isFuture);

        IEnumerable<CodePrefixInfo> GetSpecificCodeByPrefixes(int prefixLength, IEnumerable<string> codePrefixes, DateTime? effectiveOn, bool isFuture);

        List<SaleCode> GetSaleCodesByZoneName(int sellingNumberPlanId, string zoneName, DateTime effectiveDate);

        bool AreZonesUpdated(ref object updateHandle);

        List<SaleCode> GetSaleCodesEffectiveAfter(int sellingNumberPlanId, int countryId, DateTime minimumDate);

        List<SaleCode> GetSaleCodesByZoneIDs(List<long> zoneIds, DateTime effectiveDate);

        bool AreSaleCodesUpdated(ref object updateHandle);
    }
}
