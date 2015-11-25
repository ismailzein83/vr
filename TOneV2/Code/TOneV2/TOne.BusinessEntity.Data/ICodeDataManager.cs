using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.BusinessEntity.Entities;

namespace TOne.BusinessEntity.Data
{
    public interface ICodeDataManager : IDataManager
    {
        Dictionary<string, Dictionary<string, Code>> GetCodes(Char firstDigit, DateTime effectiveOn, bool isFuture, List<CarrierAccountInfo> activeSuppliers, out List<string> distinctCodes);


        SuppliersCodes GetCodesByCodePrefixGroup(String codePrefixGroup, DateTime effectiveOn, bool isFuture, List<CarrierAccountInfo> activeSuppliers, out List<string> distinctCodes, out HashSet<Int32> supplierZoneIds, out HashSet<Int32> saleZoneIds);


        List<CodeGroupInfo> GetCodeGroups();
        List<CodeGroupInfo> GetCodeGroupsByCustomer(string customerId);

        Dictionary<string, CodeGroupInfo> GetCodeGroupsByCodePrefix(string codePrefix);

        List<string> GetDistinctCodePrefixes(int codePrefixLength, DateTime effectiveOn, bool isFuture);
        List<Code> GetCodes(int zoneID, DateTime effectiveOn);
        string GetCodeGroupName(int codeGroupId);
        List<Code> GetSupplierCodes(string supplierId, char rootCode, DateTime whenEffective);
    }
}
