//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using TOne.Data.SQL;


//namespace TOne.LCR.Data.SQL
//{
//    public class CodePrefixGroupDataManager : BaseTOneDataManager, ICodePrefixGroupDataManager
//    {
//        public void PrepareCodePrefixGroupToDB(int prefixLength)
//        {
//            throw new NotImplementedException();
//        }

//        /// <summary>
//        /// Get Codes From LCR.RootCodePrefixTable to be used in Building Code Match
//        /// </summary>
//        /// <returns>A list of Distinct Code Prfix Groups</returns>
//        public List<string> GetCodePrefixGroups()
//        {

//            return GetItemsSP("LCR.sp_RootCodePrefix_Get", (reader) =>
//            {
//                return reader["Code"] as string;
//            });
//        }

//    }
//}
