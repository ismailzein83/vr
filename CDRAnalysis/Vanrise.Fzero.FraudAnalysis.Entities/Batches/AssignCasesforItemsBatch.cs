using System.Collections.Generic;

namespace Vanrise.Fzero.FraudAnalysis.Entities
{
    public class AssignCasesforItemsBatch
    {
        public List<string> AccountNumbers; // To create account cases and account case history.
        public Dictionary<string,int> AccountNumberCaseIds; // to link items to cases
        public List<AccountInfo> TobeInsertedAccountInfos;
        public List<AccountInfo> TobeUpdatedAccountInfos;
    }
}
