using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.BusinessEntity.Business;
using TOne.BusinessEntity.Entities;
using TOne.Caching;

namespace TOne.CDR.Business
{
    public class CodeMap
    {
        TOneCacheManager _cacheManager;

        public CodeMap(TOneCacheManager cacheManager)
        {
            _cacheManager = cacheManager;
        }
        public Code Find(string code, string supplier, DateTime whenEffective)
        {
            CodeManager _codeManager = new CodeManager(_cacheManager);
            if (string.IsNullOrEmpty(code)) return null;
            Code found = null;
            Dictionary<string, CodeGroupInfo> codeGroups = _codeManager.GetCodeGroupsDictionary();
            Dictionary<string, Code> supplierCodes = _codeManager.GetSupplierCodes(supplier, code[0], whenEffective);
            if (supplierCodes != null)
            {
                Code matchingCodes = null;
                StringBuilder codeValue = new StringBuilder(_codeManager.GetDigits(code));
                while (found == null && codeValue.Length > 0)
                {

                    if (supplierCodes.TryGetValue(codeValue.ToString(), out matchingCodes))
                    {
                        found = matchingCodes;
                        bool IsCodeGroup = codeGroups.ContainsKey(codeValue.ToString());
                        break;
                    }
                    codeValue.Length--;
                }
            }
            return found;
        }

    }
}
