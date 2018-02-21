using SOM.Main.Business;
using SOM.Main.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Vanrise.Web.Base;

namespace SOM.Main.Web.Controllers
{
    [JSONWithTypeAttribute]
    [RoutePrefix(Constants.ROUTE_PREFIX + "Contract")]
    public class ContractController
    {
        static ContractManager s_manager = new ContractManager();

        public List<TelephonyContractInfo> GetTelephonyContracts(string billingAccountId, string telephonyNumber)
        {
            return s_manager.GetTelephonyContracts(billingAccountId, telephonyNumber);
        }

        public List<LeasedLineContractInfo> GetLeasedLineContracts(string billingAccountId, string telephonyNumber)
        {
            return s_manager.GetLeasedLineContracts(billingAccountId, telephonyNumber);
        }

        public List<XDSLContractInfo> GetXDSLContracts(string billingAccountId, string telephonyNumber)
        {
            return s_manager.GetXDSLContracts(billingAccountId, telephonyNumber);
        }
    }
}