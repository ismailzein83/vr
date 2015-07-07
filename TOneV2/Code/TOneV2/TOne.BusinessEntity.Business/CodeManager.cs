using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.BusinessEntity.Data;
using TOne.BusinessEntity.Entities;

namespace TOne.BusinessEntity.Business
{
    public class CodeManager
    {
        ICodeDataManager _dataManager;

        public CodeManager()
        {
            _dataManager = BEDataManagerFactory.GetDataManager<ICodeDataManager>();
        }

        public Dictionary<string, Dictionary<string, Code>> GetCodes(Char firstDigit, DateTime effectiveOn, bool isFuture,
            List<CarrierAccountInfo> activeSuppliers, out List<string> distinctCodes)
        {
            return _dataManager.GetCodes(firstDigit, effectiveOn, isFuture, activeSuppliers, out distinctCodes);
        }

        public SuppliersCodes GetCodesByCodePrefixGroup(String codePrefixGroup, DateTime effectiveOn, bool isFuture,
       List<CarrierAccountInfo> activeSuppliers, out List<string> distinctCodes, out HashSet<Int32> supplierZoneIds, out HashSet<Int32> saleZoneIds)
        {
            return _dataManager.GetCodesByCodePrefixGroup(codePrefixGroup, effectiveOn, isFuture, activeSuppliers, out distinctCodes, out supplierZoneIds, out  saleZoneIds);
        }

        public List<CodeGroupInfo> GetCodeGroups()
        {
            return _dataManager.GetCodeGroups();
        }

        public List<Code> GetCodes(int zoneID, DateTime effectiveOn)
        {
            return _dataManager.GetCodes(zoneID, effectiveOn);
        }

        public Dictionary<string, CodeGroupInfo> GetCodeGroupsByCodePrefix(string codePrefix)
        {
            return _dataManager.GetCodeGroupsByCodePrefix(codePrefix);
        }

        public string GetCodeGroupName(int codeGroupId)
        {
            return _dataManager.GetCodeGroupName(codeGroupId);
        }
    }
}
