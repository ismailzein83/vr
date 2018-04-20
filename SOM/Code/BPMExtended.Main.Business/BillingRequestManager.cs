using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPMExtended.Main.Common;
using BPMExtended.Main.Entities;
using SOM.Main.BP.Arguments;
using SOM.Main.Entities;
using Vanrise.Common;

namespace BPMExtended.Main.Business
{
    public class BillingRequestManager
    {
        public List<TelephoneServiceDetail> GetServices(string ratePlanId)
        {
            List<TelephoneService> telephoneServices = null;
            using (SOMClient client = new SOMClient())
            {
                telephoneServices = client.Get<List<TelephoneService>>(String.Format("api/SOM_Main/Billing/GetServices?ratePlanId=" + ratePlanId));
            }
            List<TelephoneServiceDetail> result = new List<TelephoneServiceDetail>();
            if (telephoneServices != null)
            {
                foreach (var service in telephoneServices)
                {
                    var serviceDetail = new TelephoneServiceDetail
                    {
                        Description = service.Description,
                        Id = service.Id,
                        IsMain = service.IsMain,
                        Name = service.Name,
                        Package = service.Package,
                        SubscriptionFee = service.SubscriptionFee,
                        UsageFee = service.UsageFee,
                        RatePlanId = service.RatePlanId,
                        ServiceParams = service.ServiceParams != null ? service.ServiceParams.MapRecords(c => new BPMExtended.Main.Entities.ServiceParameters { Id = c.Id, Name = c.Name }).ToList() : null

                    };
                    result.Add(serviceDetail);
                }
            }
            return result;

        }

        public List<TelephoneServiceDetail> GetServices()
        {
            List<TelephoneService> telephoneServices = null;
            using (SOMClient client = new SOMClient())
            {
                telephoneServices = client.Get<List<TelephoneService>>(String.Format("api/SOM_Main/Billing/GetServices?ratePlanId="));
            }
            List<TelephoneServiceDetail> result = new List<TelephoneServiceDetail>();
            if (telephoneServices != null)
            {
                foreach (var service in telephoneServices)
                {
                    var serviceDetail = new TelephoneServiceDetail
                    {
                        Description = service.Description,
                        Id = service.Id,
                        IsMain = service.IsMain,
                        Name = service.Name,
                        Package = service.Package,
                        SubscriptionFee = service.SubscriptionFee,
                        UsageFee = service.UsageFee
                    };
                    result.Add(serviceDetail);
                }
            }
            return result;

        }

        public List<RatePlanDetail> GetRatePlans()
        {
            List<RatePlan> ratePlans = null;
            using (SOMClient client = new SOMClient())
            {

                ratePlans = client.Get<List<RatePlan>>(String.Format("api/SOM_Main/Billing/GetRatePlans"));
            }
            List<RatePlanDetail> result = new List<RatePlanDetail>();
            if (ratePlans != null)
            {
                foreach (var ratePlan in ratePlans)
                {
                    var ratePlanDetail = new RatePlanDetail
                    {
                        Description = ratePlan.Description,
                        Id = ratePlan.Id,
                        Name = ratePlan.Name,
                        Type = (BPMExtended.Main.Entities.RatePlanType)ratePlan.Type
                    };
                    result.Add(ratePlanDetail);
                }
            }
            return result;

        }

        public CreateCustomerRequestOutput CreateBSCSBillingAccount(CustomerObjectType customerObjectType, Guid accountOrContactId)
        {
            NewCustomerCreationSomRequestSetting newCustomerCreationSomRequestSetting = new NewCustomerCreationSomRequestSetting
            {
                CustomerId = accountOrContactId
            };
            string title = string.Format("New Customer Creation Process Input: '{0}'", newCustomerCreationSomRequestSetting.CustomerId);

            return Helper.CreateSOMRequest(customerObjectType, accountOrContactId, title, newCustomerCreationSomRequestSetting);
        }
    }
}
