using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.LCR.Data
{
    public interface ICodePrefixGroupDataManager : IDataManager
    {
        /// <summary>
        /// Insert Code Prefix Codes to LCR.RootPrefixCode
        /// </summary>
        /// <param name="prefixLength"></param>
        void PrepareCodePrefixGroupToDB(int prefixLength);

        /// <summary>
        /// Get Codes From LCR.RootCodePrefixTable to be used in Building Code Match
        /// </summary>
        /// <returns>A list of Distinct Code Prfix Groups</returns>
        List<string> GetCodePrefixGroups();

    }
}
