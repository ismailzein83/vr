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


        SuppliersCodes GetCodesByCodePrefixGroup(String codePrefixGroup, DateTime effectiveOn, bool isFuture, List<CarrierAccountInfo> activeSuppliers, out List<string> distinctCodes);


        List<CodeGroupInfo> GetCodeGroups();
    }
}
