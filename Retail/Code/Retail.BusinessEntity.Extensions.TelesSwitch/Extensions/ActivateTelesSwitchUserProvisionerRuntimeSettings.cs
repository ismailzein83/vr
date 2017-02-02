using Retail.BusinessEntity.Business;
using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Integration.Entities;

namespace Retail.BusinessEntity.Extensions.TelesSwitch
{
    public class ActivateTelesSwitchUserProvisionerRuntimeSettings : ActionProvisioner
    {
        //public string Domain { get; set; }
        //public string GateWay { get; set; }
        //public string LoginName { get; set; }
        //public string Password { get; set; }
        public override void Execute(IActionProvisioningContext context)
        {
        //    RequestManager manager = new RequestManager();
        //    string url = String.Format("https://c5-iot2-prov.teles.de/SIPManagement/rest/v1/domain/{0}/user?gateway={1}", this.Domain, this.GateWay);
        //    UserName userName = new UserName
        //    {
        //        firstName = context.Entity.Name,
        //        lastName = context.Entity.Name,
        //        loginName = this.LoginName,
        //        loginPassword = this.Password,
        //        auth = "FULL",
        //        pin = "0000",
        //        role = "END_USER",
        //        state = "ACTIVE"

        //    };
        //    string userNameData = Vanrise.Common.Serializer.Serialize(userName, true);
        //    var data = Encoding.UTF8.GetBytes(userNameData);
        //    manager.PostRequest(url, data);
        //    if (context.ExecutedActionsData == null)
        //        context.ExecutedActionsData = new List<object>();
        //    context.ExecutedActionsData.Add(new ProvisioningData
        //    {
        //        LoginName = this.LoginName,
        //        Domain = this.Domain,
        //        GateWay = this.GateWay
        //    });

        //    #region Add Or Update Account Service

        //    AccountServiceManager accountServiceManager = new Business.AccountServiceManager();

        //    var settings = context.DefinitionSettings as ActivateTelesSwitchUserProvisionerDefinitionSettings;
        //    if (settings != null && settings.ServiceTypeIds != null)
        //    {
        //        foreach (var serviceTypeId in settings.ServiceTypeIds)
        //        {
        //            AccountService accountService = accountServiceManager.GetAccountService(context.Entity.AccountId, serviceTypeId);
        //            var activeStatusId = new Guid("dcf62e68-80f4-45d6-8099-5a64523f7ec9");
        //            if (accountService == null)
        //            {
        //                accountService = new AccountService
        //                {
        //                    AccountId = context.Entity.AccountId,
        //                    StatusId = activeStatusId,
        //                    ServiceTypeId = serviceTypeId,
        //                };
        //                long accountServiceId = -1;
        //                accountServiceManager.AddAccountService(accountService, out accountServiceId);
        //            }
        //            else
        //            {
        //                accountService.StatusId = activeStatusId;
        //                accountServiceManager.UpdateAccountServiceEntity(accountService);
        //            }
        //        }
        //    }
        //    #endregion

        //    context.ExecutionOutput = new ActionProvisioningOutput
        //    {
        //        Result = ActionProvisioningResult.Succeeded,
        //    };
        }

    }


    //public class ProvisioningData
    //{
    //    public string LoginName { get; set; }
    //    public string Domain { get; set; }
    //    public string GateWay { get; set; }
    //}
    //public class Gateway
    //{
    //    public string name { get; set; }
    //    public string description { get; set; }
    //}
    //public class Site
    //{
    //    public string name { get; set; }
    //    public string description { get; set; }
    //    public int maxCalls { get; set; }
    //    public int maxCallsPerUser { get; set; }
    //    public int maxRegistrations { get; set; }
    //    public int maxRegsPerUser { get; set; }
    //    public int maxSubsPerUser { get; set; }
    //    public int maxBusinessTrunkCalls { get; set; }
    //    public int maxUsers { get; set; }
    //    public string ringBackUri { get; set; }
    //    public bool registrarEnabled { get; set; }
    //    public bool registrarAuthRequired { get; set; }
    //    public bool presenceEnabled { get; set; }

    //}
    //public class UserName
    //{
    //    public long id { get; set; }
    //    public string firstName { get; set; }
    //    public string lastName { get; set; }
    //    public string state { get; set; }
    //    public string loginName { get; set; }
    //    public string loginPassword { get; set; }
    //    public string role { get; set; }
    //    public string auth { get; set; }
    //    public string pin { get; set; }
    //    //public Guid uuid { get; set; }
    //    //public long domainId { get; set; }
    //    //public string domainType { get; set; }
    //    //public long routingGroupId { get; set; }
    //    //public long featureGroupId { get; set; }
    //    //public long locationId { get; set; }
    //    //public long gatewayId { get; set; }
    //    //public long maxRegistrations { get; set; }
    //    //public long maxCalls { get; set; }
    //    //public bool resetCallCount { get; set; }
    //}
}
