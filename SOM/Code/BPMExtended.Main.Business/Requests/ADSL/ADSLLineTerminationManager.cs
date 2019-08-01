using BPMExtended.Main.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Terrasoft.Core;
using Terrasoft.Core.DB;

namespace BPMExtended.Main.Business
{
    public class ADSLLineTerminationManager
    {
        #region User Connection
        public UserConnection BPM_UserConnection
        {
            get
            {
                return (UserConnection)HttpContext.Current.Session["UserConnection"];
            }
        }
        #endregion

        #region public

        public SaleService GetVPNServiceFee(string contractId)
        {
            bool isVPNServiceFound = false;
            List<CustomerContractServiceDetail> contractServices = new ServiceManager().GetContractServicesDetail(contractId.ToString());
            List<SaleService> vpnServices = new CatalogManager().GetVPNDivisionServices();

            isVPNServiceFound = contractServices.Any(x => vpnServices.Any(y => y.Id == x.Id));

            /*foreach (var item in vpnServices)
            {
                foreach (var Service in contractServices)
                {
                    if (Service.Id == item.Id)
                    {
                        isVPNServiceFound = true;
                        break;
                    }
                }
                if (isVPNServiceFound) break;
            }*/

            if (isVPNServiceFound)
            {
                SaleService vpnServiceFee = new CatalogManager().GetVPNServiceFee();
                vpnServiceFee.UpFront = true;
                return vpnServiceFee;
            }
            return null;
        }

        public void PostADSLLineTerminationToOM(Guid requestId)
        {
            UserConnection connection = (UserConnection)HttpContext.Current.Session["UserConnection"];
            var update = new Update(connection, "StRequestHeader").Set("StStatusId", Column.Parameter("8057E9A4-24DE-484D-B202-0D189F5B7758"))
                .Where("StRequestId").IsEqual(Column.Parameter(requestId));
            update.Execute();

            //TODO : Get taxes
            //TODO: If the contract has active VPN service, CRM should add another OCC/fees
        }
        #endregion
    }
}
