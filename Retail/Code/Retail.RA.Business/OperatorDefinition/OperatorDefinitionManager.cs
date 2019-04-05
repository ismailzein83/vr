using Retail.BusinessEntity.Business;
using Retail.BusinessEntity.Entities;
using Retail.RA.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;

namespace Retail.RA.Business
{
    public class OperatorDefinitionManager
    {
        //static Guid beDefinitionId = new Guid("1A4A2877-D4C0-4B97-B4F0-2942BA342485");
        //public IEnumerable<OperatorDefinitionInfo> GetOperatorDefinitionInfo()
        //{
        //    List<OperatorDefinitionInfo> operatorDefinitionInfo = new List<OperatorDefinitionInfo>();
        //    AccountBEManager accountBEManager = new AccountBEManager();
        //    AccountFilter filter = new AccountFilter();
        //    var genericBEInfos = accountBEManager.GetAccountsInfo(beDefinitionId,null, filter);
        //    if (genericBEInfos == null)
        //        throw new NullReferenceException("genericBEInfo");
        //    foreach (var genericBEInfo in genericBEInfos)
        //    {
        //        if (genericBEInfo != null)
        //        {
        //            operatorDefinitionInfo.Add(new OperatorDefinitionInfo
        //            {
        //                OperatorId = (long)genericBEInfo.AccountId,
        //                OperatorName = genericBEInfo.Name
        //            });
        //        }
        //    }
        //    return operatorDefinitionInfo;
        //}
    }
}
