using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.BusinessEntity.Data;
using TOne.BusinessEntity.Entities;
using TOne.Caching;
using TOne.Entities;

namespace TOne.BusinessEntity.Business
{
    public class CodeManager
    {
        ICodeDataManager _dataManager;

        TOneCacheManager _cacheManager;

        public CodeManager(TOneCacheManager cacheManager)
        {
            _cacheManager = cacheManager;
            _dataManager = BEDataManagerFactory.GetDataManager<ICodeDataManager>();
        }


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
        public Dictionary<string,CodeGroupInfo> GetCodeGroupsDictionary()
        {
            List<CodeGroupInfo> codeGroups = _cacheManager.GetOrCreateObject("CodeGroups",
            CacheObjectType.CodeGroup,
             () =>
            {
                return GetCodeGroups();
            });
            Dictionary<string, CodeGroupInfo> codeGroupsDictionary = new Dictionary<string, CodeGroupInfo>();
            foreach (CodeGroupInfo codeGroup in codeGroups)
            {
                CodeGroupInfo codeGroupInfo = null;
                if (!codeGroupsDictionary.TryGetValue(codeGroup.Code,out codeGroupInfo))
                codeGroupsDictionary.Add(codeGroup.Code, codeGroup);
            }
            return codeGroupsDictionary;
        }
        public CodeGroupInfo GetCodeGroupById(string codeGroupId)
        {
            CodeGroupInfo codeGroup;
            var allCodeGroups = GetCodeGroupsDictionary();
            if (allCodeGroups != null)
                allCodeGroups.TryGetValue(codeGroupId, out codeGroup);
            else
                codeGroup = null;
            return codeGroup;
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
        public Dictionary<string, Code> GetSupplierCodes(string supplierId, char rootCode, DateTime whenEffective)
        {
            return _cacheManager.GetOrCreateObject(String.Format("GetEffectiveGetSupplierCodes_{0}_{1}_{2:ddMMMyy}", supplierId, rootCode, whenEffective.Date),
           CacheObjectType.SupplierCodes,
           () =>
           {
               return GetGetSupplierCodesFromDB(supplierId, rootCode, whenEffective);
           });
        }
        private Dictionary<string, Code> GetGetSupplierCodesFromDB(string supplierId, char rootCode, DateTime when)
        {
            ICodeDataManager _dataManager=_dataManager = BEDataManagerFactory.GetDataManager<ICodeDataManager>();
            List<Code> supplierCodes = _dataManager.GetSupplierCodes(supplierId, rootCode, when);
            Dictionary<string, Code> supplierCodesDictionary = new Dictionary<string, Code>();

            foreach (Code supplierCode in supplierCodes)
            {
                Code value=null;
                if (!supplierCodesDictionary.TryGetValue(supplierCode.Value, out value))
                    supplierCodesDictionary.Add(supplierCode.Value, supplierCode);
            }
            
            return supplierCodesDictionary;
        }
        public string GetDigits(string codeValue)
        {
            StringBuilder sb = new StringBuilder(codeValue.Length);
            for (int i = 0; i < codeValue.Length; i++)
            {
                char c = codeValue[i];
                if (char.IsDigit(c)) sb.Append(c);
            }
            return sb.ToString();
        }
    }
}
